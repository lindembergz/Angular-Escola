using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestaoEscolar.Professores.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Professores.Infraestrutura.Persistencia.Configuracoes;

public class ProfessorEntityConfiguration : IEntityTypeConfiguration<ProfessorEntity>
{
    public void Configure(EntityTypeBuilder<ProfessorEntity> builder)
    {
        builder.ToTable("Professores");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Cpf)
            .IsRequired()
            .HasMaxLength(11)
            .IsFixedLength();

        builder.Property(p => p.Registro)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Email)
            .HasMaxLength(100);

        builder.Property(p => p.Telefone)
            .HasMaxLength(20);

        builder.Property(p => p.DataNascimento)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(p => p.DataContratacao)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(p => p.EscolaId)
            .IsRequired();

        builder.Property(p => p.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.DataCadastro)
            .IsRequired()
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(p => p.UpdatedAt)
            .HasColumnType("datetime(6)");

        builder.Property(p => p.Observacoes)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(p => p.Cpf)
            .IsUnique()
            .HasDatabaseName("IX_Professores_Cpf");

        builder.HasIndex(p => p.Registro)
            .IsUnique()
            .HasDatabaseName("IX_Professores_Registro");

        builder.HasIndex(p => p.Email)
            .HasDatabaseName("IX_Professores_Email");

        builder.HasIndex(p => p.EscolaId)
            .HasDatabaseName("IX_Professores_EscolaId");

        builder.HasIndex(p => p.Ativo)
            .HasDatabaseName("IX_Professores_Ativo");

        builder.HasIndex(p => new { p.Nome, p.EscolaId })
            .HasDatabaseName("IX_Professores_Nome_EscolaId");

        // Relationships
        builder.HasMany(p => p.Titulos)
            .WithOne(t => t.Professor)
            .HasForeignKey(t => t.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Disciplinas)
            .WithOne(d => d.Professor)
            .HasForeignKey(d => d.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}