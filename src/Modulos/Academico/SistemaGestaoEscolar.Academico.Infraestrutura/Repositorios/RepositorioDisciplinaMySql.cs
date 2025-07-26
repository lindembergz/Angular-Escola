using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.Academico.Dominio.Entidades;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;
using SistemaGestaoEscolar.Academico.Infraestrutura.Mapeadores;
using SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Repositorios;

public class RepositorioDisciplinaMySql : IRepositorioDisciplina
{
    private readonly DbContext _context;
    private readonly DbSet<DisciplinaEntity> _disciplinas;

    public RepositorioDisciplinaMySql(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _disciplinas = _context.Set<DisciplinaEntity>();
    }

    public async Task<Disciplina?> GetByIdAsync(Guid id)
    {
        return await ObterPorIdAsync(id);
    }

    public async Task<Disciplina?> ObterPorIdAsync(Guid id)
    {
        var entity = await _disciplinas
            .Include(d => d.PreRequisitos)
            .Include(d => d.DisciplinasQueRequerem)
            .FirstOrDefaultAsync(d => d.Id == id);
        
        return entity != null ? DisciplinaMapper.ToDomain(entity) : null;
    }

    public async Task<IEnumerable<Disciplina>> GetAllAsync()
    {
        var entities = await _disciplinas
            .Include(d => d.PreRequisitos)
            .Include(d => d.DisciplinasQueRequerem)
            .ToListAsync();
        
        return entities.Select(DisciplinaMapper.ToDomain);
    }

    public async Task AddAsync(Disciplina entity)
    {
        var disciplinaEntity = DisciplinaMapper.ToEntity(entity);
        await _disciplinas.AddAsync(disciplinaEntity);
        
        // Add pre-requisites
        foreach (var preRequisitoId in entity.PreRequisitos)
        {
            await _context.Set<DisciplinaPreRequisitoEntity>().AddAsync(new DisciplinaPreRequisitoEntity
            {
                Id = Guid.NewGuid(),
                DisciplinaId = entity.Id,
                PreRequisitoId = preRequisitoId,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    public async Task UpdateAsync(Disciplina entity)
    {
        var existingEntity = await _disciplinas
            .Include(d => d.PreRequisitos)
            .FirstOrDefaultAsync(d => d.Id == entity.Id);
            
        if (existingEntity != null)
        {
            DisciplinaMapper.UpdateEntity(existingEntity, entity);
            
            // Update pre-requisites
            var existingPreRequisitos = existingEntity.PreRequisitos.ToList();
            var novosPreRequisitos = entity.PreRequisitos.ToList();
            
            // Remove pre-requisites no longer needed
            foreach (var preRequisitoExistente in existingPreRequisitos)
            {
                if (!novosPreRequisitos.Contains(preRequisitoExistente.PreRequisitoId))
                {
                    _context.Set<DisciplinaPreRequisitoEntity>().Remove(preRequisitoExistente);
                }
            }
            
            // Add new pre-requisites
            foreach (var novoPreRequisitoId in novosPreRequisitos)
            {
                if (!existingPreRequisitos.Any(ep => ep.PreRequisitoId == novoPreRequisitoId))
                {
                    await _context.Set<DisciplinaPreRequisitoEntity>().AddAsync(new DisciplinaPreRequisitoEntity
                    {
                        Id = Guid.NewGuid(),
                        DisciplinaId = entity.Id,
                        PreRequisitoId = novoPreRequisitoId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }
    }

    public async Task DeleteAsync(Disciplina entity)
    {
        var existingEntity = await _disciplinas.FirstOrDefaultAsync(d => d.Id == entity.Id);
        if (existingEntity != null)
        {
            _disciplinas.Remove(existingEntity);
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _disciplinas.AnyAsync(d => d.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _disciplinas.CountAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    // Additional methods specific to Disciplina
    public async Task<IEnumerable<Disciplina>> ObterPorEscolaAsync(Guid escolaId)
    {
        var entities = await _disciplinas
            .Include(d => d.PreRequisitos)
            .Include(d => d.DisciplinasQueRequerem)
            .Where(d => d.EscolaId == escolaId)
            .ToListAsync();
        
        return entities.Select(DisciplinaMapper.ToDomain);
    }

    public async Task<IEnumerable<Disciplina>> ObterPorSerieAsync(int tipoSerie, int anoSerie)
    {
        var entities = await _disciplinas
            .Include(d => d.PreRequisitos)
            .Include(d => d.DisciplinasQueRequerem)
            .Where(d => d.TipoSerie == tipoSerie && d.AnoSerie == anoSerie)
            .ToListAsync();
        
        return entities.Select(DisciplinaMapper.ToDomain);
    }

    public async Task<IEnumerable<Disciplina>> ObterAtivasAsync()
    {
        var entities = await _disciplinas
            .Include(d => d.PreRequisitos)
            .Include(d => d.DisciplinasQueRequerem)
            .Where(d => d.Ativa)
            .ToListAsync();
        
        return entities.Select(DisciplinaMapper.ToDomain);
    }

    public async Task<IEnumerable<Disciplina>> ObterObrigatoriasAsync()
    {
        var entities = await _disciplinas
            .Include(d => d.PreRequisitos)
            .Include(d => d.DisciplinasQueRequerem)
            .Where(d => d.Obrigatoria && d.Ativa)
            .ToListAsync();
        
        return entities.Select(DisciplinaMapper.ToDomain);
    }

    public async Task<Disciplina?> ObterPorCodigoAsync(string codigo, Guid escolaId)
    {
        var entity = await _disciplinas
            .Include(d => d.PreRequisitos)
            .Include(d => d.DisciplinasQueRequerem)
            .FirstOrDefaultAsync(d => d.Codigo == codigo && d.EscolaId == escolaId);
        
        return entity != null ? DisciplinaMapper.ToDomain(entity) : null;
    }

    public async Task<bool> ExisteCodigoNaEscolaAsync(string codigo, Guid escolaId, Guid? excluirId = null)
    {
        var query = _disciplinas.Where(d => d.Codigo == codigo && d.EscolaId == escolaId);
        
        if (excluirId.HasValue)
            query = query.Where(d => d.Id != excluirId.Value);
            
        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Disciplina>> PesquisarPorNomeAsync(string nome, Guid escolaId)
    {
        var entities = await _disciplinas
            .Include(d => d.PreRequisitos)
            .Include(d => d.DisciplinasQueRequerem)
            .Where(d => EF.Functions.Like(d.Nome, $"%{nome}%") && d.EscolaId == escolaId)
            .ToListAsync();
        
        return entities.Select(DisciplinaMapper.ToDomain);
    }

    public async Task<IEnumerable<Disciplina>> ObterSemPreRequisitosAsync(Guid escolaId)
    {
        var entities = await _disciplinas
            .Include(d => d.PreRequisitos)
            .Include(d => d.DisciplinasQueRequerem)
            .Where(d => d.EscolaId == escolaId && !d.PreRequisitos.Any())
            .ToListAsync();
        
        return entities.Select(DisciplinaMapper.ToDomain);
    }

    public async Task<int> ContarPorCargaHorariaAsync(int cargaHorariaMinima, Guid escolaId)
    {
        return await _disciplinas
            .CountAsync(d => d.CargaHoraria >= cargaHorariaMinima && d.EscolaId == escolaId);
    }

    // Application layer compatibility methods
    public async Task AdicionarAsync(Disciplina disciplina)
    {
        await AddAsync(disciplina);
    }

    public async Task<IEnumerable<Disciplina>> ObterTodosAsync()
    {
        return await GetAllAsync();
    }
}