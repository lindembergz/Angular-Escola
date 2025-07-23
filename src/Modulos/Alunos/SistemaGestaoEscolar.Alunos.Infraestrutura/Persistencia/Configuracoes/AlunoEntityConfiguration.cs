using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestaoEscolar.Alunos.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Alunos.Infraestrutura.Persistencia.Configuracoes;

public class AlunoEntityConfiguration : IEntityTypeConfiguration<AlunoEntity>
{
    public void Configure(EntityTypeBuilder<AlunoEntity> builder)
    {
        builder.ToTable("Alunos");

        // Primary Key
        builder.HasKey(a => a.Id);

        // Properties
        builder.Property(a => a.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Cpf)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(a => a.DataNascimento)
            .IsRequired()
            .HasColumnType("date");

        // Endereço
        builder.Property(a => a.Logradouro)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Numero)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.Complemento)
            .HasMaxLength(100);

        builder.Property(a => a.Bairro)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Cidade)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Estado)
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(a => a.Cep)
            .IsRequired()
            .HasMaxLength(8);

        // Gênero e Deficiência
        builder.Property(a => a.Genero)
            .IsRequired()
            .HasDefaultValue(0); // NaoInformado

        builder.Property(a => a.TipoDeficiencia)
            .IsRequired(false);

        builder.Property(a => a.DescricaoDeficiencia)
            .HasMaxLength(500);

        // Contato
        builder.Property(a => a.Telefone)
            .HasMaxLength(20);

        builder.Property(a => a.Email)
            .HasMaxLength(200);

        builder.Property(a => a.Observacoes)
            .HasMaxLength(1000);

        // Escola
        builder.Property(a => a.EscolaId)
            .IsRequired();

        // Auditoria
        builder.Property(a => a.DataCadastro)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(a => a.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(a => a.UpdatedAt)
            .HasColumnType("datetime(6)");

        // Indexes
        builder.HasIndex(a => a.Cpf)
            .IsUnique()
            .HasDatabaseName("IX_Alunos_Cpf");

        builder.HasIndex(a => a.Email)
            .HasDatabaseName("IX_Alunos_Email");

        builder.HasIndex(a => a.EscolaId)
            .HasDatabaseName("IX_Alunos_EscolaId");

        builder.HasIndex(a => a.Nome)
            .HasDatabaseName("IX_Alunos_Nome");

        builder.HasIndex(a => a.DataNascimento)
            .HasDatabaseName("IX_Alunos_DataNascimento");

        // Relationships
        builder.HasMany(a => a.Responsaveis)
            .WithOne(r => r.Aluno)
            .HasForeignKey(r => r.AlunoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Matriculas)
            .WithOne(m => m.Aluno)
            .HasForeignKey(m => m.AlunoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}