using Application.Commands.User;
using Application.Models;
using Application.Queries.User;

namespace Application.Interfaces;

public interface IAuthenticationAppService
{
    Task<Response<UserTokenQuery>> Login(LoginCommand command);
    Task<Response<string>> Register(CreateUserCommand command);
    Task<Response<bool>> ConfirmRegister(ConfirmRegisterCommand command);
    Task<Response<string>> ResendRegisterCode(RequestCodeCommand command);
    Task<Response<string>> RequestRecoverPassword(RequestCodeCommand command);
    Task<Response<bool>> CheckRecoveryCodeIsValid(CheckRecoveryCodeCommand command);
    Task<Response<bool>> RecoverPassword(RecoveryPasswordCommand command);
}