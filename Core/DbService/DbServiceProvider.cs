using UrFUEducationalModules.Data;

namespace Core;

public class DbServiceProvider : IDbServiceProvider
{
    private readonly Func<ApplicationDbContext> coreDbContextGenerator;

    public DbServiceProvider(Func<ApplicationDbContext> coreDbContextGenerator)
    {
        this.coreDbContextGenerator = coreDbContextGenerator;
    }

    public DbService Get() => new(coreDbContextGenerator());
}