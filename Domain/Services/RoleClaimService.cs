using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Domain.Services;

public class RoleClaimService : BaseService<RoleClaim, IRoleClaimRepository>, IRoleClaimService
{
    public RoleClaimService(IRoleClaimRepository repository) : base(repository)
    {
        
    }
}