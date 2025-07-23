using SistemaGestaoEscolar.Alunos.Dominio.Entidades;
using SistemaGestaoEscolar.Alunos.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Alunos.Infraestrutura.Mapeadores;

public static class MatriculaMapper
{
    public static MatriculaEntity ToEntity(Matricula matricula)
    {
        return new MatriculaEntity
        {
            Id = matricula.Id,
            AlunoId = matricula.AlunoId,
            TurmaId = matricula.TurmaId,
            AnoLetivo = matricula.AnoLetivo,
            DataMatricula = matricula.DataMatricula,
            DataCancelamento = matricula.DataCancelamento,
            MotivoCancelamento = matricula.MotivoCancelamento,
            Ativa = matricula.Ativa,
            NumeroMatricula = matricula.NumeroMatricula,
            Status = (int)matricula.Status,
            Observacoes = matricula.Observacoes,
            CreatedAt = matricula.CreatedAt,
            UpdatedAt = matricula.UpdatedAt
        };
    }

    public static Matricula ToDomain(MatriculaEntity entity)
    {
        var matricula = new Matricula(
            entity.AlunoId,
            entity.TurmaId,
            entity.AnoLetivo,
            entity.Observacoes);

        // Definir propriedades que não são definidas no construtor
        SetPrivateProperty(matricula, "Id", entity.Id);
        SetPrivateProperty(matricula, "DataMatricula", entity.DataMatricula);
        SetPrivateProperty(matricula, "DataCancelamento", entity.DataCancelamento!);
        SetPrivateProperty(matricula, "MotivoCancelamento", entity.MotivoCancelamento!);
        SetPrivateProperty(matricula, "Ativa", entity.Ativa);
        SetPrivateProperty(matricula, "NumeroMatricula", entity.NumeroMatricula!);
        SetPrivateProperty(matricula, "Status", (StatusMatricula)entity.Status);
        SetPrivateProperty(matricula, "CreatedAt", entity.CreatedAt);
        SetPrivateProperty(matricula, "UpdatedAt", entity.UpdatedAt!);

        return matricula;
    }

    public static void UpdateEntity(MatriculaEntity entity, Matricula matricula)
    {
        entity.TurmaId = matricula.TurmaId;
        entity.AnoLetivo = matricula.AnoLetivo;
        entity.DataCancelamento = matricula.DataCancelamento;
        entity.MotivoCancelamento = matricula.MotivoCancelamento;
        entity.Ativa = matricula.Ativa;
        entity.Status = (int)matricula.Status;
        entity.Observacoes = matricula.Observacoes;
        entity.UpdatedAt = matricula.UpdatedAt;
    }

    private static void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName, 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Public | 
            System.Reflection.BindingFlags.Instance);
        
        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value);
        }
        else
        {
            // Tentar campo privado
            var field = obj.GetType().GetField($"<{propertyName}>k__BackingField", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            field?.SetValue(obj, value);
        }
    }
}