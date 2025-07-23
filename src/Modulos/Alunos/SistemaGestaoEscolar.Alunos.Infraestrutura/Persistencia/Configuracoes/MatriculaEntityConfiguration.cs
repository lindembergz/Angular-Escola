using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestaoEscolar.Alunos.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Alunos.Infraestrutura.Persistencia.Configuracoes;

public class MatriculaEntityConfiguration : IEntityTypeConfiguration<MatriculaEntity>
{
    public void Configure(EntityTypeBuilder<MatriculaEntity> builder)
    {
        builder.ToTable("Matriculas");

        // Primary Key
        builder.HasKey(m => m.Id);

        // Properties
        builder.Property(m => m.AlunoId)
            .IsRequired();

        builder.Property(m => m.TurmaId)
            .IsRequired();

        builder.Property(m => m.AnoLetivo)
            .IsRequired();

        builder.Property(m => m.DataMatricula)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(m => m.DataCancelamento)
            .HasColumnType("date");

        builder.Property(m => m.MotivoCancelamento)
            .HasMaxLength(500);

        builder.Property(m => m.Ativa)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(m => m.NumeroMatricula)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(m => m.Status)
            .IsRequired();

        builder.Property(m => m.Observacoes)
            .HasMaxLength(1000);

        // Auditoria
        builder.Property(m => m.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(m => m.UpdatedAt)
            .HasColumnType("datetime(6)");

        // Indexes
        builder.HasIndex(m => m.AlunoId)
            .HasDatabaseName("IX_Matriculas_AlunoId");

        builder.HasIndex(m => m.TurmaId)
            .HasDatabaseName("IX_Matriculas_TurmaId");

        builder.HasIndex(m => new { m.AlunoId, m.AnoLetivo })
            .HasDatabaseName("IX_Matriculas_AlunoId_AnoLetivo");

        builder.HasIndex(m => m.Status)
            .HasDatabaseName("IX_Matriculas_Status");

        // Relationship
        builder.HasOne(m => m.Aluno)
            .WithMany(a => a.Matriculas)
            .HasForeignKey(m => m.AlunoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}