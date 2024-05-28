namespace Core;

public interface IDbServiceProvider
{
    public DbService Get();
}