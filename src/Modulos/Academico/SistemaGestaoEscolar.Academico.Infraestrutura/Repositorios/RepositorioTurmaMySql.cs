using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.Academico.Dominio.Entidades;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;
using SistemaGestaoEscolar.Academico.Infraestrutura.Mapeadores;
using SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Repositorios;

public class RepositorioTurmaMySql : IRepositorioTurma
{
    private readonly DbContext _context;
    private readonly DbSet<TurmaEntity> _turmas;

    public RepositorioTurmaMySql(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _turmas = _context.Set<TurmaEntity>();
    }

    public async Task<Turma?> GetByIdAsync(Guid id)
    {
        return await ObterPorIdAsync(id);
    }

    public async Task<Turma?> ObterPorIdAsync(Guid id)
    {
        var entity = await _turmas
            .Include(t => t.TurmaAlunos)
            .Include(t => t.Disciplinas)
            .FirstOrDefaultAsync(t => t.Id == id);
        
        return entity != null ? TurmaMapper.ToDomain(entity) : null;
    }

    public async Task<IEnumerable<Turma>> GetAllAsync()
    {
        var entities = await _turmas
            .Include(t => t.TurmaAlunos)
            .Include(t => t.Disciplinas)
            .ToListAsync();
        
        return entities.Select(TurmaMapper.ToDomain);
    }

    public async Task AddAsync(Turma entity)
    {
        var turmaEntity = TurmaMapper.ToEntity(entity);
        await _turmas.AddAsync(turmaEntity);
    }

    public async Task UpdateAsync(Turma entity)
    {
        var existingEntity = await _turmas
            .Include(t => t.TurmaAlunos)
            .FirstOrDefaultAsync(t => t.Id == entity.Id);
            
        if (existingEntity != null)
        {
            TurmaMapper.UpdateEntity(existingEntity, entity);
            
            // Update student enrollments
            var existingAlunos = existingEntity.TurmaAlunos.ToList();
            var novosAlunos = entity.AlunosMatriculados.ToList();
            
            // Remove students no longer enrolled
            foreach (var alunoExistente in existingAlunos)
            {
                if (!novosAlunos.Contains(alunoExistente.AlunoId))
                {
                    alunoExistente.Ativa = false;
                    alunoExistente.DataDesmatricula = DateTime.UtcNow;
                }
            }
            
            // Add new students
            foreach (var novoAlunoId in novosAlunos)
            {
                if (!existingAlunos.Any(ea => ea.AlunoId == novoAlunoId && ea.Ativa))
                {
                    existingEntity.TurmaAlunos.Add(new TurmaAlunoEntity
                    {
                        Id = Guid.NewGuid(),
                        TurmaId = entity.Id,
                        AlunoId = novoAlunoId,
                        DataMatricula = DateTime.UtcNow,
                        Ativa = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }
    }

    public async Task DeleteAsync(Turma entity)
    {
        var existingEntity = await _turmas.FirstOrDefaultAsync(t => t.Id == entity.Id);
        if (existingEntity != null)
        {
            _turmas.Remove(existingEntity);
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _turmas.AnyAsync(t => t.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _turmas.CountAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    // Additional methods specific to Turma
    public async Task<IEnumerable<Turma>> ObterPorEscolaAsync(Guid escolaId)
    {
        var entities = await _turmas
            .Include(t => t.TurmaAlunos)
            .Include(t => t.Disciplinas)
            .Where(t => t.EscolaId == escolaId)
            .ToListAsync();
        
        return entities.Select(TurmaMapper.ToDomain);
    }

    public async Task<IEnumerable<Turma>> ObterPorAnoLetivoAsync(int anoLetivo)
    {
        var entities = await _turmas
            .Include(t => t.TurmaAlunos)
            .Include(t => t.Disciplinas)
            .Where(t => t.AnoLetivo == anoLetivo)
            .ToListAsync();
        
        return entities.Select(TurmaMapper.ToDomain);
    }

    public async Task<IEnumerable<Turma>> ObterAtivasAsync()
    {
        var entities = await _turmas
            .Include(t => t.TurmaAlunos)
            .Include(t => t.Disciplinas)
            .Where(t => t.Ativa)
            .ToListAsync();
        
        return entities.Select(TurmaMapper.ToDomain);
    }

    public async Task<bool> ExisteNomeNaEscolaAsync(string nome, Guid escolaId, Guid? excluirId = null)
    {
        var query = _turmas.Where(t => t.Nome == nome && t.EscolaId == escolaId);
        
        if (excluirId.HasValue)
            query = query.Where(t => t.Id != excluirId.Value);
            
        return await query.AnyAsync();
    }

    public async Task<int> ContarAlunosMatriculadosAsync(Guid turmaId)
    {
        return await _context.Set<TurmaAlunoEntity>()
            .CountAsync(ta => ta.TurmaId == turmaId && ta.Ativa);
    }

    public async Task<IEnumerable<Turma>> ObterComVagasDisponiveisAsync(Guid escolaId)
    {
        var entities = await _turmas
            .Include(t => t.TurmaAlunos)
            .Include(t => t.Disciplinas)
            .Where(t => t.EscolaId == escolaId && t.Ativa)
            .ToListAsync();
        
        var turmas = entities.Select(TurmaMapper.ToDomain);
        return turmas.Where(t => t.PossuiVagasDisponiveis());
    }

    // Application layer compatibility methods
    public async Task AdicionarAsync(Turma turma)
    {
        await AddAsync(turma);
    }

    public async Task AtualizarAsync(Turma turma)
    {
        await UpdateAsync(turma);
    }

    public async Task<IEnumerable<Turma>> ObterTodasPorUnidadeEscolarAsync(Guid unidadeEscolarId)
    {
        return await ObterPorEscolaAsync(unidadeEscolarId);
    }
}