using Enumerations.Enums;
using Enumerations.Helpers;

namespace API.Helpers;

public static class AuthorizationSetup
{
    public static void AddAuthorizationSetup(this IServiceCollection services)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        var claims = Enum.GetValues(typeof(ClaimsEnum)).Cast<ClaimsEnum>().ToList();

        foreach (var claim in claims)
        {
            var description = Utilities.GetEnumDescription(claim);
            var model = description.Split("-")[0];
            var value = description.Split("-")[1].ToLower();
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy(claim.ToString(), policy => policy.RequireClaim(model, value));
            });
        }
    }
}