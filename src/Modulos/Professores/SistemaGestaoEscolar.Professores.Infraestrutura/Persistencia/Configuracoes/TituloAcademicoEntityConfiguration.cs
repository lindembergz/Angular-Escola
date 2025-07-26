using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestaoEscolar.Professores.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Professores.Infraestrutura.Persistencia.Configuracoes;

public class TituloAcademicoEntityConfiguration : IEntityTypeConfiguration<TituloAcademicoEntity>
{
    public void Configure(EntityTypeBuilder<TituloAcademicoEntity> builder)
    {
        builder.ToTable("TitulosAcademicos");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(t => t.ProfessorId)
            .IsRequired();

        builder.Property(t => t.Tipo)
            .IsRequired();

        builder.Property(t => t.Curso)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Instituicao)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.AnoFormatura)
            .IsRequired();

        builder.Property(t => t.DataCadastro)
            .IsRequired()
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        // Indexes
        builder.HasIndex(t => t.ProfessorId)
            .HasDatabaseName("IX_TitulosAcademicos_ProfessorId");

        builder.HasIndex(t => t.Tipo)
            .HasDatabaseName("IX_TitulosAcademicos_Tipo");

        builder.HasIndex(t => new { t.ProfessorId, t.Tipo, t.Curso })
            .IsUnique()
            .HasDatabaseName("IX_TitulosAcademicos_Professor_Tipo_Curso");

        // Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_TitulosAcademicos_AnoFormatura", 
            "AnoFormatura >= 1950 AND AnoFormatura <= YEAR(CURDATE())"));

        builder.ToTable(t => t.HasCheckConstraint("CK_TitulosAcademicos_Tipo", 
            "Tipo BETWEEN 1 AND 7"));
    }
}