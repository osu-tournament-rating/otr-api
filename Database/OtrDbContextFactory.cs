using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Database;

public class OtrDbContextFactory : IDesignTimeDbContextFactory<OtrContext>
{
    public OtrContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OtrContext>();
        optionsBuilder.UseNpgsql();

        return new OtrContext(optionsBuilder.Options);
    }
}
