using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Configuracao;

public class DisciplinaConfiguration : IEntityTypeConfiguration<DisciplinaEntity>
{
    public void Configure(EntityTypeBuilder<DisciplinaEntity> builder)
    {
        builder.ToTable("Disciplinas");
        
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.Id)
            .IsRequired()
            .ValueGeneratedNever();
            
        builder.Property(d => d.Nome)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("varchar(100)");
            
        builder.Property(d => d.Codigo)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnType("varchar(20)");
            
        builder.Property(d => d.CargaHoraria)
            .IsRequired()
            .HasColumnType("int");
            
        builder.Property(d => d.TipoSerie)
            .IsRequired()
            .HasColumnType("int");
            
        builder.Property(d => d.AnoSerie)
            .IsRequired()
            .HasColumnType("int");
            
        builder.Property(d => d.Obrigatoria)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnType("tinyint(1)");
            
        builder.Property(d => d.Descricao)
            .HasMaxLength(500)
            .HasColumnType("varchar(500)");
            
        builder.Property(d => d.EscolaId)
            .IsRequired()
            .HasColumnType("char(36)");
            
        builder.Property(d => d.Ativa)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnType("tinyint(1)");
            
        builder.Property(d => d.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            
        builder.Property(d => d.UpdatedAt)
            .HasColumnType("datetime(6)");
        
        // Relacionamentos
        builder.HasMany(d => d.Horarios)
            .WithOne(h => h.Disciplina)
            .HasForeignKey(h => h.DisciplinaId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(d => d.PreRequisitos)
            .WithOne(pr => pr.Disciplina)
            .HasForeignKey(pr => pr.DisciplinaId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(d => d.DisciplinasQueRequerem)
            .WithOne(pr => pr.PreRequisito)
            .HasForeignKey(pr => pr.PreRequisitoId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Índices
        builder.HasIndex(d => d.Codigo)
            .IsUnique()
            .HasDatabaseName("IX_Disciplinas_Codigo_Unique");
            
        builder.HasIndex(d => d.Nome)
            .HasDatabaseName("IX_Disciplinas_Nome");
            
        builder.HasIndex(d => d.EscolaId)
            .HasDatabaseName("IX_Disciplinas_EscolaId");
            
        builder.HasIndex(d => new { d.TipoSerie, d.AnoSerie, d.EscolaId })
            .HasDatabaseName("IX_Disciplinas_Serie_EscolaId");
            
        builder.HasIndex(d => new { d.EscolaId, d.Codigo })
            .IsUnique()
            .HasDatabaseName("IX_Disciplinas_EscolaId_Codigo_Unique");
    }
}