using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.Escolas.Infraestrutura.Contexto;

namespace SistemaGestaoEscolar.API.Configuration;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from all modules
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EscolasDbContext).Assembly);
    }
}