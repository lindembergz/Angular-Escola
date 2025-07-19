using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.Auth.Domain.Entities;
using SistemaGestaoEscolar.Auth.Infrastructure.Mappings;

namespace SistemaGestaoEscolar.Auth.Infrastructure.Context;

/// <summary>
/// Contexto do Entity Framework para o módulo de autenticação.
/// Configura mapeamentos e relacionamentos das entidades de domínio.
/// </summary>
public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Session> Sessions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configurações de mapeamento
        modelBuilder.ApplyConfiguration(new UserMapping());
        modelBuilder.ApplyConfiguration(new SessionMapping());

        // Configurações globais
        ConfigureGlobalSettings(modelBuilder);
    }

    private static void ConfigureGlobalSettings(ModelBuilder modelBuilder)
    {
        // Configurar precisão para campos DateTime
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

        // Configurar charset padrão para MySQL
        modelBuilder.HasCharSet("utf8mb4");
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Atualizar timestamps automaticamente
        UpdateTimestamps();
        
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        // Atualizar timestamps automaticamente
        UpdateTimestamps();
        
        return base.SaveChanges();
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Domain.Entities.User && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            if (entry.Entity is Domain.Entities.User user)
            {
                if (entry.State == EntityState.Modified)
                {
                    user.MarkAsUpdated();
                }
            }
        }
    }
}