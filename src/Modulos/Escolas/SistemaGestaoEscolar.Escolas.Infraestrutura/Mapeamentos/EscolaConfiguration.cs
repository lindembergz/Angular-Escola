using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestaoEscolar.Escolas.Dominio.Entidades;
using SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;

namespace SistemaGestaoEscolar.Escolas.Infraestrutura.Mapeamentos;

public class EscolaConfiguration : IEntityTypeConfiguration<Escola>
{
    public void Configure(EntityTypeBuilder<Escola> builder)
    {
        builder.ToTable("Escolas");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired()
            .ValueGeneratedNever();

        // Configuração do Nome (Value Object)
        builder.OwnsOne(e => e.Nome, nome =>
        {
            nome.Property(n => n.Valor)
                .HasColumnName("Nome")
                .HasMaxLength(200)
                .IsRequired();
        });

        // Configuração do CNPJ (Value Object)
        builder.OwnsOne(e => e.Cnpj, cnpj =>
        {
            cnpj.Property(c => c.Numero)
                .HasColumnName("Cnpj")
                .HasMaxLength(14)
                .IsRequired();

            cnpj.HasIndex(c => c.Numero)
                .IsUnique()
                .HasDatabaseName("IX_Escolas_Cnpj");
        });

        // Configuração do Endereço (Value Object)
        builder.OwnsOne(e => e.Endereco, endereco =>
        {
            endereco.Property(end => end.Logradouro)
                .HasColumnName("Logradouro")
                .HasMaxLength(200)
                .IsRequired();

            endereco.Property(end => end.Numero)
                .HasColumnName("Numero")
                .HasMaxLength(20)
                .IsRequired();

            endereco.Property(end => end.Complemento)
                .HasColumnName("Complemento")
                .HasMaxLength(100);

            endereco.Property(end => end.Bairro)
                .HasColumnName("Bairro")
                .HasMaxLength(100)
                .IsRequired();

            endereco.Property(end => end.Cidade)
                .HasColumnName("Cidade")
                .HasMaxLength(100)
                .IsRequired();

            endereco.Property(end => end.Estado)
                .HasColumnName("Estado")
                .HasMaxLength(2)
                .IsRequired();

            endereco.Property(end => end.Cep)
                .HasColumnName("Cep")
                .HasMaxLength(8)
                .IsRequired();
        });

        // Configuração do Tipo (Value Object)
        builder.OwnsOne(e => e.Tipo, tipo =>
        {
            tipo.Property(t => t.Valor)
                .HasColumnName("Tipo")
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.Property(e => e.RedeEscolarId)
            .HasColumnName("RedeEscolarId");

        builder.Property(e => e.DataCriacao)
            .HasColumnName("DataCriacao")
            .IsRequired();

        builder.Property(e => e.Ativa)
            .HasColumnName("Ativa")
            .IsRequired();

        // Configuração do relacionamento com RedeEscolar
        builder.HasOne<RedeEscolar>()
            .WithMany()
            .HasForeignKey(e => e.RedeEscolarId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configuração do relacionamento com UnidadeEscolar
        builder.HasMany(e => e.Unidades)
            .WithOne()
            .HasForeignKey("EscolaId")
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(e => e.DataCriacao)
            .HasDatabaseName("IX_Escolas_DataCriacao");

        builder.HasIndex(e => e.Ativa)
            .HasDatabaseName("IX_Escolas_Ativa");

        builder.HasIndex(e => e.RedeEscolarId)
            .HasDatabaseName("IX_Escolas_RedeEscolarId");

        // Ignorar propriedades de domínio que não devem ser persistidas
        builder.Ignore(e => e.DomainEvents);
    }
}