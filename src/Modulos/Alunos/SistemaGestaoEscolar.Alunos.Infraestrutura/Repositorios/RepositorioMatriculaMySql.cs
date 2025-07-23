using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.Alunos.Dominio.Entidades;
using SistemaGestaoEscolar.Alunos.Dominio.Repositorios;
using SistemaGestaoEscolar.Alunos.Infraestrutura.Mapeadores;
using SistemaGestaoEscolar.Alunos.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Alunos.Infraestrutura.Repositorios;

public class RepositorioMatriculaMySql : IRepositorioMatricula
{
    private readonly DbContext _context;
    private readonly DbSet<MatriculaEntity> _matriculas;

    public RepositorioMatriculaMySql(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _matriculas = _context.Set<MatriculaEntity>();
    }

    public async Task<Matricula?> ObterPorIdAsync(Guid id)
    {
        var entity = await _matriculas.FirstOrDefaultAsync(m => m.Id == id);
        return entity != null ? MatriculaMapper.ToDomain(entity) : null;
    }

    public async Task<Matricula?> ObterPorNumeroAsync(string numeroMatricula)
    {
        var entity = await _matriculas.FirstOrDefaultAsync(m => m.NumeroMatricula == numeroMatricula);
        return entity != null ? MatriculaMapper.ToDomain(entity) : null;
    }

    public async Task<IEnumerable<Matricula>> ObterPorAlunoAsync(Guid alunoId)
    {
        var entities = await _matriculas
            .Where(m => m.AlunoId == alunoId)
            .OrderByDescending(m => m.DataMatricula)
            .ToListAsync();
        
        return entities.Select(MatriculaMapper.ToDomain);
    }

    public async Task<IEnumerable<Matricula>> ObterPorTurmaAsync(Guid turmaId)
    {
        var entities = await _matriculas
            .Where(m => m.TurmaId == turmaId)
            .OrderBy(m => m.DataMatricula)
            .ToListAsync();
        
        return entities.Select(MatriculaMapper.ToDomain);
    }

    public async Task<IEnumerable<Matricula>> ObterPorAnoLetivoAsync(int anoLetivo)
    {
        var entities = await _matriculas
            .Where(m => m.AnoLetivo == anoLetivo)
            .OrderBy(m => m.DataMatricula)
            .ToListAsync();
        
        return entities.Select(MatriculaMapper.ToDomain);
    }

    public async Task<IEnumerable<Matricula>> ObterAtivasAsync()
    {
        var entities = await _matriculas
            .Where(m => m.Ativa)
            .OrderBy(m => m.DataMatricula)
            .ToListAsync();
        
        return entities.Select(MatriculaMapper.ToDomain);
    }

    public async Task<IEnumerable<Matricula>> ObterInativasAsync()
    {
        var entities = await _matriculas
            .Where(m => !m.Ativa)
            .OrderByDescending(m => m.DataCancelamento)
            .ToListAsync();
        
        return entities.Select(MatriculaMapper.ToDomain);
    }

    public async Task<IEnumerable<Matricula>> ObterSuspensasAsync()
    {
        var entities = await _matriculas
            .Where(m => m.Status == (int)StatusMatricula.Suspensa)
            .OrderBy(m => m.DataMatricula)
            .ToListAsync();
        
        return entities.Select(MatriculaMapper.ToDomain);
    }

    public async Task<IEnumerable<Matricula>> ObterCanceladasAsync()
    {
        var entities = await _matriculas
            .Where(m => m.Status == (int)StatusMatricula.Cancelada)
            .OrderByDescending(m => m.DataCancelamento)
            .ToListAsync();
        
        return entities.Select(MatriculaMapper.ToDomain);
    }

    public async Task<Matricula?> ObterMatriculaAtivaDoAlunoAsync(Guid alunoId)
    {
        var entity = await _matriculas
            .Where(m => m.AlunoId == alunoId && m.Ativa && m.Status == (int)StatusMatricula.Ativa)
            .OrderByDescending(m => m.DataMatricula)
            .FirstOrDefaultAsync();
        
        return entity != null ? MatriculaMapper.ToDomain(entity) : null;
    }

    public async Task<IEnumerable<Matricula>> ObterMatriculasAtivasPorTurmaAsync(Guid turmaId)
    {
        var entities = await _matriculas
            .Where(m => m.TurmaId == turmaId && m.Ativa && m.Status == (int)StatusMatricula.Ativa)
            .OrderBy(m => m.DataMatricula)
            .ToListAsync();
        
        return entities.Select(MatriculaMapper.ToDomain);
    }

    public async Task<bool> ExisteMatriculaAtivaAsync(Guid alunoId, int anoLetivo)
    {
        return await _matriculas.AnyAsync(m => 
            m.AlunoId == alunoId && 
            m.AnoLetivo == anoLetivo && 
            m.Ativa && 
            m.Status == (int)StatusMatricula.Ativa);
    }

    public async Task<bool> ExisteNumeroMatriculaAsync(string numeroMatricula)
    {
        return await _matriculas.AnyAsync(m => m.NumeroMatricula == numeroMatricula);
    }

    public async Task<int> ContarMatriculasAtivasAsync()
    {
        return await _matriculas.CountAsync(m => m.Ativa && m.Status == (int)StatusMatricula.Ativa);
    }

    public async Task<int> ContarMatriculasPorTurmaAsync(Guid turmaId)
    {
        return await _matriculas.CountAsync(m => m.TurmaId == turmaId && m.Ativa);
    }

    public async Task<int> ContarMatriculasPorAnoLetivoAsync(int anoLetivo)
    {
        return await _matriculas.CountAsync(m => m.AnoLetivo == anoLetivo && m.Ativa);
    }

    public async Task AdicionarAsync(Matricula matricula)
    {
        var entity = MatriculaMapper.ToEntity(matricula);
        await _matriculas.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Matricula matricula)
    {
        var entity = await _matriculas.FirstOrDefaultAsync(m => m.Id == matricula.Id);
        if (entity != null)
        {
            MatriculaMapper.UpdateEntity(entity, matricula);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoverAsync(Matricula matricula)
    {
        var entity = await _matriculas.FirstOrDefaultAsync(m => m.Id == matricula.Id);
        if (entity != null)
        {
            _matriculas.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Dictionary<StatusMatricula, int>> ObterEstatisticasPorStatusAsync()
    {
        var estatisticas = await _matriculas
            .GroupBy(m => m.Status)
            .ToDictionaryAsync(g => (StatusMatricula)g.Key, g => g.Count());

        return estatisticas;
    }

    public async Task<Dictionary<int, int>> ObterEstatisticasPorAnoLetivoAsync()
    {
        return await _matriculas
            .GroupBy(m => m.AnoLetivo)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    public async Task<Dictionary<Guid, int>> ObterEstatisticasPorTurmaAsync()
    {
        return await _matriculas
            .Where(m => m.Ativa)
            .GroupBy(m => m.TurmaId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    public async Task<IEnumerable<Matricula>> ObterMatriculasRecentesAsync(int dias = 30)
    {
        var dataLimite = DateTime.UtcNow.AddDays(-dias);
        
        var entities = await _matriculas
            .Where(m => m.DataMatricula >= dataLimite)
            .OrderByDescending(m => m.DataMatricula)
            .ToListAsync();
        
        return entities.Select(MatriculaMapper.ToDomain);
    }

    public async Task<IEnumerable<Matricula>> ObterCancelamentosRecentesAsync(int dias = 30)
    {
        var dataLimite = DateTime.UtcNow.AddDays(-dias);
        
        var entities = await _matriculas
            .Where(m => m.DataCancelamento.HasValue && m.DataCancelamento >= dataLimite)
            .OrderByDescending(m => m.DataCancelamento)
            .ToListAsync();
        
        return entities.Select(MatriculaMapper.ToDomain);
    }

    public async Task<IEnumerable<Matricula>> ObterTransferenciasRecentesAsync(int dias = 30)
    {
        var dataLimite = DateTime.UtcNow.AddDays(-dias);
        
        var entities = await _matriculas
            .Where(m => m.Status == (int)StatusMatricula.Transferida && m.UpdatedAt >= dataLimite)
            .OrderByDescending(m => m.UpdatedAt)
            .ToListAsync();
        
        return entities.Select(MatriculaMapper.ToDomain);
    }

    public async Task<IEnumerable<Matricula>> BuscarAvancadaAsync(
        Guid? alunoId = null,
        Guid? turmaId = null,
        int? anoLetivo = null,
        StatusMatricula? status = null,
        bool? ativa = null,
        DateTime? dataMatriculaInicio = null,
        DateTime? dataMatriculaFim = null)
    {
        var query = _matriculas.AsQueryable();

        if (alunoId.HasValue)
            query = query.Where(m => m.AlunoId == alunoId.Value);

        if (turmaId.HasValue)
            query = query.Where(m => m.TurmaId == turmaId.Value);

        if (anoLetivo.HasValue)
            query = query.Where(m => m.AnoLetivo == anoLetivo.Value);

        if (status.HasValue)
            query = query.Where(m => m.Status == (int)status.Value);

        if (ativa.HasValue)
            query = query.Where(m => m.Ativa == ativa.Value);

        if (dataMatriculaInicio.HasValue)
            query = query.Where(m => m.DataMatricula >= dataMatriculaInicio.Value);

        if (dataMatriculaFim.HasValue)
            query = query.Where(m => m.DataMatricula <= dataMatriculaFim.Value);

        var entities = await query
            .OrderByDescending(m => m.DataMatricula)
            .ToListAsync();

        return entities.Select(MatriculaMapper.ToDomain);
    }

    public async Task<(IEnumerable<Matricula> Matriculas, int Total)> ObterPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        Guid? filtroTurma = null,
        int? filtroAnoLetivo = null,
        StatusMatricula? filtroStatus = null)
    {
        var query = _matriculas.AsQueryable();

        if (filtroTurma.HasValue)
            query = query.Where(m => m.TurmaId == filtroTurma.Value);

        if (filtroAnoLetivo.HasValue)
            query = query.Where(m => m.AnoLetivo == filtroAnoLetivo.Value);

        if (filtroStatus.HasValue)
            query = query.Where(m => m.Status == (int)filtroStatus.Value);

        var total = await query.CountAsync();

        var entities = await query
            .OrderByDescending(m => m.DataMatricula)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        var matriculas = entities.Select(MatriculaMapper.ToDomain);

        return (matriculas, total);
    }

    public async Task<bool> PodeMatricularAlunoAsync(Guid alunoId, Guid turmaId, int anoLetivo)
    {
        // Verificar se já tem matrícula ativa no ano
        var jaTemMatricula = await ExisteMatriculaAtivaAsync(alunoId, anoLetivo);
        if (jaTemMatricula)
            return false;

        // Verificar conflitos (mesmo aluno, mesma turma, mesmo ano)
        var temConflito = await _matriculas.AnyAsync(m => 
            m.AlunoId == alunoId && 
            m.TurmaId == turmaId && 
            m.AnoLetivo == anoLetivo);

        return !temConflito;
    }

    public async Task<int> ObterCapacidadeDisponivelTurmaAsync(Guid turmaId, int anoLetivo)
    {
        // Esta implementação seria mais complexa, integrando com o módulo Acadêmico
        // Por simplicidade, assumindo capacidade fixa de 30 alunos por turma
        const int capacidadeMaxima = 30;
        
        var matriculasAtivas = await ContarMatriculasPorTurmaAsync(turmaId);
        return Math.Max(0, capacidadeMaxima - matriculasAtivas);
    }

    public async Task<IEnumerable<Matricula>> ObterConflitosMatriculaAsync(Guid alunoId, int anoLetivo)
    {
        var entities = await _matriculas
            .Where(m => m.AlunoId == alunoId && m.AnoLetivo == anoLetivo)
            .ToListAsync();

        return entities.Select(MatriculaMapper.ToDomain);
    }

    // Implementação da interface IRepository<Matricula>
    public async Task<Matricula?> GetByIdAsync(Guid id)
    {
        return await ObterPorIdAsync(id);
    }

    public async Task<IEnumerable<Matricula>> GetAllAsync()
    {
        var entities = await _matriculas.ToListAsync();
        return entities.Select(MatriculaMapper.ToDomain);
    }

    public async Task AddAsync(Matricula entity)
    {
        await AdicionarAsync(entity);
    }

    public async Task UpdateAsync(Matricula entity)
    {
        await AtualizarAsync(entity);
    }

    public async Task DeleteAsync(Matricula entity)
    {
        await RemoverAsync(entity);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _matriculas.AnyAsync(m => m.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _matriculas.CountAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}