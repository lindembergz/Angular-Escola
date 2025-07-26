using MediatR;
using SistemaGestaoEscolar.Professores.Aplicacao.Queries;
using SistemaGestaoEscolar.Professores.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Professores.Aplicacao.QueryHandlers;

public class ListarProfessoresQueryHandler : IRequestHandler<ListarProfessoresQuery, PaginatedResult<ProfessorResumoDto>>
{
    private readonly IRepositorioProfessor _repositorio;

    public ListarProfessoresQueryHandler(IRepositorioProfessor repositorio)
    {
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public async Task<PaginatedResult<ProfessorResumoDto>> Handle(ListarProfessoresQuery request, CancellationToken cancellationToken)
    {
        // Por simplicidade, vou implementar uma versão básica
        // Em um cenário real, isso seria implementado com paginação no repositório
        
        IEnumerable<SistemaGestaoEscolar.Professores.Dominio.Entidades.Professor> professores;
        
        if (request.EscolaId.HasValue)
        {
            professores = await _repositorio.ObterPorEscolaAsync(request.EscolaId.Value);
        }
        else if (request.Ativo.HasValue && request.Ativo.Value)
        {
            professores = await _repositorio.ObterAtivosAsync();
        }
        else
        {
            // Para este exemplo, vou usar ObterAtivosAsync como fallback
            professores = await _repositorio.ObterAtivosAsync();
        }

        // Filtrar por nome se fornecido
        if (!string.IsNullOrWhiteSpace(request.Nome))
        {
            professores = professores.Where(p => 
                p.Nome.Valor.Contains(request.Nome, StringComparison.OrdinalIgnoreCase));
        }

        // Filtrar por status ativo se especificado
        if (request.Ativo.HasValue)
        {
            professores = professores.Where(p => p.Ativo == request.Ativo.Value);
        }

        var totalItems = professores.Count();
        var items = professores
            .Skip((request.Pagina - 1) * request.TamanhoPagina)
            .Take(request.TamanhoPagina)
            .Select(p => new ProfessorResumoDto(
                p.Id,
                p.Nome.Valor,
                p.Cpf.NumeroFormatado,
                p.Registro.NumeroFormatado,
                p.Email,
                p.EscolaId,
                p.Ativo,
                p.ObterCargaHorariaTotal(),
                p.ObterMaiorTitulo()?.Descricao ?? "Não informado",
                p.Disciplinas.Count))
            .ToList();

        var totalPaginas = (int)Math.Ceiling((double)totalItems / request.TamanhoPagina);

        return new PaginatedResult<ProfessorResumoDto>(
            items,
            totalItems,
            request.Pagina,
            request.TamanhoPagina,
            totalPaginas);
    }
}