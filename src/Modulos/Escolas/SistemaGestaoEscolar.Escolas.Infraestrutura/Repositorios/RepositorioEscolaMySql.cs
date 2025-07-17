using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.Escolas.Dominio.Entidades;
using SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Escolas.Dominio.Repositorios;
using SistemaGestaoEscolar.Escolas.Infraestrutura.Contexto;

namespace SistemaGestaoEscolar.Escolas.Infraestrutura.Repositorios;

public class RepositorioEscolaMySql : IRepositorioEscola
{
    private readonly EscolasDbContext _context;

    public RepositorioEscolaMySql(EscolasDbContext context)
    {
        _context = context;
    }

    public async Task<Escola?> ObterPorIdAsync(Guid id)
    {
        return await _context.Escolas
            .Include(e => e.Unidades)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Escola?> ObterPorCnpjAsync(CNPJ cnpj)
    {
        return await _context.Escolas
            .Include(e => e.Unidades)
            .FirstOrDefaultAsync(e => e.Cnpj == cnpj);
    }

    public async Task<IEnumerable<Escola>> ObterPorRedeAsync(Guid redeEscolarId)
    {
        return await _context.Escolas
            .Include(e => e.Unidades)
            .Where(e => e.RedeEscolarId == redeEscolarId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Escola>> ObterPorTipoAsync(TipoEscola tipo)
    {
        return await _context.Escolas
            .Include(e => e.Unidades)
            .Where(e => e.Tipo == tipo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Escola>> ObterAtivasAsync()
    {
        return await _context.Escolas
            .Include(e => e.Unidades)
            .Where(e => e.Ativa)
            .ToListAsync();
    }

    public async Task<IEnumerable<Escola>> ObterInativasAsync()
    {
        return await _context.Escolas
            .Include(e => e.Unidades)
            .Where(e => !e.Ativa)
            .ToListAsync();
    }

    public async Task<IEnumerable<Escola>> PesquisarPorNomeAsync(string nome)
    {
        return await _context.Escolas
            .Include(e => e.Unidades)
            .Where(e => EF.Functions.Like(e.Nome.Valor, $"%{nome}%"))
            .ToListAsync();
    }

    public async Task<IEnumerable<Escola>> ObterPorCidadeAsync(string cidade)
    {
        return await _context.Escolas
            .Include(e => e.Unidades)
            .Where(e => e.Endereco.Cidade == cidade)
            .ToListAsync();
    }

    public async Task<IEnumerable<Escola>> ObterPorEstadoAsync(string estado)
    {
        return await _context.Escolas
            .Include(e => e.Unidades)
            .Where(e => e.Endereco.Estado == estado)
            .ToListAsync();
    }

    public async Task<bool> ExisteCnpjAsync(CNPJ cnpj)
    {
        return await _context.Escolas
            .AnyAsync(e => e.Cnpj == cnpj);
    }

    public async Task<bool> ExisteNomeAsync(NomeEscola nome, Guid? excluirId = null)
    {
        var query = _context.Escolas.Where(e => e.Nome == nome);
        
        if (excluirId.HasValue)
            query = query.Where(e => e.Id != excluirId.Value);
            
        return await query.AnyAsync();
    }

    public async Task<int> ContarEscolasAtivasAsync()
    {
        return await _context.Escolas
            .CountAsync(e => e.Ativa);
    }

    public async Task<int> ContarEscolasPorTipoAsync(TipoEscola tipo)
    {
        return await _context.Escolas
            .CountAsync(e => e.Tipo == tipo);
    }

    public async Task<int> ContarEscolasPorRedeAsync(Guid redeEscolarId)
    {
        return await _context.Escolas
            .CountAsync(e => e.RedeEscolarId == redeEscolarId);
    }

    public async Task<int> ContarEscolasPorEstadoAsync(string estado)
    {
        return await _context.Escolas
            .CountAsync(e => e.Endereco.Estado == estado);
    }

    public async Task AdicionarAsync(Escola escola)
    {
        await _context.Escolas.AddAsync(escola);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Escola escola)
    {
        _context.Escolas.Update(escola);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Escola escola)
    {
        _context.Escolas.Remove(escola);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Escola>> ObterComUnidadesAsync()
    {
        return await _context.Escolas
            .Include(e => e.Unidades)
            .ToListAsync();
    }

    public async Task<Escola?> ObterComUnidadesPorIdAsync(Guid id)
    {
        return await _context.Escolas
            .Include(e => e.Unidades)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Dictionary<string, int>> ObterEstatisticasPorTipoAsync()
    {
        return await _context.Escolas
            .GroupBy(e => e.Tipo)
            .Select(g => new { Tipo = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(x => x.Tipo, x => x.Count);
    }

    public async Task<Dictionary<string, int>> ObterEstatisticasPorEstadoAsync()
    {
        return await _context.Escolas
            .GroupBy(e => e.Endereco.Estado)
            .Select(g => new { Estado = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Estado, x => x.Count);
    }

    public async Task<IEnumerable<Escola>> ObterEscolasComMaiorCapacidadeAsync(int limite = 10)
    {
        return await _context.Escolas
            .Include(e => e.Unidades)
            .OrderByDescending(e => e.Unidades.Sum(u => u.CapacidadeMaximaAlunos))
            .Take(limite)
            .ToListAsync();
    }

    public async Task<IEnumerable<Escola>> ObterEscolasComMenorOcupacaoAsync(int limite = 10)
    {
        return await _context.Escolas
            .Include(e => e.Unidades)
            .OrderBy(e => e.Unidades.Sum(u => u.AlunosMatriculados))
            .Take(limite)
            .ToListAsync();
    }

    public async Task<IEnumerable<Escola>> BuscarAvancadaAsync(
        string? nome = null,
        TipoEscola? tipo = null,
        string? cidade = null,
        string? estado = null,
        Guid? redeEscolarId = null,
        bool? ativa = null,
        int? capacidadeMinima = null,
        int? capacidadeMaxima = null)
    {
        var query = _context.Escolas
            .Include(e => e.Unidades)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(e => EF.Functions.Like(e.Nome.Valor, $"%{nome}%"));

        if (tipo != null)
            query = query.Where(e => e.Tipo == tipo);

        if (!string.IsNullOrWhiteSpace(cidade))
            query = query.Where(e => e.Endereco.Cidade == cidade);

        if (!string.IsNullOrWhiteSpace(estado))
            query = query.Where(e => e.Endereco.Estado == estado);

        if (redeEscolarId.HasValue)
            query = query.Where(e => e.RedeEscolarId == redeEscolarId.Value);

        if (ativa.HasValue)
            query = query.Where(e => e.Ativa == ativa.Value);

        if (capacidadeMinima.HasValue)
            query = query.Where(e => e.Unidades.Sum(u => u.CapacidadeMaximaAlunos) >= capacidadeMinima.Value);

        if (capacidadeMaxima.HasValue)
            query = query.Where(e => e.Unidades.Sum(u => u.CapacidadeMaximaAlunos) <= capacidadeMaxima.Value);

        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<Escola> Escolas, int Total)> ObterPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        string? filtroNome = null,
        TipoEscola? filtroTipo = null,
        bool? filtroAtiva = null)
    {
        var query = _context.Escolas
            .Include(e => e.Unidades)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtroNome))
            query = query.Where(e => EF.Functions.Like(e.Nome.Valor, $"%{filtroNome}%"));

        if (filtroTipo != null)
            query = query.Where(e => e.Tipo == filtroTipo);

        if (filtroAtiva.HasValue)
            query = query.Where(e => e.Ativa == filtroAtiva.Value);

        var total = await query.CountAsync();

        var escolas = await query
            .OrderBy(e => e.Nome.Valor)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (escolas, total);
    }

    public async Task<IEnumerable<Escola>> GetAllAsync()
    {
        return await _context.Escolas
            .Include(e => e.Unidades)
            .ToListAsync();
    }

    public async Task<Escola?> GetByIdAsync(Guid id)
    {
        return await ObterPorIdAsync(id);
    }

    public async Task AddAsync(Escola entity)
    {
        await AdicionarAsync(entity);
    }

    public async Task UpdateAsync(Escola entity)
    {
        await AtualizarAsync(entity);
    }

    public async Task DeleteAsync(Escola entity)
    {
        await RemoverAsync(entity);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Escolas.AnyAsync(e => e.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _context.Escolas.CountAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}