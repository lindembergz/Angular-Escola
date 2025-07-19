using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestaoEscolar.Auth.Domain.Entities;
using SistemaGestaoEscolar.Auth.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Auth.Infrastructure.Mappings;

/// <summary>
/// Configuração de mapeamento para a entidade User.
/// Define como a entidade de domínio é mapeada para o banco de dados.
/// </summary>
public class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Configuração da tabela
        builder.ToTable("Usuarios");
        
        // Chave primária
        builder.HasKey(u => u.Id);
        
        // Propriedades básicas
        builder.Property(u => u.Id)
            .IsRequired()
            .HasColumnType("char(36)");
            
        builder.Property(u => u.PrimeiroNome)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("varchar(100)");
            
        builder.Property(u => u.UltimoNome)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("varchar(100)");
            
        builder.Property(u => u.Ativo)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(u => u.EmailConfirmado)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(u => u.UltimoLoginEm)
            .HasColumnType("datetime(6)");
            
        builder.Property(u => u.UltimaMudancaSenhaEm)
            .HasColumnType("datetime(6)");
            
        builder.Property(u => u.TentativasLoginFalhadas)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(u => u.BloqueadoAte)
            .HasColumnType("datetime(6)");
            
        builder.Property(u => u.RefreshToken)
            .HasMaxLength(500)
            .HasColumnType("varchar(500)");
            
        builder.Property(u => u.RefreshTokenExpiraEm)
            .HasColumnType("datetime(6)");
            
        builder.Property(u => u.EscolaId)
            .HasColumnType("char(36)");

        // Timestamps herdados de BaseEntity
        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            
        builder.Property(u => u.UpdatedAt)
            .HasColumnType("datetime(6)");

        // Configuração do Value Object Email
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(254)
                .HasColumnName("Email")
                .HasColumnType("varchar(254)");
                
            // Índice único para email
            email.HasIndex(e => e.Value)
                .IsUnique()
                .HasDatabaseName("IX_Usuarios_Email");
        });

        // Configuração do Value Object Password
        builder.OwnsOne(u => u.Senha, password =>
        {
            password.Property(p => p.HashedValue)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnName("SenhaHash")
                .HasColumnType("varchar(500)");
        });

        // Configuração do Value Object UserRole
        builder.OwnsOne(u => u.Perfil, role =>
        {
            role.Property(r => r.Code)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("CodigoPerfil")
                .HasColumnType("varchar(50)");
                
            role.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("NomePerfil")
                .HasColumnType("varchar(100)");
                
            role.Property(r => r.Level)
                .IsRequired()
                .HasColumnName("NivelPerfil");
                
            // Índice para role code
            role.HasIndex(r => r.Code)
                .HasDatabaseName("IX_Usuarios_CodigoPerfil");
        });

        // Relacionamento com Sessions
        builder.HasMany(u => u.Sessions)
            .WithOne()
            .HasForeignKey(s => s.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices adicionais
        builder.HasIndex(u => u.EscolaId)
            .HasDatabaseName("IX_Usuarios_EscolaId");
            
        builder.HasIndex(u => u.Ativo)
            .HasDatabaseName("IX_Usuarios_Ativo");
            
        builder.HasIndex(u => u.UltimoLoginEm)
            .HasDatabaseName("IX_Usuarios_UltimoLoginEm");
            
        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("IX_Usuarios_CreatedAt");

        // Configuração de nome completo computado (para busca)
        builder.HasIndex(u => new { u.PrimeiroNome, u.UltimoNome })
            .HasDatabaseName("IX_Usuarios_NomeCompleto");

        // Ignorar propriedades calculadas
        builder.Ignore(u => u.NomeCompleto);
        builder.Ignore(u => u.Iniciais);
        
        // Ignorar eventos de domínio (são tratados pela classe base)
        builder.Ignore(u => u.DomainEvents);
    }
}