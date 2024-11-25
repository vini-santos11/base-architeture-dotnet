using Microsoft.OpenApi.Models;

namespace API.Helpers;

public static class SwaggerSetup
{
    public static void AddSwaggerSetup(this IServiceCollection services)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "API",
                Description = "API using .NET",
                TermsOfService = new Uri("https://termsofservice.com.br/"),
                Contact = new OpenApiContact
                {
                    Name = "Contact",
                    Email = "contact@contact.com.br",
                    Url = new Uri("https://contact.com.br/contato.html")
                },
            });
        });
    }
    
    public static void UseSwaggerSetup(this IApplicationBuilder app)
    {
        if (app == null)
            throw new ArgumentNullException(nameof(app));

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
        });
    }
}