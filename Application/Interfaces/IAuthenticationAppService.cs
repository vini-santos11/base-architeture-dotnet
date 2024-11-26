using Application.Commands.User;

namespace Application.Interfaces;

public interface IAuthenticationAppService
{
    Task<string> Register(CreateUserCommand command);
    Task ConfirmRegister(ConfirmRegisterCommand command);
    Task<string> ResendRegisterCode(RequestCodeCommand command);
    Task<string> RequestRecoverPassword(RequestCodeCommand command);
    Task<bool> CheckRecoveryCodeIsValid(CheckRecoveryCodeCommand command);
    Task RecoverPassword(RecoveryPasswordCommand command);

}