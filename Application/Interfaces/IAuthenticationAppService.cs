using Application.Commands.User;

namespace Application.Interfaces;

public interface IAuthenticationAppService
{
    Task<string> Register(CreateUserCommand command);
    Task ConfirmRegister(ConfirmRegisterCommand command);
}