using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.Escolas.Dominio.Entidades;
using SistemaGestaoEscolar.Escolas.Infraestrutura.Mapeamentos;

namespace SistemaGestaoEscolar.Escolas.Infraestrutura.Contexto;

public class EscolasDbContext : DbContext
{
    public DbSet<Escola> Escolas { get; set; } = null!;
    public DbSet<RedeEscolar> RedesEscolares { get; set; } = null!;
    public DbSet<UnidadeEscolar> UnidadesEscolares { get; set; } = null!;

    public EscolasDbContext(DbContextOptions<EscolasDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configurações das entidades
        modelBuilder.ApplyConfiguration(new EscolaConfiguration());
        modelBuilder.ApplyConfiguration(new RedeEscolarConfiguration());
        modelBuilder.ApplyConfiguration(new UnidadeEscolarConfiguration());

        // Configurações globais
        ConfigureGlobalSettings(modelBuilder);
    }

    private static void ConfigureGlobalSettings(ModelBuilder modelBuilder)
    {
        // Configurar todas as strings como VARCHAR por padrão (MySQL)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(string))
                {
                    property.SetColumnType("varchar");
                }
            }
        }

        // Configurar precisão para DateTime
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
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Atualizar timestamps antes de salvar
        UpdateTimestamps();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Escola || e.Entity is RedeEscolar || e.Entity is UnidadeEscolar);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                if (entry.Entity is Escola escola)
                {
                    escola.MarkAsUpdated();
                }
                else if (entry.Entity is RedeEscolar rede)
                {
                    rede.MarkAsUpdated();
                }
                else if (entry.Entity is UnidadeEscolar unidade)
                {
                    unidade.MarkAsUpdated();
                }
            }
        }
    }
}