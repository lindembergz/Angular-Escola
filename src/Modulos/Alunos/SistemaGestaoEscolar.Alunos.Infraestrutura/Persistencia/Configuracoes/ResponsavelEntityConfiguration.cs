using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestaoEscolar.Alunos.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Alunos.Infraestrutura.Persistencia.Configuracoes;

public class ResponsavelEntityConfiguration : IEntityTypeConfiguration<ResponsavelEntity>
{
    public void Configure(EntityTypeBuilder<ResponsavelEntity> builder)
    {
        builder.ToTable("Responsaveis");

        // Primary Key
        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Cpf)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(r => r.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Telefone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(r => r.Tipo)
            .IsRequired();

        builder.Property(r => r.Profissao)
            .HasMaxLength(100);

        builder.Property(r => r.LocalTrabalho)
            .HasMaxLength(200);

        builder.Property(r => r.TelefoneTrabalho)
            .HasMaxLength(20);

        builder.Property(r => r.ResponsavelFinanceiro)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.ResponsavelAcademico)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.AutorizadoBuscar)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.Observacoes)
            .HasMaxLength(1000);

        // Endereço do responsável (opcional)
        builder.Property(r => r.Logradouro)
            .HasMaxLength(200);

        builder.Property(r => r.Numero)
            .HasMaxLength(20);

        builder.Property(r => r.Complemento)
            .HasMaxLength(100);

        builder.Property(r => r.Bairro)
            .HasMaxLength(100);

        builder.Property(r => r.Cidade)
            .HasMaxLength(100);

        builder.Property(r => r.Estado)
            .HasMaxLength(2);

        builder.Property(r => r.Cep)
            .HasMaxLength(8);

        // Auditoria
        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(r => r.UpdatedAt)
            .HasColumnType("datetime(6)");

        // Foreign Key
        builder.Property(r => r.AlunoId)
            .IsRequired();

        // Indexes
        builder.HasIndex(r => r.Cpf)
            .HasDatabaseName("IX_Responsaveis_Cpf");

        builder.HasIndex(r => r.Email)
            .HasDatabaseName("IX_Responsaveis_Email");

        builder.HasIndex(r => r.AlunoId)
            .HasDatabaseName("IX_Responsaveis_AlunoId");

        // Relationship
        builder.HasOne(r => r.Aluno)
            .WithMany(a => a.Responsaveis)
            .HasForeignKey(r => r.AlunoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}