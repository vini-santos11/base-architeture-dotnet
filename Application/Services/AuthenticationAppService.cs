using Application.Commands.User;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class AuthenticationAppService : IAuthenticationAppService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IUserStore<User> _userStore;
    private readonly IMapper _mapper;
    public AuthenticationAppService(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IUserStore<User> userStore,
        IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userStore = userStore;
        _mapper = mapper;
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
        var user = _userManager.Users.FirstOrDefault(x => x.Document == command.Document) ?? throw new Exception("User not found");
        if(user.RegisterCode != command.Code) throw new Exception("Invalid code");
        
        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var result = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);
        
        if (!result.Succeeded) throw new Exception(result.Errors.Select(e => e.Description).Aggregate((a, b) => $"{a}, {b}"));
        
        var role = _roleManager.Roles.FirstOrDefault(x => x.Name.ToLower() == "User".ToLower()) ?? throw new Exception("Role User not found");
        await _userManager.AddToRoleAsync(user, role.Name);
    }
    
    private string RandomStringCode(int length)
    {
        Random random = new();
        const string chars = "0123456789";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}