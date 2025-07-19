using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.Escolas.Infraestrutura.Contexto;
using SistemaGestaoEscolar.Auth.Infrastructure.Mappings;
using SistemaGestaoEscolar.Auth.Domain.Entities;

namespace SistemaGestaoEscolar.API.Configuration;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets for Auth module
    public DbSet<User> Usuarios { get; set; } = null!;
    public DbSet<Session> Sessoes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from all modules
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EscolasDbContext).Assembly);
        
        // Apply Auth module configurations manually since we can't reference AuthDbContext
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SistemaGestaoEscolar.Auth.Infrastructure.Mappings.UserMapping).Assembly);

        // Configure global settings for MySQL
        ConfigureGlobalSettings(modelBuilder);
    }

    private static void ConfigureGlobalSettings(ModelBuilder modelBuilder)
    {
        // Configure precision for DateTime fields
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetColumnType("datetime(6)");
                }
            }
        }

        // Configure default charset for MySQL
        modelBuilder.HasCharSet("utf8mb4");
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps automatically
        UpdateTimestamps();
        
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        // Update timestamps automatically
        UpdateTimestamps();
        
        return base.SaveChanges();
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is User && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            if (entry.Entity is User user)
            {
                if (entry.State == EntityState.Modified)
                {
                    user.MarkAsUpdated();
                }
            }
        }
    }
}