using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands.Disciplina;

public record CriarDisciplinaCommand(
    string Nome,
    string Codigo,
    int CargaHoraria,
    string Serie,
    bool Obrigatoria,
    Guid EscolaId,
    string? Descricao = null
) : IRequest<DisciplinaReadDto>;