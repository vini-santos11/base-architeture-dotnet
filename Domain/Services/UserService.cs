using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Domain.Services;

public class UserService : BaseService<User, IUserRepository>, IUserService
{
    public UserService(IUserRepository repository) : base(repository)
    {
        
    }
}