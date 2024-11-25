using Application.Commands.User;
using Application.Queries.User;

namespace Application.Interfaces;

public interface IUserAppService : IBaseAppService<CreateUserCommand, UserQuery>
{
    
}