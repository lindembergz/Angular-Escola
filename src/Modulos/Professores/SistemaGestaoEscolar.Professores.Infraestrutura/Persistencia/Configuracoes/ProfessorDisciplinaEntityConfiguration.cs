using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestaoEscolar.Professores.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Professores.Infraestrutura.Persistencia.Configuracoes;

public class ProfessorDisciplinaEntityConfiguration : IEntityTypeConfiguration<ProfessorDisciplinaEntity>
{
    public void Configure(EntityTypeBuilder<ProfessorDisciplinaEntity> builder)
    {
        builder.ToTable("ProfessorDisciplinas");

        builder.HasKey(pd => pd.Id);

        builder.Property(pd => pd.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(pd => pd.ProfessorId)
            .IsRequired();

        builder.Property(pd => pd.DisciplinaId)
            .IsRequired();

        builder.Property(pd => pd.Observacoes)
            .HasMaxLength(500);

        builder.Property(pd => pd.CargaHorariaSemanal)
            .IsRequired();

        builder.Property(pd => pd.DataAtribuicao)
            .IsRequired()
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(pd => pd.Ativa)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(pd => pd.UpdatedAt)
            .HasColumnType("datetime(6)");

        // Indexes
        builder.HasIndex(pd => pd.ProfessorId)
            .HasDatabaseName("IX_ProfessorDisciplinas_ProfessorId");

        builder.HasIndex(pd => pd.DisciplinaId)
            .HasDatabaseName("IX_ProfessorDisciplinas_DisciplinaId");

        builder.HasIndex(pd => new { pd.ProfessorId, pd.DisciplinaId })
            .IsUnique()
            .HasDatabaseName("IX_ProfessorDisciplinas_Professor_Disciplina");

        builder.HasIndex(pd => pd.Ativa)
            .HasDatabaseName("IX_ProfessorDisciplinas_Ativa");

        // Foreign Key para Disciplinas (módulo Acadêmico)
        builder.HasIndex(pd => pd.DisciplinaId)
            .HasDatabaseName("IX_ProfessorDisciplinas_DisciplinaId");

        // Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_ProfessorDisciplinas_CargaHoraria", 
            "CargaHorariaSemanal > 0 AND CargaHorariaSemanal <= 40"));
    }
}