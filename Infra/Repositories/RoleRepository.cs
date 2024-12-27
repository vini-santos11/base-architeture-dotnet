using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.Context;

namespace Infra.Repositories;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(BaseContext context) : base(context)
    {
        
    }
}