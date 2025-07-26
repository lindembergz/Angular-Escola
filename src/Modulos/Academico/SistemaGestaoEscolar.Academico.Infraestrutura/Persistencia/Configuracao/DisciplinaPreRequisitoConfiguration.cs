using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Configuracao;

public class DisciplinaPreRequisitoConfiguration : IEntityTypeConfiguration<DisciplinaPreRequisitoEntity>
{
    public void Configure(EntityTypeBuilder<DisciplinaPreRequisitoEntity> builder)
    {
        builder.ToTable("DisciplinaPreRequisitos");
        
        builder.HasKey(dpr => dpr.Id);
        
        builder.Property(dpr => dpr.Id)
            .IsRequired()
            .ValueGeneratedNever();
            
        builder.Property(dpr => dpr.DisciplinaId)
            .IsRequired()
            .HasColumnType("char(36)");
            
        builder.Property(dpr => dpr.PreRequisitoId)
            .IsRequired()
            .HasColumnType("char(36)");
            
        builder.Property(dpr => dpr.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
        
        // Índices
        builder.HasIndex(dpr => dpr.DisciplinaId)
            .HasDatabaseName("IX_DisciplinaPreRequisitos_DisciplinaId");
            
        builder.HasIndex(dpr => dpr.PreRequisitoId)
            .HasDatabaseName("IX_DisciplinaPreRequisitos_PreRequisitoId");
            
        builder.HasIndex(dpr => new { dpr.DisciplinaId, dpr.PreRequisitoId })
            .IsUnique()
            .HasDatabaseName("IX_DisciplinaPreRequisitos_DisciplinaId_PreRequisitoId_Unique");
    }
}