using Models.Commands.User;
using Models.Queries.User;

namespace Application.Interfaces;

public interface IUserAppService : IBaseAppService<CreateUserCommand, UserQuery>
{
    
}