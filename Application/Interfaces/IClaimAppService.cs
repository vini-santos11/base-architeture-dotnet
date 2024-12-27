using Application.Queries.Claim;

namespace Application.Interfaces;

public interface IClaimAppService
{
    public List<ClaimQuery> GetClaims();
}