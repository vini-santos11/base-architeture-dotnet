using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Domain.Services;

public class RoleService : BaseService<Role, IRoleRepository>, IRoleService
{
    public RoleService(IRoleRepository repository) : base(repository)
    {
        
    }
}