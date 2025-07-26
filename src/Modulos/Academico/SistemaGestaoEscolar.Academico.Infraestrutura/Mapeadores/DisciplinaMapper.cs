using SistemaGestaoEscolar.Academico.Dominio.Entidades;
using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Mapeadores;

public static class DisciplinaMapper
{
    public static Disciplina ToDomain(DisciplinaEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var serie = Serie.Criar((TipoSerie)entity.TipoSerie, entity.AnoSerie);

        var disciplina = Disciplina.Criar(entity.Nome, entity.Codigo, entity.CargaHoraria,
                                         serie, entity.Obrigatoria, entity.EscolaId, entity.Descricao);

        // Set private fields using reflection
        SetPrivateProperty(disciplina, "Id", entity.Id);
        SetPrivateProperty(disciplina, "Ativa", entity.Ativa);
        SetPrivateProperty(disciplina, "CreatedAt", entity.CreatedAt);
        SetPrivateProperty(disciplina, "UpdatedAt", entity.UpdatedAt??DateTime.UtcNow);

        // Add pre-requisites
        foreach (var preRequisito in entity.PreRequisitos)
        {
            disciplina.AdicionarPreRequisito(preRequisito.PreRequisitoId);
        }

        return disciplina;
    }

    public static DisciplinaEntity ToEntity(Disciplina disciplina)
    {
        if (disciplina == null) throw new ArgumentNullException(nameof(disciplina));

        return new DisciplinaEntity
        {
            Id = disciplina.Id,
            Nome = disciplina.Nome,
            Codigo = disciplina.Codigo,
            CargaHoraria = disciplina.CargaHoraria,
            TipoSerie = (int)disciplina.Serie.Tipo,
            AnoSerie = disciplina.Serie.Ano,
            Obrigatoria = disciplina.Obrigatoria,
            Descricao = disciplina.Descricao,
            EscolaId = disciplina.EscolaId,
            Ativa = disciplina.Ativa,
            CreatedAt = disciplina.CreatedAt,
            UpdatedAt = disciplina.UpdatedAt
        };
    }

    public static void UpdateEntity(DisciplinaEntity entity, Disciplina disciplina)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        if (disciplina == null) throw new ArgumentNullException(nameof(disciplina));

        entity.Nome = disciplina.Nome;
        entity.Codigo = disciplina.Codigo;
        entity.CargaHoraria = disciplina.CargaHoraria;
        entity.TipoSerie = (int)disciplina.Serie.Tipo;
        entity.AnoSerie = disciplina.Serie.Ano;
        entity.Obrigatoria = disciplina.Obrigatoria;
        entity.Descricao = disciplina.Descricao;
        entity.EscolaId = disciplina.EscolaId;
        entity.Ativa = disciplina.Ativa;
        entity.UpdatedAt = disciplina.UpdatedAt;
    }

    private static void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName, 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        property?.SetValue(obj, value);
    }
}