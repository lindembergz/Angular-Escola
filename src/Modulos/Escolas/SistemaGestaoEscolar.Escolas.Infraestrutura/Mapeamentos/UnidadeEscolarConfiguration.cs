using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestaoEscolar.Escolas.Dominio.Entidades;

namespace SistemaGestaoEscolar.Escolas.Infraestrutura.Mapeamentos;

public class UnidadeEscolarConfiguration : IEntityTypeConfiguration<UnidadeEscolar>
{
    public void Configure(EntityTypeBuilder<UnidadeEscolar> builder)
    {
        builder.ToTable("UnidadesEscolares");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .IsRequired()
            .ValueGeneratedNever();

        // Configuração do Nome (Value Object)
        builder.OwnsOne(u => u.Nome, nome =>
        {
            nome.Property(n => n.Valor)
                .HasColumnName("Nome")
                .HasMaxLength(200)
                .IsRequired();
        });

        // Configuração do Endereço (Value Object)
        builder.OwnsOne(u => u.Endereco, endereco =>
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
        builder.OwnsOne(u => u.Tipo, tipo =>
        {
            tipo.Property(t => t.Valor)
                .HasColumnName("Tipo")
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.Property(u => u.CapacidadeMaximaAlunos)
            .HasColumnName("CapacidadeMaximaAlunos")
            .IsRequired();

        builder.Property(u => u.AlunosMatriculados)
            .HasColumnName("AlunosMatriculados")
            .IsRequired();

        builder.Property(u => u.DataCriacao)
            .HasColumnName("DataCriacao")
            .IsRequired();

        builder.Property(u => u.Ativa)
            .HasColumnName("Ativa")
            .IsRequired();

        // Propriedades específicas para ensino infantil
        builder.Property(u => u.IdadeMinima)
            .HasColumnName("IdadeMinima");

        builder.Property(u => u.IdadeMaxima)
            .HasColumnName("IdadeMaxima");

        builder.Property(u => u.TemBerçario)
            .HasColumnName("TemBercario")
            .IsRequired();

        builder.Property(u => u.TemPreEscola)
            .HasColumnName("TemPreEscola")
            .IsRequired();

        // Chave estrangeira para Escola
        builder.Property<Guid>("EscolaId")
            .HasColumnName("EscolaId")
            .IsRequired();

        // Índices
        builder.HasIndex("EscolaId")
            .HasDatabaseName("IX_UnidadesEscolares_EscolaId");

        builder.HasIndex(u => u.DataCriacao)
            .HasDatabaseName("IX_UnidadesEscolares_DataCriacao");

        builder.HasIndex(u => u.Ativa)
            .HasDatabaseName("IX_UnidadesEscolares_Ativa");

        // UnidadeEscolar herda de BaseEntity, não de AggregateRoot, então não tem DomainEvents
    }
}