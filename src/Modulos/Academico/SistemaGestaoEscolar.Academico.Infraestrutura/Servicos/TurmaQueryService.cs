using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;
using SistemaGestaoEscolar.Academico.Aplicacao.Interfaces;
using SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Servicos;

public class TurmaQueryService : ITurmaQueryService
{
    private readonly DbContext _context;

    public TurmaQueryService(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<(IEnumerable<TurmaResumoReadDto> turmas, int totalItems)> ObterTurmasComFiltrosAsync(
        Guid? escolaId = null,
        int? anoLetivo = null,
        string? serie = null,
        string? turno = null,
        bool? ativa = null,
        int page = 1,
        int pageSize = 10)
    {
        var query = _context.Set<TurmaEntity>().AsQueryable();

        // Aplicar filtros
        if (escolaId.HasValue)
            query = query.Where(t => t.EscolaId == escolaId.Value);

        if (anoLetivo.HasValue)
            query = query.Where(t => t.AnoLetivo == anoLetivo.Value);

        if (!string.IsNullOrEmpty(serie))
        {
            // Filtrar por série - precisa converter a string de volta para os valores do enum
            query = query.Where(t => ObterDescricaoSerie(t.TipoSerie, t.AnoSerie).Contains(serie));
        }

        if (!string.IsNullOrEmpty(turno))
        {
            // Filtrar por turno
            var tipoTurno = turno.ToLower() switch
            {
                "matutino" => 1,
                "vespertino" => 2,
                "noturno" => 3,
                "integral" => 4,
                _ => -1
            };
            if (tipoTurno > 0)
                query = query.Where(t => t.TipoTurno == tipoTurno);
        }

        if (ativa.HasValue)
            query = query.Where(t => t.Ativa == ativa.Value);

        var totalItems = await query.CountAsync();

        var turmas = await query
            .OrderBy(t => t.TipoSerie)
            .ThenBy(t => t.AnoSerie)
            .ThenBy(t => t.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TurmaResumoReadDto
            {
                Id = t.Id.ToString(),
                Nome = t.Nome,
                Serie = ObterDescricaoSerie(t.TipoSerie, t.AnoSerie),
                Turno = ObterDescricaoTurno(t.TipoTurno),
                AnoLetivo = t.AnoLetivo,
                CapacidadeMaxima = t.CapacidadeMaxima,
                AlunosMatriculados = t.TurmaAlunos.Count(ta => ta.Ativa),
                Ativa = t.Ativa
            })
            .ToListAsync();

        return (turmas, totalItems);
    }

    public async Task<TurmaReadDto?> ObterTurmaDetalheAsync(Guid turmaId)
    {
        return await _context.Set<TurmaEntity>()
            .Where(t => t.Id == turmaId)
            .Select(t => new TurmaReadDto(
                t.Id,
                t.Nome,
                ObterDescricaoSerie(t.TipoSerie, t.AnoSerie),
                ObterDescricaoTurno(t.TipoTurno),
                t.CapacidadeMaxima,
                t.AnoLetivo,
                t.EscolaId,
                t.Ativa,
                t.TurmaAlunos.Count(ta => ta.Ativa),
                t.CapacidadeMaxima - t.TurmaAlunos.Count(ta => ta.Ativa),
                t.CreatedAt
            ))
            .FirstOrDefaultAsync();
    }

    private static string ObterDescricaoSerie(int tipoSerie, int anoSerie)
    {
        return tipoSerie switch
        {
            1 => $"Infantil {anoSerie}",
            2 => $"{anoSerie}º Ano Fundamental",
            3 => $"{anoSerie}º Ano Médio",
            _ => "Série Desconhecida"
        };
    }

    private static string ObterDescricaoTurno(int tipoTurno)
    {
        return tipoTurno switch
        {
            1 => "Matutino",
            2 => "Vespertino",
            3 => "Noturno",
            4 => "Integral",
            _ => "Turno Desconhecido"
        };
    }
}