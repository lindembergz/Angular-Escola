using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestaoEscolar.Escolas.Dominio.Entidades;

namespace SistemaGestaoEscolar.Escolas.Infraestrutura.Mapeamentos;

public class RedeEscolarConfiguration : IEntityTypeConfiguration<RedeEscolar>
{
    public void Configure(EntityTypeBuilder<RedeEscolar> builder)
    {
        builder.ToTable("RedesEscolares");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .IsRequired()
            .ValueGeneratedNever();

        // Configuração do Nome (Value Object)
        builder.OwnsOne(r => r.Nome, nome =>
        {
            nome.Property(n => n.Valor)
                .HasColumnName("Nome")
                .HasMaxLength(200)
                .IsRequired();
        });

        // Configuração do CNPJ da Mantenedora (Value Object)
        builder.OwnsOne(r => r.CnpjMantenedora, cnpj =>
        {
            cnpj.Property(c => c.Numero)
                .HasColumnName("CnpjMantenedora")
                .HasMaxLength(14)
                .IsRequired();

            cnpj.HasIndex(c => c.Numero)
                .IsUnique()
                .HasDatabaseName("IX_RedesEscolares_CnpjMantenedora");
        });

        // Configuração do Endereço da Sede (Value Object)
        builder.OwnsOne(r => r.EnderecoSede, endereco =>
        {
            endereco.Property(end => end.Logradouro)
                .HasColumnName("LogradouroSede")
                .HasMaxLength(200)
                .IsRequired();

            endereco.Property(end => end.Numero)
                .HasColumnName("NumeroSede")
                .HasMaxLength(20)
                .IsRequired();

            endereco.Property(end => end.Complemento)
                .HasColumnName("ComplementoSede")
                .HasMaxLength(100);

            endereco.Property(end => end.Bairro)
                .HasColumnName("BairroSede")
                .HasMaxLength(100)
                .IsRequired();

            endereco.Property(end => end.Cidade)
                .HasColumnName("CidadeSede")
                .HasMaxLength(100)
                .IsRequired();

            endereco.Property(end => end.Estado)
                .HasColumnName("EstadoSede")
                .HasMaxLength(2)
                .IsRequired();

            endereco.Property(end => end.Cep)
                .HasColumnName("CepSede")
                .HasMaxLength(8)
                .IsRequired();
        });

        builder.Property(r => r.DataCriacao)
            .HasColumnName("DataCriacao")
            .IsRequired();

        builder.Property(r => r.Ativa)
            .HasColumnName("Ativa")
            .IsRequired();

        // Configuração do relacionamento com Escolas
        builder.HasMany(r => r.Escolas)
            .WithOne()
            .HasForeignKey(e => e.RedeEscolarId)
            .OnDelete(DeleteBehavior.SetNull);

        // Índices
        builder.HasIndex(r => r.DataCriacao)
            .HasDatabaseName("IX_RedesEscolares_DataCriacao");

        builder.HasIndex(r => r.Ativa)
            .HasDatabaseName("IX_RedesEscolares_Ativa");

        // Ignorar propriedades de domínio que não devem ser persistidas
        builder.Ignore(r => r.DomainEvents);
        builder.Ignore(r => r.TotalEscolas);
        builder.Ignore(r => r.EscolasAtivas);
        builder.Ignore(r => r.EscolasInativas);
    }
}