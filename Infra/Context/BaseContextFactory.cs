using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infra.Context;

public class BaseContextFactory : IDesignTimeDbContextFactory<BaseContext>
{
    public BaseContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BaseContext>();
        return new BaseContext(optionsBuilder.Options);
    }
}