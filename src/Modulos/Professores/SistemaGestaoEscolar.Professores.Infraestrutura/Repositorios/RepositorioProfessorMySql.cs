using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.Professores.Dominio.Entidades;
using SistemaGestaoEscolar.Professores.Dominio.Repositorios;
using SistemaGestaoEscolar.Professores.Infraestrutura.Mapeadores;
using SistemaGestaoEscolar.Professores.Infraestrutura.Persistencia.Entidades;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Professores.Infraestrutura.Repositorios;

public class RepositorioProfessorMySql : IRepositorioProfessor
{
    private readonly DbContext _context;
    private readonly DbSet<ProfessorEntity> _professores;

    public RepositorioProfessorMySql(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _professores = _context.Set<ProfessorEntity>();
    }

    public async Task<Professor?> ObterPorIdAsync(Guid id)
    {
        var entity = await _professores
            .Include(p => p.Titulos)
            .Include(p => p.Disciplinas)
            .FirstOrDefaultAsync(p => p.Id == id);

        return entity != null ? ProfessorMapper.ToDomain(entity) : null;
    }

    public async Task<Professor?> ObterPorCpfAsync(Cpf cpf)
    {
        var entity = await _professores
            .Include(p => p.Titulos)
            .Include(p => p.Disciplinas)
            .FirstOrDefaultAsync(p => p.Cpf == cpf.Numero);

        return entity != null ? ProfessorMapper.ToDomain(entity) : null;
    }

    public async Task<Professor?> ObterPorRegistroAsync(string registro)
    {
        var entity = await _professores
            .Include(p => p.Titulos)
            .Include(p => p.Disciplinas)
            .FirstOrDefaultAsync(p => p.Registro == registro);

        return entity != null ? ProfessorMapper.ToDomain(entity) : null;
    }

    public async Task<IEnumerable<Professor>> ObterPorEscolaAsync(Guid escolaId)
    {
        var entities = await _professores
            .Include(p => p.Titulos)
            .Include(p => p.Disciplinas)
            .Where(p => p.EscolaId == escolaId)
            .ToListAsync();

        return entities.Select(ProfessorMapper.ToDomain);
    }

    public async Task<IEnumerable<Professor>> ObterAtivosAsync()
    {
        var entities = await _professores
            .Include(p => p.Titulos)
            .Include(p => p.Disciplinas)
            .Where(p => p.Ativo)
            .ToListAsync();

        return entities.Select(ProfessorMapper.ToDomain);
    }

    public async Task<IEnumerable<Professor>> ObterPorDisciplinaAsync(Guid disciplinaId)
    {
        var entities = await _professores
            .Include(p => p.Titulos)
            .Include(p => p.Disciplinas)
            .Where(p => p.Disciplinas.Any(d => d.DisciplinaId == disciplinaId && d.Ativa))
            .ToListAsync();

        return entities.Select(ProfessorMapper.ToDomain);
    }

    public async Task<bool> ExistePorCpfAsync(Cpf cpf)
    {
        return await _professores.AnyAsync(p => p.Cpf == cpf.Numero);
    }

    public async Task<bool> ExistePorRegistroAsync(string registro)
    {
        return await _professores.AnyAsync(p => p.Registro == registro);
    }

    public async Task AdicionarAsync(Professor professor)
    {
        var entity = ProfessorMapper.ToEntity(professor);
        await _professores.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Professor professor)
    {
        var entity = await _professores
            .Include(p => p.Titulos)
            .Include(p => p.Disciplinas)
            .FirstOrDefaultAsync(p => p.Id == professor.Id);

        if (entity != null)
        {
            // Update basic properties
            ProfessorMapper.UpdateEntity(entity, professor);

            // Update titles - remove existing and add new ones
            var titulosParaRemover = entity.Titulos.ToList();
            foreach (var titulo in titulosParaRemover)
            {
                _context.Entry(titulo).State = EntityState.Deleted;
            }

            // Add new titles
            foreach (var titulo in professor.Titulos)
            {
                var tituloEntity = new TituloAcademicoEntity
                {
                    Id = Guid.NewGuid(),
                    ProfessorId = professor.Id,
                    Tipo = (int)titulo.Tipo,
                    Curso = titulo.Curso,
                    Instituicao = titulo.Instituicao,
                    AnoFormatura = titulo.AnoFormatura,
                    DataCadastro = DateTime.UtcNow
                };

                entity.Titulos.Add(tituloEntity);
                _context.Entry(tituloEntity).State = EntityState.Added;
            }

            // Update disciplines - remove existing and add new ones
            var disciplinasParaRemover = entity.Disciplinas.ToList();
            foreach (var disciplina in disciplinasParaRemover)
            {
                _context.Entry(disciplina).State = EntityState.Deleted;
            }

            // Add new disciplines
            foreach (var disciplina in professor.Disciplinas)
            {
                var disciplinaEntity = ProfessorDisciplinaMapper.ToEntity(disciplina);
                entity.Disciplinas.Add(disciplinaEntity);
                _context.Entry(disciplinaEntity).State = EntityState.Added;
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoverAsync(Professor professor)
    {
        var entity = await _professores
            .Include(p => p.Titulos)
            .Include(p => p.Disciplinas)
            .FirstOrDefaultAsync(p => p.Id == professor.Id);

        if (entity != null)
        {
            _professores.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> ContarPorEscolaAsync(Guid escolaId)
    {
        return await _professores.CountAsync(p => p.EscolaId == escolaId);
    }

    public async Task<int> ContarAtivosAsync()
    {
        return await _professores.CountAsync(p => p.Ativo);
    }
}