using MediatR;

namespace SistemaGestaoEscolar.Professores.Aplicacao.Queries;

public record ObterDisciplinasDisponiveisQuery(Guid EscolaId) : IRequest<IEnumerable<DisciplinaDisponivelDto>>;

public record DisciplinaDisponivelDto(
    Guid Id,
    string Nome,
    string Codigo,
    int CargaHoraria,
    string Serie,
    bool Obrigatoria);