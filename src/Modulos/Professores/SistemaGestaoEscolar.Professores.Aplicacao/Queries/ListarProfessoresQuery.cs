using MediatR;

namespace SistemaGestaoEscolar.Professores.Aplicacao.Queries;

public record ListarProfessoresQuery(
    Guid? EscolaId = null,
    bool? Ativo = null,
    string? Nome = null,
    int Pagina = 1,
    int TamanhoPagina = 10) : IRequest<PaginatedResult<ProfessorResumoDto>>;

public record ProfessorResumoDto(
    Guid Id,
    string Nome,
    string Cpf,
    string Registro,
    string? Email,
    Guid EscolaId,
    bool Ativo,
    int CargaHorariaTotal,
    string MaiorTitulo,
    int QuantidadeDisciplinas);

public record PaginatedResult<T>(
    List<T> Items,
    int TotalItems,
    int Pagina,
    int TamanhoPagina,
    int TotalPaginas);