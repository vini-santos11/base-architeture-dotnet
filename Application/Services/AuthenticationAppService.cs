using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
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

    public async Task<UserTokenQuery> Login(LoginCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email) ?? throw new Exception("User not found");
        if (!await _userManager.CheckPasswordAsync(user, command.Password)) throw new Exception("Invalid password");

        var result = await _signInManager.PasswordSignInAsync(user, command.Password, false, false);
        if (!result.Succeeded) throw new Exception("Invalid password");
        return await SetToken(user);
    }
    
    public async Task<string> Register(CreateUserCommand command)
    {
        try
        {
            var user = _mapper.Map<User>(command);
            if(await _userManager.FindByEmailAsync(user.Email) is not null)
                throw new Exception("Email already in use");
            if(_userManager.Users.FirstOrDefault(x => x.Document == user.Document) is not null)
                throw new Exception("Document already in use");

            user.RegisterCode = RandomStringCode(6);
            user.ExpirationRegisterCode = DateTime.UtcNow.AddMinutes(15);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdateAt = DateTime.UtcNow;
            
            await _userStore.SetUserNameAsync(user, user.Document.ToLower(), CancellationToken.None);
            
            var result = await _userManager.CreateAsync(user, command.Password);
            if (!result.Succeeded)
                throw new Exception(result.Errors.Select(e => e.Description).Aggregate((a, b) => $"{a}, {b}"));
            
            //todo send email with code

            return user.RegisterCode;
        } catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task ConfirmRegister(ConfirmRegisterCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email) ?? throw new Exception("User not found");
        if(user.RegisterCode != command.Code) throw new Exception("Invalid code");
        
        if(user.ExpirationRegisterCode < DateTime.UtcNow) throw new Exception("Code expired");
        
        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var result = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);
        
        if (!result.Succeeded) throw new Exception(result.Errors.Select(e => e.Description).Aggregate((a, b) => $"{a}, {b}"));
        
        var role = _roleManager.Roles.FirstOrDefault(x => x.Name.ToLower() == "User".ToLower()) ?? throw new Exception("Role User not found");
        await _userManager.AddToRoleAsync(user, role.Name);
    }
    
    public async Task<string> ResendRegisterCode(RequestCodeCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email) ?? throw new Exception("User not found");
        user.RegisterCode = RandomStringCode(6);
        user.ExpirationRegisterCode = DateTime.UtcNow.AddMinutes(15);
        await _userManager.UpdateAsync(user);
        
        //todo send email with code
        
        return user.RegisterCode;
    }

    public async Task<string> RequestRecoverPassword(RequestCodeCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email) ?? throw new Exception("User not found");
        user.RecoveryCode = RandomStringCode(6);
        user.ExpirationRecoveryCode = DateTime.UtcNow.AddMinutes(15);
        await _userManager.UpdateAsync(user);
        
        //todo send email with code
        
        return user.RecoveryCode;
    }

    public async Task<bool> CheckRecoveryCodeIsValid(CheckRecoveryCodeCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email) ?? throw new Exception("User not found");
        if(user.RecoveryCode != command.RecoveryCode) return false;
        
        if(user.ExpirationRecoveryCode < DateTime.UtcNow) return false;
        return true;
    }
    
    public async Task RecoverPassword(RecoveryPasswordCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email) ?? throw new Exception("User not found");
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, command.NewPassword);
        
        if (!result.Succeeded) throw new Exception(result.Errors.Select(e => e.Description).Aggregate((a, b) => $"{a}, {b}"));
    }
    
    private string RandomStringCode(int length)
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