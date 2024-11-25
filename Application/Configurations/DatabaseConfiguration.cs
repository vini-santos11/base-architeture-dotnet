using Application.Configurations.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.Configurations;

public class DatabaseConfiguration : IDatabaseConfiguration
{
    private readonly IConfiguration _configuration;
    public DatabaseConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetConnectionString()
    {
        return _configuration.GetConnectionString("DefaultConnection");
    }
}