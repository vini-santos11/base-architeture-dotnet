using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.Context;

namespace Infra.Repositories;

public class RoleClaimRepository : BaseRepository<RoleClaim>, IRoleClaimRepository
{
    public RoleClaimRepository(BaseContext context) : base(context)
    {
        
    }
}