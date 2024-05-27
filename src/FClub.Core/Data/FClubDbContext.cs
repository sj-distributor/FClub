using System.Reflection;
using FClub.Core.Domain;
using FClub.Core.Settings;
using Microsoft.EntityFrameworkCore;

namespace FClub.Core.Data;

public class FClubDbContext : DbContext, IUnitOfWork
{
    private readonly FClubConnectionString _connectionString;

    public FClubDbContext(FClubConnectionString connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(_connectionString.Value, new MySqlServerVersion(new Version(8, 0, 28)));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        typeof(FClubDbContext).GetTypeInfo().Assembly.GetTypes()
            .Where(t => typeof(IEntity).IsAssignableFrom(t) && t.IsClass).ToList()
            .ForEach(x =>
            {
                if (modelBuilder.Model.FindEntityType(x) == null)
                    modelBuilder.Model.AddEntityType(x);
            });
    }
    
    public bool ShouldSaveChanges { get; set; }
}