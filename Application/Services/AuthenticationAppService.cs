using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Application.Commands.User;
using Application.Interfaces;
using Application.Models;
using Application.Queries.User;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class AuthenticationAppService : IAuthenticationAppService
{
    private readonly TokenConfiguration _tokenConfiguration;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IUserStore<User> _userStore;
    private readonly IMapper _mapper;
    public AuthenticationAppService(
        TokenConfiguration tokenConfiguration,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IUserStore<User> userStore,
        SignInManager<User> signInManager,
        IMapper mapper)
    {
        _tokenConfiguration = tokenConfiguration;
        _userManager = userManager;
        _roleManager = roleManager;
        _userStore = userStore;
        _signInManager = signInManager;
        _mapper = mapper;
    }

    public async Task<Response<UserTokenQuery>> Login(LoginCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user is null) return Response.Fail<UserTokenQuery>("User not found", HttpStatusCode.Unauthorized);
        
        if (!await _userManager.CheckPasswordAsync(user, command.Password)) return Response.Fail<UserTokenQuery>("Invalid password", HttpStatusCode.Unauthorized);
        
        var result = await _signInManager.PasswordSignInAsync(user, command.Password, false, false);
        if (!result.Succeeded) return Response.Fail<UserTokenQuery>("Login failed", HttpStatusCode.Unauthorized);
        
        var token = await SetToken(user);
        return Response.Ok(token, "User logged in successfully");
    }

    public async Task<Response<string>> Register(CreateUserCommand command)
    {
        var user = _mapper.Map<User>(command);
        if (await _userManager.FindByEmailAsync(user.Email) is not null)
            return Response.Fail<string>("Email already in use");

        if (_userManager.Users.FirstOrDefault(x => x.Document == user.Document) is not null)
            return Response.Fail<string>("Document already in use");

        user.RegisterCode = RandomStringCode(6);
        user.ExpirationRegisterCode = DateTime.UtcNow.AddMinutes(15);
        user.CreatedAt = DateTime.UtcNow;
        user.UpdateAt = DateTime.UtcNow;

        await _userStore.SetUserNameAsync(user, user.Document.ToLower(), CancellationToken.None);

        var result = await _userManager.CreateAsync(user, command.Password);
        if (result.Succeeded) return Response.Ok(user.RegisterCode, "User registered successfully");
        
        var errors = result.Errors.Select(e => e.Description).ToList();
        return Response.Fail<string>(errors.First());

        //todo send email with code

    }

    public async Task<Response<bool>> ConfirmRegister(ConfirmRegisterCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user is null)
            return Response.Fail<bool>("User not found", HttpStatusCode.NotFound);

        if (user.RegisterCode != command.Code)
            return Response.Fail<bool>("Invalid code");

        if (user.ExpirationRegisterCode < DateTime.UtcNow)
            return Response.Fail<bool>("Code expired");

        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var result = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Response.Fail<bool>(errors.First());
        }

        var role = _roleManager.Roles.FirstOrDefault(x => x.Name.ToLower() == "User".ToLower());
        if (role is null)
            return Response.Fail<bool>("Role User not found", HttpStatusCode.InternalServerError);

        await _userManager.AddToRoleAsync(user, role.Name);
        return Response.Ok(true, "User confirmed successfully");
    }

    public async Task<Response<string>> ResendRegisterCode(RequestCodeCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user is null)
            return Response.Fail<string>("User not found", HttpStatusCode.NotFound);

        user.RegisterCode = RandomStringCode(6);
        user.ExpirationRegisterCode = DateTime.UtcNow.AddMinutes(15);
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded) return Response.Ok(user.RegisterCode, "Register code resent successfully");
        
        var errors = result.Errors.Select(e => e.Description).ToList();
        return Response.Fail<string>(errors.First());

        //todo send email with code

    }

    public async Task<Response<string>> RequestRecoverPassword(RequestCodeCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user is null)
            return Response.Fail<string>("User not found", HttpStatusCode.NotFound);

        user.RecoveryCode = RandomStringCode(6);
        user.ExpirationRecoveryCode = DateTime.UtcNow.AddMinutes(15);
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Response.Fail<string>(errors.First());
        }

        //todo send email with code

        return Response.Ok(user.RecoveryCode, "Recovery code sent successfully");
    }

    public async Task<Response<bool>> CheckRecoveryCodeIsValid(CheckRecoveryCodeCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user is null)
            return Response.Fail<bool>("User not found", HttpStatusCode.NotFound);

        if (user.RecoveryCode != command.RecoveryCode)
            return Response.Ok(false, "Recovery code is invalid");

        if (user.ExpirationRecoveryCode < DateTime.UtcNow)
            return Response.Ok(false, "Recovery code expired");

        return Response.Ok(true, "Recovery code is valid");
    }

    public async Task<Response<bool>> RecoverPassword(RecoveryPasswordCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user is null)
            return Response.Fail<bool>("User not found", HttpStatusCode.NotFound);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, command.NewPassword);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Response.Fail<bool>(errors.First());
        }

        return Response.Ok(true, "Password recovered successfully");
    }

    private static string RandomStringCode(int length)
    {
        Random random = new();
        const string chars = "0123456789";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private async Task<UserTokenQuery> SetToken(User user)
    {
        var expires = DateTime.UtcNow.AddMinutes(_tokenConfiguration.Minutes);

        var userRoles = await _userManager.GetRolesAsync(user);

        var role = await _roleManager.FindByNameAsync(userRoles.FirstOrDefault()!) ?? throw new Exception("Role not found");

        var roleClaims = await _roleManager.GetClaimsAsync(role);

        var claims = new List<Claim>
        {
            new (ClaimTypes.Name , user.Name),
            new (ClaimTypes.Email, user.Email!),
            new (ClaimTypes.Role, role.Name!),
            new (ClaimTypes.Sid, user.Id.ToString()),
            new (ClaimTypes.Expiration, expires.ToString(CultureInfo.InvariantCulture))
        };

        claims.AddRange(roleClaims);

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new GenericIdentity(user.Name, "Name"), claims),
            Expires = expires,
            NotBefore = DateTime.UtcNow,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenConfiguration.Secret)), SecurityAlgorithms.HmacSha256Signature)
        });

        return new UserTokenQuery
        {
            Id = user.Id,
            Name = user.Name,
            Document = user.Document,
            Email = user.Email,
            Role = role.Name,
            Token = tokenHandler.WriteToken(securityToken),
        };
    }
}