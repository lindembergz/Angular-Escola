using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.Escolas.Infraestrutura.Contexto;
using SistemaGestaoEscolar.Auth.Infrastructure.Mappings;
using SistemaGestaoEscolar.Auth.Domain.Entities;
using SistemaGestaoEscolar.Alunos.Infraestrutura.Persistencia.Entidades;
using SistemaGestaoEscolar.Professores.Infraestrutura.Persistencia.Entidades;
using SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.API.Configuration;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets for Auth module
    public DbSet<User> Usuarios { get; set; } = null!;
    public DbSet<Session> Sessoes { get; set; } = null!;

    // DbSets for Alunos module
    public DbSet<AlunoEntity> Alunos { get; set; } = null!;
    public DbSet<ResponsavelEntity> Responsaveis { get; set; } = null!;
    public DbSet<MatriculaEntity> Matriculas { get; set; } = null!;

    // DbSets for Professores module
    public DbSet<ProfessorEntity> Professores { get; set; } = null!;
    public DbSet<TituloAcademicoEntity> TitulosAcademicos { get; set; } = null!;
    public DbSet<ProfessorDisciplinaEntity> ProfessorDisciplinas { get; set; } = null!;

    // DbSets for Academico module
    public DbSet<TurmaEntity> Turmas { get; set; } = null!;
    public DbSet<DisciplinaEntity> Disciplinas { get; set; } = null!;
    public DbSet<HorarioEntity> Horarios { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from all modules
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EscolasDbContext).Assembly);
        
        // Apply Auth module configurations manually since we can't reference AuthDbContext
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SistemaGestaoEscolar.Auth.Infrastructure.Mappings.UserMapping).Assembly);
        
        // Apply Alunos module configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SistemaGestaoEscolar.Alunos.Infraestrutura.Persistencia.Configuracoes.AlunoEntityConfiguration).Assembly);
        
        // Apply Professores module configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SistemaGestaoEscolar.Professores.Infraestrutura.Persistencia.Configuracoes.ProfessorEntityConfiguration).Assembly);

        // Apply Academico module configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Configuracao.TurmaConfiguration).Assembly);

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