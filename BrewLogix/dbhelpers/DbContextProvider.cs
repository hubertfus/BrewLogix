using BrewLogix.Services;
using Microsoft.EntityFrameworkCore;

namespace BrewLogix.dbhelpers;

public interface IDbContextProvider
{
    AppDbContext GetDbContext();
    void SetConnection();
}
public class DbContextProvider : IDbContextProvider
{
    private readonly IDbContextFactory<AppDbContext> _factory;
    private readonly IConfiguration _configuration;
    private string _connectionString;

    public DbContextProvider(IDbContextFactory<AppDbContext> factory, IConfiguration configuration)
    {
        _factory = factory;
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }

    public void SetConnection()
    {
        _connectionString = _configuration.GetConnectionString(DBName.Name);
    }

    public AppDbContext GetDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(_connectionString);
        return new AppDbContext(optionsBuilder.Options);
    }
}
