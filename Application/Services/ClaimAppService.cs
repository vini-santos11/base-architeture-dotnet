using Application.Interfaces;
using Application.Queries.Claim;
using Enumerations.Enums;
using Enumerations.Helpers;

namespace Application.Services;

public class ClaimAppService : IClaimAppService
{
    public List<ClaimQuery> GetClaims()
    {
        var claims = Enum.GetValues(typeof(ClaimsEnum)).Cast<ClaimsEnum>().ToList();
        var queries = new List<ClaimQuery>();
        
        claims.ForEach(claim =>
        {
            var description = Utilities.GetEnumDescription(claim);
            queries.Add(new ClaimQuery
            {
                Description = description,
                Model = description.Split("-")[0],
                Value = description.Split("-")[1].ToLower()
            });
        });
        return queries;
    }
}