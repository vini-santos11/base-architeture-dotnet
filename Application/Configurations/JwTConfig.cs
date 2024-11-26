using System.Net;
using System.Text;
using Application.Models;
using Enumerations.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Application.Configurations;

public static class JwTConfig
{
    public static void AddJwtConfig(this IServiceCollection services, IConfiguration configuration)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        var tokenConfiguration = ConfigureTokenSettings(configuration);
        services.AddSingleton(tokenConfiguration);
        var signingKey = CreateSigningKey(tokenConfiguration.Secret);
        var jsonSerializerSettings = CreateJsonSerializerSettings();

        ConfigureAuthentication(services, signingKey, jsonSerializerSettings);
    }

    private static TokenConfiguration ConfigureTokenSettings(IConfiguration configuration)
    {
        var tokenConfiguration = new TokenConfiguration();
        configuration.GetSection("TokenConfiguration").Bind(tokenConfiguration);
        return tokenConfiguration;
    }

    private static SymmetricSecurityKey CreateSigningKey(string secret)
    {
        var keyBytes = Encoding.ASCII.GetBytes(secret);
        return new SymmetricSecurityKey(keyBytes);
    }

    private static JsonSerializerSettings CreateJsonSerializerSettings()
    {
        return new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy { ProcessDictionaryKeys = true }
            },
            Formatting = Formatting.Indented
        };
    }

    private static void ConfigureAuthentication(IServiceCollection services, SymmetricSecurityKey signingKey, JsonSerializerSettings serializerSettings)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = CreateTokenValidationParameters(signingKey);
            options.Events = CreateJwtBearerEvents(serializerSettings);
        });
    }

    private static TokenValidationParameters CreateTokenValidationParameters(SecurityKey signingKey)
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    }

    private static JwtBearerEvents CreateJwtBearerEvents(JsonSerializerSettings serializerSettings)
    {
        return new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                context.Response.StatusCode = context.Exception switch
                {
                    SecurityTokenInvalidAudienceException => (int)HttpStatusCode.ExpectationFailed,
                    SecurityTokenInvalidSignatureException => (int)HttpStatusCode.Unauthorized,
                    SecurityTokenExpiredException => (int)HttpStatusCode.FailedDependency,
                    _ => context.Response.StatusCode
                };
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();

                if (context.AuthenticateFailure == null || context.Error == "invalid_token")
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }

                var response = new Response<object>
                {
                    Success = false,
                    Message = "Unauthorized",
                    Errors = context.AuthenticateFailure != null
                        ? new Dictionary<string, List<string>>
                        {
                            { "Data", new List<string> { Utilities.GetExceptionList(context.AuthenticateFailure) } }
                        }
                        : new Dictionary<string, List<string>>()
                };

                return context.Response.WriteAsync(JsonConvert.SerializeObject(response, serializerSettings));
            }
        };
    }
}