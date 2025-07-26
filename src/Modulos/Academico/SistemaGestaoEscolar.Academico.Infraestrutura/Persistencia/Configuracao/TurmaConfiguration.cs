using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Configuracao;

public class TurmaConfiguration : IEntityTypeConfiguration<TurmaEntity>
{
    public void Configure(EntityTypeBuilder<TurmaEntity> builder)
    {
        builder.ToTable("Turmas");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Id)
            .IsRequired()
            .ValueGeneratedNever();
            
        builder.Property(t => t.Nome)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("varchar(100)");
            
        builder.Property(t => t.TipoSerie)
            .IsRequired()
            .HasColumnType("int");
            
        builder.Property(t => t.AnoSerie)
            .IsRequired()
            .HasColumnType("int");
            
        builder.Property(t => t.TipoTurno)
            .IsRequired()
            .HasColumnType("int");
            
        builder.Property(t => t.HoraInicioTurno)
            .IsRequired()
            .HasColumnType("time");
            
        builder.Property(t => t.HoraFimTurno)
            .IsRequired()
            .HasColumnType("time");
            
        builder.Property(t => t.CapacidadeMaxima)
            .IsRequired()
            .HasColumnType("int");
            
        builder.Property(t => t.AnoLetivo)
            .IsRequired()
            .HasColumnType("int");
            
        builder.Property(t => t.EscolaId)
            .IsRequired()
            .HasColumnType("char(36)");
            
        builder.Property(t => t.Ativa)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnType("tinyint(1)");
            
        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            
        builder.Property(t => t.UpdatedAt)
            .HasColumnType("datetime(6)");
        
        // Relacionamentos
        builder.HasMany(t => t.Disciplinas)
            .WithMany(d => d.Turmas)
            .UsingEntity("TurmaDisciplinas",
                l => l.HasOne(typeof(DisciplinaEntity)).WithMany().HasForeignKey("DisciplinaId"),
                r => r.HasOne(typeof(TurmaEntity)).WithMany().HasForeignKey("TurmaId"));
                
        builder.HasMany(t => t.Horarios)
            .WithOne(h => h.Turma)
            .HasForeignKey(h => h.TurmaId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(t => t.TurmaAlunos)
            .WithOne(ta => ta.Turma)
            .HasForeignKey(ta => ta.TurmaId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Índices
        builder.HasIndex(t => t.Nome)
            .HasDatabaseName("IX_Turmas_Nome");
            
        builder.HasIndex(t => t.EscolaId)
            .HasDatabaseName("IX_Turmas_EscolaId");
            
        builder.HasIndex(t => new { t.AnoLetivo, t.EscolaId })
            .HasDatabaseName("IX_Turmas_AnoLetivo_EscolaId");
            
        builder.HasIndex(t => new { t.TipoSerie, t.AnoSerie, t.EscolaId })
            .HasDatabaseName("IX_Turmas_Serie_EscolaId");
    }
}