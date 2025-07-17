using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.Escolas.Dominio.Entidades;
using SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Escolas.Dominio.Repositorios;
using SistemaGestaoEscolar.Escolas.Infraestrutura.Contexto;

namespace SistemaGestaoEscolar.Escolas.Infraestrutura.Repositorios;

public class RepositorioRedeEscolarMySql : IRepositorioRedeEscolar
{
    private readonly EscolasDbContext _context;

    public RepositorioRedeEscolarMySql(EscolasDbContext context)
    {
        _context = context;
    }

    public async Task<RedeEscolar?> ObterPorIdAsync(Guid id)
    {
        return await _context.RedesEscolares
            .Include(r => r.Escolas)
                .ThenInclude(e => e.Unidades)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<RedeEscolar?> ObterPorCnpjAsync(CNPJ cnpjMantenedora)
    {
        return await _context.RedesEscolares
            .Include(r => r.Escolas)
                .ThenInclude(e => e.Unidades)
            .FirstOrDefaultAsync(r => r.CnpjMantenedora == cnpjMantenedora);
    }

    public async Task<IEnumerable<RedeEscolar>> ObterTodasAsync()
    {
        return await _context.RedesEscolares
            .Include(r => r.Escolas)
                .ThenInclude(e => e.Unidades)
            .ToListAsync();
    }

    public async Task<IEnumerable<RedeEscolar>> ObterAtivasAsync()
    {
        return await _context.RedesEscolares
            .Include(r => r.Escolas)
                .ThenInclude(e => e.Unidades)
            .Where(r => r.Ativa)
            .ToListAsync();
    }

    public async Task<IEnumerable<RedeEscolar>> ObterInativasAsync()
    {
        return await _context.RedesEscolares
            .Include(r => r.Escolas)
                .ThenInclude(e => e.Unidades)
            .Where(r => !r.Ativa)
            .ToListAsync();
    }

    public async Task<IEnumerable<RedeEscolar>> PesquisarPorNomeAsync(string nome)
    {
        return await _context.RedesEscolares
            .Include(r => r.Escolas)
                .ThenInclude(e => e.Unidades)
            .Where(r => EF.Functions.Like(r.Nome.Valor, $"%{nome}%"))
            .ToListAsync();
    }

    public async Task<IEnumerable<RedeEscolar>> ObterPorCidadeSedeAsync(string cidade)
    {
        return await _context.RedesEscolares
            .Include(r => r.Escolas)
                .ThenInclude(e => e.Unidades)
            .Where(r => r.EnderecoSede.Cidade == cidade)
            .ToListAsync();
    }

    public async Task<IEnumerable<RedeEscolar>> ObterPorEstadoSedeAsync(string estado)
    {
        return await _context.RedesEscolares
            .Include(r => r.Escolas)
                .ThenInclude(e => e.Unidades)
            .Where(r => r.EnderecoSede.Estado == estado)
            .ToListAsync();
    }

    public async Task<bool> ExisteCnpjAsync(CNPJ cnpjMantenedora)
    {
        return await _context.RedesEscolares
            .AnyAsync(r => r.CnpjMantenedora == cnpjMantenedora);
    }

    public async Task<bool> ExisteNomeAsync(NomeEscola nome, Guid? excluirId = null)
    {
        var query = _context.RedesEscolares.Where(r => r.Nome == nome);
        
        if (excluirId.HasValue)
            query = query.Where(r => r.Id != excluirId.Value);
            
        return await query.AnyAsync();
    }

    public async Task<int> ContarRedesAtivasAsync()
    {
        return await _context.RedesEscolares
            .CountAsync(r => r.Ativa);
    }

    public async Task<int> ContarTotalEscolasAsync()
    {
        return await _context.Escolas.CountAsync();
    }

    public async Task AdicionarAsync(RedeEscolar redeEscolar)
    {
        await _context.RedesEscolares.AddAsync(redeEscolar);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(RedeEscolar redeEscolar)
    {
        _context.RedesEscolares.Update(redeEscolar);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(RedeEscolar redeEscolar)
    {
        _context.RedesEscolares.Remove(redeEscolar);
        await _context.SaveChangesAsync();
    }

    public async Task<RedeEscolar?> ObterComEscolasPorIdAsync(Guid id)
    {
        return await _context.RedesEscolares
            .Include(r => r.Escolas)
                .ThenInclude(e => e.Unidades)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<RedeEscolar>> ObterComEscolasAsync()
    {
        return await _context.RedesEscolares
            .Include(r => r.Escolas)
                .ThenInclude(e => e.Unidades)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> ObterEstatisticasEscolasPorRedeAsync()
    {
        return await _context.RedesEscolares
            .Include(r => r.Escolas)
            .Select(r => new { Nome = r.Nome.Valor, Count = r.Escolas.Count })
            .ToDictionaryAsync(x => x.Nome, x => x.Count);
    }

    public async Task<Dictionary<string, int>> ObterEstatisticasPorEstadoSedeAsync()
    {
        return await _context.RedesEscolares
            .GroupBy(r => r.EnderecoSede.Estado)
            .Select(g => new { Estado = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Estado, x => x.Count);
    }

    public async Task<IEnumerable<RedeEscolar>> ObterRedesComMaisEscolasAsync(int limite = 10)
    {
        return await _context.RedesEscolares
            .Include(r => r.Escolas)
                .ThenInclude(e => e.Unidades)
            .OrderByDescending(r => r.Escolas.Count)
            .Take(limite)
            .ToListAsync();
    }

    public async Task<IEnumerable<RedeEscolar>> ObterRedesComMenosEscolasAsync(int limite = 10)
    {
        return await _context.RedesEscolares
            .Include(r => r.Escolas)
                .ThenInclude(e => e.Unidades)
            .OrderBy(r => r.Escolas.Count)
            .Take(limite)
            .ToListAsync();
    }

    public async Task<int> ObterTotalAlunosRedeAsync(Guid redeId)
    {
        var rede = await _context.RedesEscolares
            .Include(r => r.Escolas)
                .ThenInclude(e => e.Unidades)
            .FirstOrDefaultAsync(r => r.Id == redeId);

        return rede?.Escolas
            .SelectMany(e => e.Unidades)
            .Sum(u => u.AlunosMatriculados) ?? 0;
    }

    public async Task<int> ObterTotalProfessoresRedeAsync(Guid redeId)
    {
        // Por enquanto retorna 0, pois não temos ainda o módulo de professores implementado
        // TODO: Implementar quando o módulo de professores estiver pronto
        await Task.CompletedTask;
        return 0;
    }

    public async Task<IEnumerable<RedeEscolar>> BuscarAvancadaAsync(
        string? nome = null,
        string? cidadeSede = null,
        string? estadoSede = null,
        bool? ativa = null,
        int? minimoEscolas = null,
        int? maximoEscolas = null)
    {
        var query = _context.RedesEscolares
            .Include(r => r.Escolas)
                .ThenInclude(e => e.Unidades)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(r => EF.Functions.Like(r.Nome.Valor, $"%{nome}%"));

        if (!string.IsNullOrWhiteSpace(cidadeSede))
            query = query.Where(r => r.EnderecoSede.Cidade == cidadeSede);

        if (!string.IsNullOrWhiteSpace(estadoSede))
            query = query.Where(r => r.EnderecoSede.Estado == estadoSede);

        if (ativa.HasValue)
            query = query.Where(r => r.Ativa == ativa.Value);

        if (minimoEscolas.HasValue)
            query = query.Where(r => r.Escolas.Count >= minimoEscolas.Value);

        if (maximoEscolas.HasValue)
            query = query.Where(r => r.Escolas.Count <= maximoEscolas.Value);

        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<RedeEscolar> Redes, int Total)> ObterPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        string? filtroNome = null,
        string? filtroCidade = null,
        bool? filtroAtiva = null)
    {
        var query = _context.RedesEscolares
            .Include(r => r.Escolas)
                .ThenInclude(e => e.Unidades)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtroNome))
            query = query.Where(r => EF.Functions.Like(r.Nome.Valor, $"%{filtroNome}%"));

        if (!string.IsNullOrWhiteSpace(filtroCidade))
            query = query.Where(r => r.EnderecoSede.Cidade == filtroCidade);

        if (filtroAtiva.HasValue)
            query = query.Where(r => r.Ativa == filtroAtiva.Value);

        var total = await query.CountAsync();

        var redes = await query
            .OrderBy(r => r.Nome.Valor)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (redes, total);
    }

    public async Task<bool> PodeAdicionarEscolaAsync(Guid redeId, CNPJ cnpjEscola)
    {
        var redeExiste = await _context.RedesEscolares.AnyAsync(r => r.Id == redeId && r.Ativa);
        var escolaExiste = await _context.Escolas.AnyAsync(e => e.Cnpj == cnpjEscola);
        var escolaJaTemRede = await _context.Escolas.AnyAsync(e => e.Cnpj == cnpjEscola && e.RedeEscolarId != null);

        return redeExiste && escolaExiste && !escolaJaTemRede;
    }

    public async Task<bool> PodeRemoverEscolaAsync(Guid redeId, Guid escolaId)
    {
        return await _context.Escolas
            .AnyAsync(e => e.Id == escolaId && e.RedeEscolarId == redeId);
    }

    public async Task<IEnumerable<Escola>> ObterEscolasDisponiveis()
    {
        return await _context.Escolas
            .Include(e => e.Unidades)
            .Where(e => e.RedeEscolarId == null && e.Ativa)
            .ToListAsync();
    }

    public async Task<RedeEscolar?> ObterRedeDaEscolaAsync(Guid escolaId)
    {
        var escola = await _context.Escolas
            .FirstOrDefaultAsync(e => e.Id == escolaId);

        if (escola?.RedeEscolarId == null)
            return null;

        return await _context.RedesEscolares
            .Include(r => r.Escolas)
                .ThenInclude(e => e.Unidades)
            .FirstOrDefaultAsync(r => r.Id == escola.RedeEscolarId);
    }

    public async Task<Dictionary<TipoEscola, int>> ObterDistribuicaoTiposEscolasAsync(Guid redeId)
    {
        return await _context.Escolas
            .Where(e => e.RedeEscolarId == redeId)
            .GroupBy(e => e.Tipo)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    public async Task<Dictionary<string, int>> ObterDistribuicaoGeograficaAsync(Guid redeId)
    {
        return await _context.Escolas
            .Where(e => e.RedeEscolarId == redeId)
            .GroupBy(e => e.Endereco.Estado)
            .Select(g => new { Estado = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Estado, x => x.Count);
    }

    public async Task<int> ObterCapacidadeTotalAsync(Guid redeId)
    {
        return await _context.Escolas
            .Where(e => e.RedeEscolarId == redeId)
            .SelectMany(e => e.Unidades)
            .SumAsync(u => u.CapacidadeMaximaAlunos);
    }

    public async Task<int> ObterOcupacaoTotalAsync(Guid redeId)
    {
        return await _context.Escolas
            .Where(e => e.RedeEscolarId == redeId)
            .SelectMany(e => e.Unidades)
            .SumAsync(u => u.AlunosMatriculados);
    }

    public async Task<double> ObterPercentualOcupacaoRedeAsync(Guid redeId)
    {
        var capacidadeTotal = await ObterCapacidadeTotalAsync(redeId);
        var ocupacaoTotal = await ObterOcupacaoTotalAsync(redeId);

        if (capacidadeTotal == 0)
            return 0;

        return (double)ocupacaoTotal / capacidadeTotal * 100;
    }

    public async Task<IEnumerable<RedeEscolar>> GetAllAsync()
    {
        return await ObterTodasAsync();
    }

    public async Task<RedeEscolar?> GetByIdAsync(Guid id)
    {
        return await ObterPorIdAsync(id);
    }

    public async Task AddAsync(RedeEscolar entity)
    {
        await AdicionarAsync(entity);
    }

    public async Task UpdateAsync(RedeEscolar entity)
    {
        await AtualizarAsync(entity);
    }

    public async Task DeleteAsync(RedeEscolar entity)
    {
        await RemoverAsync(entity);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.RedesEscolares.AnyAsync(r => r.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _context.RedesEscolares.CountAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}