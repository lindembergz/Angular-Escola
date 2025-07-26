using SistemaGestaoEscolar.Professores.Dominio.Entidades;
using SistemaGestaoEscolar.Professores.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Professores.Infraestrutura.Mapeadores;

public static class ProfessorDisciplinaMapper
{
    public static ProfessorDisciplina ToDomain(ProfessorDisciplinaEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // Create using reflection to access private constructor
        var disciplina = (ProfessorDisciplina)Activator.CreateInstance(typeof(ProfessorDisciplina), true)!;
        
        // Set properties using reflection
        var idProperty = typeof(ProfessorDisciplina).GetProperty("Id");
        idProperty?.SetValue(disciplina, entity.Id);

        var professorIdProperty = typeof(ProfessorDisciplina).GetProperty("ProfessorId");
        professorIdProperty?.SetValue(disciplina, entity.ProfessorId);

        var disciplinaIdProperty = typeof(ProfessorDisciplina).GetProperty("DisciplinaId");
        disciplinaIdProperty?.SetValue(disciplina, entity.DisciplinaId);

        var observacoesProperty = typeof(ProfessorDisciplina).GetProperty("Observacoes");
        observacoesProperty?.SetValue(disciplina, entity.Observacoes);

        var cargaHorariaProperty = typeof(ProfessorDisciplina).GetProperty("CargaHorariaSemanal");
        cargaHorariaProperty?.SetValue(disciplina, entity.CargaHorariaSemanal);

        var dataAtribuicaoProperty = typeof(ProfessorDisciplina).GetProperty("DataAtribuicao");
        dataAtribuicaoProperty?.SetValue(disciplina, entity.DataAtribuicao);

        var ativaProperty = typeof(ProfessorDisciplina).GetProperty("Ativa");
        ativaProperty?.SetValue(disciplina, entity.Ativa);

        // Set base entity properties
        var createdAtProperty = typeof(ProfessorDisciplina).GetProperty("CreatedAt");
        createdAtProperty?.SetValue(disciplina, entity.DataAtribuicao);

        var updatedAtProperty = typeof(ProfessorDisciplina).GetProperty("UpdatedAt");
        updatedAtProperty?.SetValue(disciplina, entity.UpdatedAt);

        return disciplina;
    }

    public static ProfessorDisciplinaEntity ToEntity(ProfessorDisciplina disciplina)
    {
        if (disciplina == null)
            throw new ArgumentNullException(nameof(disciplina));

        return new ProfessorDisciplinaEntity
        {
            Id = disciplina.Id,
            ProfessorId = disciplina.ProfessorId,
            DisciplinaId = disciplina.DisciplinaId,
            Observacoes = disciplina.Observacoes,
            CargaHorariaSemanal = disciplina.CargaHorariaSemanal,
            DataAtribuicao = disciplina.DataAtribuicao,
            Ativa = disciplina.Ativa,
            UpdatedAt = disciplina.UpdatedAt
        };
    }

    public static void UpdateEntity(ProfessorDisciplinaEntity entity, ProfessorDisciplina disciplina)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        
        if (disciplina == null)
            throw new ArgumentNullException(nameof(disciplina));

        entity.Observacoes = disciplina.Observacoes;
        entity.CargaHorariaSemanal = disciplina.CargaHorariaSemanal;
        entity.Ativa = disciplina.Ativa;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}