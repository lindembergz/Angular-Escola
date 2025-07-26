using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Configuracao;

public class TurmaAlunoConfiguration : IEntityTypeConfiguration<TurmaAlunoEntity>
{
    public void Configure(EntityTypeBuilder<TurmaAlunoEntity> builder)
    {
        builder.ToTable("TurmaAlunos");
        
        builder.HasKey(ta => ta.Id);
        
        builder.Property(ta => ta.Id)
            .IsRequired()
            .ValueGeneratedNever();
            
        builder.Property(ta => ta.TurmaId)
            .IsRequired()
            .HasColumnType("char(36)");
            
        builder.Property(ta => ta.AlunoId)
            .IsRequired()
            .HasColumnType("char(36)");
            
        builder.Property(ta => ta.DataMatricula)
            .IsRequired()
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            
        builder.Property(ta => ta.DataDesmatricula)
            .HasColumnType("datetime(6)");
            
        builder.Property(ta => ta.Ativa)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnType("tinyint(1)");
            
        builder.Property(ta => ta.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            
        builder.Property(ta => ta.UpdatedAt)
            .HasColumnType("datetime(6)");
        
        // Índices
        builder.HasIndex(ta => ta.TurmaId)
            .HasDatabaseName("IX_TurmaAlunos_TurmaId");
            
        builder.HasIndex(ta => ta.AlunoId)
            .HasDatabaseName("IX_TurmaAlunos_AlunoId");
            
        builder.HasIndex(ta => new { ta.TurmaId, ta.AlunoId })
            .IsUnique()
            .HasDatabaseName("IX_TurmaAlunos_TurmaId_AlunoId_Unique");
            
        builder.HasIndex(ta => new { ta.AlunoId, ta.Ativa })
            .HasDatabaseName("IX_TurmaAlunos_AlunoId_Ativa");
    }
}