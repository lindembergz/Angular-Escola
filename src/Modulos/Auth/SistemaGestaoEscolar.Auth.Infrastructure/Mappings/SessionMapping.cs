using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestaoEscolar.Auth.Domain.Entities;

namespace SistemaGestaoEscolar.Auth.Infrastructure.Mappings;

/// <summary>
/// Configuração de mapeamento para a entidade Session.
/// Define como a entidade de domínio é mapeada para o banco de dados.
/// </summary>
public class SessionMapping : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        // Configuração da tabela
        builder.ToTable("Sessoes");
        
        // Chave primária
        builder.HasKey(s => s.Id);
        
        // Propriedades básicas
        builder.Property(s => s.Id)
            .IsRequired()
            .HasColumnType("char(36)");
            
        builder.Property(s => s.UsuarioId)
            .IsRequired()
            .HasColumnType("char(36)");
            
        builder.Property(s => s.EnderecoIp)
            .IsRequired()
            .HasMaxLength(45) // IPv6 máximo
            .HasColumnType("varchar(45)");
            
        builder.Property(s => s.AgenteUsuario)
            .IsRequired()
            .HasMaxLength(1000)
            .HasColumnType("varchar(1000)");
            
        builder.Property(s => s.IniciadaEm)
            .IsRequired()
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            
        builder.Property(s => s.UltimaAtividadeEm)
            .IsRequired()
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            
        builder.Property(s => s.FinalizadaEm)
            .HasColumnType("datetime(6)");
            
        builder.Property(s => s.Ativa)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(s => s.InfoDispositivo)
            .HasMaxLength(500)
            .HasColumnType("varchar(500)");
            
        builder.Property(s => s.Localizacao)
            .HasMaxLength(200)
            .HasColumnType("varchar(200)");

        // Timestamps herdados de BaseEntity
        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            
        builder.Property(s => s.UpdatedAt)
            .HasColumnType("datetime(6)");

        // Índices para performance
        builder.HasIndex(s => s.UsuarioId)
            .HasDatabaseName("IX_Sessoes_UsuarioId");
            
        builder.HasIndex(s => s.Ativa)
            .HasDatabaseName("IX_Sessoes_Ativa");
            
        builder.HasIndex(s => s.IniciadaEm)
            .HasDatabaseName("IX_Sessoes_IniciadaEm");
            
        builder.HasIndex(s => s.UltimaAtividadeEm)
            .HasDatabaseName("IX_Sessoes_UltimaAtividadeEm");
            
        builder.HasIndex(s => s.EnderecoIp)
            .HasDatabaseName("IX_Sessoes_EnderecoIp");

        // Índice composto para consultas comuns
        builder.HasIndex(s => new { s.UsuarioId, s.Ativa })
            .HasDatabaseName("IX_Sessoes_UsuarioId_Ativa");
            
        builder.HasIndex(s => new { s.Ativa, s.UltimaAtividadeEm })
            .HasDatabaseName("IX_Sessoes_Ativa_UltimaAtividade");

        // Ignorar propriedades calculadas
        builder.Ignore(s => s.Duracao);
        builder.Ignore(s => s.TempoDesdeUltimaAtividade);
    }
}