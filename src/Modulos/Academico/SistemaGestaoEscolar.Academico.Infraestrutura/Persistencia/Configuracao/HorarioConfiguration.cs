using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Configuracao;

public class HorarioConfiguration : IEntityTypeConfiguration<HorarioEntity>
{
    public void Configure(EntityTypeBuilder<HorarioEntity> builder)
    {
        builder.ToTable("Horarios");
        
        builder.HasKey(h => h.Id);
        
        builder.Property(h => h.Id)
            .IsRequired()
            .ValueGeneratedNever();
            
        builder.Property(h => h.TurmaId)
            .IsRequired()
            .HasColumnType("char(36)");
            
        builder.Property(h => h.DisciplinaId)
            .IsRequired()
            .HasColumnType("char(36)");
            
        builder.Property(h => h.ProfessorId)
            .IsRequired()
            .HasColumnType("char(36)");
            
        builder.Property(h => h.DiaSemana)
            .IsRequired()
            .HasColumnType("int");
            
        builder.Property(h => h.HoraInicio)
            .IsRequired()
            .HasColumnType("time");
            
        builder.Property(h => h.HoraFim)
            .IsRequired()
            .HasColumnType("time");
            
        builder.Property(h => h.Sala)
            .HasMaxLength(50)
            .HasColumnType("varchar(50)");
            
        builder.Property(h => h.AnoLetivo)
            .IsRequired()
            .HasColumnType("int");
            
        builder.Property(h => h.Semestre)
            .IsRequired()
            .HasColumnType("int");
            
        builder.Property(h => h.Ativo)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnType("tinyint(1)");
            
        builder.Property(h => h.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            
        builder.Property(h => h.UpdatedAt)
            .HasColumnType("datetime(6)");
        
        // Relacionamentos já definidos nas outras configurações
        
        // Índices
        builder.HasIndex(h => h.TurmaId)
            .HasDatabaseName("IX_Horarios_TurmaId");
            
        builder.HasIndex(h => h.DisciplinaId)
            .HasDatabaseName("IX_Horarios_DisciplinaId");
            
        builder.HasIndex(h => h.ProfessorId)
            .HasDatabaseName("IX_Horarios_ProfessorId");
            
        builder.HasIndex(h => new { h.AnoLetivo, h.Semestre })
            .HasDatabaseName("IX_Horarios_AnoLetivo_Semestre");
            
        builder.HasIndex(h => new { h.DiaSemana, h.HoraInicio, h.HoraFim })
            .HasDatabaseName("IX_Horarios_DiaSemana_Horario");
            
        // Índice para detectar conflitos de horário
        builder.HasIndex(h => new { h.ProfessorId, h.DiaSemana, h.HoraInicio, h.HoraFim, h.AnoLetivo, h.Semestre })
            .HasDatabaseName("IX_Horarios_Conflito_Professor");
            
        builder.HasIndex(h => new { h.Sala, h.DiaSemana, h.HoraInicio, h.HoraFim, h.AnoLetivo, h.Semestre })
            .HasDatabaseName("IX_Horarios_Conflito_Sala")
            .HasFilter("Sala IS NOT NULL");
    }
}