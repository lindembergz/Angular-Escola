using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.Alunos.Dominio.Entidades;
using SistemaGestaoEscolar.Alunos.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Alunos.Dominio.Repositorios;
using SistemaGestaoEscolar.Alunos.Infraestrutura.Mapeadores;
using SistemaGestaoEscolar.Alunos.Infraestrutura.Persistencia.Entidades;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Alunos.Infraestrutura.Repositorios;

public class RepositorioAlunoMySql : IRepositorioAluno
{
    private readonly DbContext _context;
    private readonly DbSet<AlunoEntity> _alunos;

    public RepositorioAlunoMySql(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _alunos = _context.Set<AlunoEntity>();
    }

    public async Task<Aluno?> ObterPorIdAsync(Guid id)
    {
        var entity = await _alunos.FirstOrDefaultAsync(a => a.Id == id);
        return entity != null ? AlunoMapper.ToDomain(entity) : null;
    }

    public async Task<Aluno?> ObterPorCpfAsync(Cpf cpf)
    {
        var entity = await _alunos.FirstOrDefaultAsync(a => a.Cpf == cpf.Numero);
        return entity != null ? AlunoMapper.ToDomain(entity) : null;
    }

    public async Task<IEnumerable<Aluno>> ObterPorEscolaAsync(Guid escolaId)
    {
        var entities = await _alunos
            .Where(a => a.EscolaId == escolaId)
            .ToListAsync();
        
        return entities.Select(AlunoMapper.ToDomain);
    }

    public async Task<IEnumerable<Aluno>> ObterAtivosAsync()
    {
        var entities = await _alunos
            .Where(a => a.Ativo)
            .ToListAsync();
        
        return entities.Select(AlunoMapper.ToDomain);
    }

    public async Task<IEnumerable<Aluno>> ObterInativosAsync()
    {
        var entities = await _alunos
            .Where(a => !a.Ativo)
            .ToListAsync();
        
        return entities.Select(AlunoMapper.ToDomain);
    }

    public async Task<IEnumerable<Aluno>> PesquisarPorNomeAsync(string nome)
    {
        var entities = await _alunos
            .Where(a => EF.Functions.Like(a.Nome, $"%{nome}%"))
            .ToListAsync();
        
        return entities.Select(AlunoMapper.ToDomain);
    }

    public async Task<IEnumerable<Aluno>> ObterPorIdadeAsync(int idadeMinima, int idadeMaxima)
    {
        var dataMaxima = DateTime.Today.AddYears(-idadeMinima);
        var dataMinima = DateTime.Today.AddYears(-idadeMaxima - 1);

        var entities = await _alunos
            .Where(a => a.DataNascimento >= dataMinima && a.DataNascimento <= dataMaxima)
            .ToListAsync();
        
        return entities.Select(AlunoMapper.ToDomain);
    }

    public async Task<IEnumerable<Aluno>> ObterPorFaixaEtariaAsync(string faixaEtaria)
    {
        // Implementação simplificada - seria mais complexa baseada na faixa etária
        var entities = await _alunos.ToListAsync();
        var alunos = entities.Select(AlunoMapper.ToDomain);
        
        return alunos.Where(a => a.ObterFaixaEtariaEscolar() == faixaEtaria);
    }

    public async Task<bool> ExisteCpfAsync(Cpf cpf)
    {
        return await _alunos.AnyAsync(a => a.Cpf == cpf.Numero);
    }

    public async Task<bool> ExisteEmailAsync(string email, Guid? excluirId = null)
    {
        var query = _alunos.Where(a => a.Email == email);
        
        if (excluirId.HasValue)
            query = query.Where(a => a.Id != excluirId.Value);
            
        return await query.AnyAsync();
    }

    public async Task<int> ContarAlunosAtivosAsync()
    {
        return await _alunos.CountAsync(a => a.Ativo);
    }

    public async Task<int> ContarAlunosPorEscolaAsync(Guid escolaId)
    {
        return await _alunos.CountAsync(a => a.EscolaId == escolaId);
    }

    public async Task<int> ContarAlunosPorIdadeAsync(int idade)
    {
        var dataMaxima = DateTime.Today.AddYears(-idade);
        var dataMinima = DateTime.Today.AddYears(-idade - 1);

        return await _alunos.CountAsync(a => a.DataNascimento >= dataMinima && a.DataNascimento <= dataMaxima);
    }

    public async Task AdicionarAsync(Aluno aluno)
    {
        var entity = AlunoMapper.ToEntity(aluno);
        await _alunos.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Aluno aluno)
    {
        var entity = await _alunos
            .Include(a => a.Responsaveis)
            .Include(a => a.Matriculas)
            .FirstOrDefaultAsync(a => a.Id == aluno.Id);
            
        if (entity != null)
        {
            // Atualizar dados básicos do aluno
            entity.Nome = aluno.Nome.Valor;
            entity.DataNascimento = aluno.DataNascimento.Valor;
            entity.Logradouro = aluno.Endereco.Logradouro;
            entity.Numero = aluno.Endereco.Numero;
            entity.Complemento = aluno.Endereco.Complemento;
            entity.Bairro = aluno.Endereco.Bairro;
            entity.Cidade = aluno.Endereco.Cidade;
            entity.Estado = aluno.Endereco.Estado;
            entity.Cep = aluno.Endereco.Cep;
            entity.Genero = (int)aluno.Genero.Tipo;
            entity.TipoDeficiencia = aluno.Deficiencia.PossuiDeficiencia ? (int)aluno.Deficiencia.Tipo! : null;
            entity.DescricaoDeficiencia = aluno.Deficiencia.PossuiDeficiencia ? aluno.Deficiencia.Descricao : null;
            entity.Telefone = aluno.Telefone;
            entity.Email = aluno.Email;
            entity.Observacoes = aluno.Observacoes;
            entity.EscolaId = aluno.EscolaId;
            entity.Ativo = aluno.Ativo;
            entity.UpdatedAt = aluno.UpdatedAt;

            // Remover responsáveis existentes
            var responsaveisParaRemover = entity.Responsaveis.ToList();
            foreach (var responsavelExistente in responsaveisParaRemover)
            {
                _context.Entry(responsavelExistente).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            }

            // Adicionar novos responsáveis
            foreach (var responsavel in aluno.Responsaveis)
            {
                var responsavelEntity = ResponsavelMapper.ToEntity(responsavel);
                responsavelEntity.Id = Guid.NewGuid();
                responsavelEntity.AlunoId = entity.Id;
                
                entity.Responsaveis.Add(responsavelEntity);
                // Forçar estado como Added para garantir INSERT
                _context.Entry(responsavelEntity).State = Microsoft.EntityFrameworkCore.EntityState.Added;
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoverAsync(Aluno aluno)
    {
        var entity = await _alunos.FirstOrDefaultAsync(a => a.Id == aluno.Id);
        if (entity != null)
        {
            _alunos.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Aluno?> ObterComResponsaveisPorIdAsync(Guid id)
    {
        var entity = await _alunos
            .Include(a => a.Responsaveis)
            .FirstOrDefaultAsync(a => a.Id == id);
        
        return entity != null ? AlunoMapper.ToDomain(entity) : null;
    }

    public async Task<Aluno?> ObterComMatriculasPorIdAsync(Guid id)
    {
        var entity = await _alunos
            .Include(a => a.Matriculas)
            .Include(r=> r.Responsaveis)
            .FirstOrDefaultAsync(a => a.Id == id);
        
        return entity != null ? AlunoMapper.ToDomain(entity) : null;
    }

    public async Task<Aluno?> ObterCompletoAsync(Guid id)
    {
        var entity = await _alunos
            .Include(a => a.Responsaveis)
            .Include(a => a.Matriculas)
            .FirstOrDefaultAsync(a => a.Id == id);
        
        return entity != null ? AlunoMapper.ToDomain(entity) : null;
    }

    public async Task<IEnumerable<Aluno>> ObterComResponsaveisAsync()
    {
        var entities = await _alunos
            .Include(a => a.Responsaveis)
            .ToListAsync();
        
        return entities.Select(AlunoMapper.ToDomain);
    }

    public async Task<IEnumerable<Aluno>> ObterComMatriculasAtivasAsync()
    {
        var entities = await _alunos
            .Include(a => a.Matriculas.Where(m => m.Ativa))
            .ToListAsync();
        
        return entities.Select(AlunoMapper.ToDomain);
    }

    public async Task<Dictionary<string, int>> ObterEstatisticasPorIdadeAsync()
    {
        var alunos = await _alunos.ToListAsync();
        var estatisticas = new Dictionary<string, int>();

        foreach (var aluno in alunos)
        {
            var idade = DateTime.Today.Year - aluno.DataNascimento.Year;
            if (DateTime.Today < aluno.DataNascimento.AddYears(idade))
                idade--;

            var faixa = idade switch
            {
                < 6 => "0-5 anos",
                < 11 => "6-10 anos",
                < 15 => "11-14 anos",
                < 18 => "15-17 anos",
                _ => "18+ anos"
            };

            estatisticas[faixa] = estatisticas.GetValueOrDefault(faixa, 0) + 1;
        }

        return estatisticas;
    }

    public async Task<Dictionary<string, int>> ObterEstatisticasPorEscolaAsync()
    {
        return await _alunos
            .GroupBy(a => a.EscolaId)
            .ToDictionaryAsync(g => g.Key.ToString(), g => g.Count());
    }

    public async Task<Dictionary<string, int>> ObterEstatisticasPorFaixaEtariaAsync()
    {
        var alunos = await _alunos.ToListAsync();
        var estatisticas = new Dictionary<string, int>();

        foreach (var aluno in alunos)
        {
            var domainAluno = AlunoMapper.ToDomain(aluno);
            var faixa = domainAluno.ObterFaixaEtariaEscolar();
            estatisticas[faixa] = estatisticas.GetValueOrDefault(faixa, 0) + 1;
        }

        return estatisticas;
    }

    public async Task<IEnumerable<Aluno>> ObterAniversariantesDoMesAsync(int mes)
    {
        var entities = await _alunos
            .Where(a => a.DataNascimento.Month == mes)
            .ToListAsync();
        
        return entities.Select(AlunoMapper.ToDomain);
    }

    public async Task<IEnumerable<Aluno>> ObterSemResponsavelFinanceiroAsync()
    {
        var entities = await _alunos
            .Include(a => a.Responsaveis)
            .Where(a => !a.Responsaveis.Any(r => r.ResponsavelFinanceiro))
            .ToListAsync();
        
        return entities.Select(AlunoMapper.ToDomain);
    }

    public async Task<IEnumerable<Aluno>> ObterSemMatriculaAtivaAsync()
    {
        var entities = await _alunos
            .Include(a => a.Matriculas)
            .Where(a => !a.Matriculas.Any(m => m.Ativa))
            .ToListAsync();
        
        return entities.Select(AlunoMapper.ToDomain);
    }

    public async Task<IEnumerable<Aluno>> BuscarAvancadaAsync(
        string? nome = null,
        Cpf? cpf = null,
        Guid? escolaId = null,
        int? idadeMinima = null,
        int? idadeMaxima = null,
        string? cidade = null,
        string? estado = null,
        bool? ativo = null,
        bool? possuiMatriculaAtiva = null)
    {
        var query = _alunos
            .Include(a => a.Responsaveis) // Always include responsaveis for correct count
            .AsQueryable();

        if (!string.IsNullOrEmpty(nome))
            query = query.Where(a => EF.Functions.Like(a.Nome, $"%{nome}%"));

        if (cpf != null)
            query = query.Where(a => a.Cpf == cpf.Numero);

        if (escolaId.HasValue)
            query = query.Where(a => a.EscolaId == escolaId.Value);

        if (!string.IsNullOrEmpty(cidade))
            query = query.Where(a => a.Cidade == cidade);

        if (!string.IsNullOrEmpty(estado))
            query = query.Where(a => a.Estado == estado);

        if (ativo.HasValue)
            query = query.Where(a => a.Ativo == ativo.Value);

        if (idadeMinima.HasValue || idadeMaxima.HasValue)
        {
            var dataMaxima = idadeMinima.HasValue ? DateTime.Today.AddYears(-idadeMinima.Value) : DateTime.MaxValue;
            var dataMinima = idadeMaxima.HasValue ? DateTime.Today.AddYears(-idadeMaxima.Value - 1) : DateTime.MinValue;
            query = query.Where(a => a.DataNascimento >= dataMinima && a.DataNascimento <= dataMaxima);
        }

        if (possuiMatriculaAtiva.HasValue)
        {
            query = query.Include(a => a.Matriculas);
            if (possuiMatriculaAtiva.Value)
                query = query.Where(a => a.Matriculas.Any(m => m.Ativa));
            else
                query = query.Where(a => !a.Matriculas.Any(m => m.Ativa));
        }

        var entities = await query.ToListAsync();
        return entities.Select(AlunoMapper.ToDomain);
    }

    public async Task<(IEnumerable<Aluno> Alunos, int Total)> ObterPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        string? filtroNome = null,
        Guid? filtroEscola = null,
        bool? filtroAtivo = null)
    {
        var query = _alunos.AsQueryable();

        if (!string.IsNullOrEmpty(filtroNome))
            query = query.Where(a => EF.Functions.Like(a.Nome, $"%{filtroNome}%"));

        if (filtroEscola.HasValue)
            query = query.Where(a => a.EscolaId == filtroEscola.Value);

        if (filtroAtivo.HasValue)
            query = query.Where(a => a.Ativo == filtroAtivo.Value);

        var total = await query.CountAsync();

        var entities = await query
            .Include(a => a.Responsaveis) // Include responsaveis to fix count issue
            .Include(a => a.Matriculas)   // Include matriculas for active enrollment check
            .OrderBy(a => a.Nome)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        var alunos = entities.Select(AlunoMapper.ToDomain);

        return (alunos, total);
    }

    public async Task<IEnumerable<Aluno>> ObterPorResponsavelAsync(Guid responsavelId)
    {
        var entities = await _alunos
            .Include(a => a.Responsaveis)
            .Where(a => a.Responsaveis.Any(r => r.Id == responsavelId))
            .ToListAsync();
        
        return entities.Select(AlunoMapper.ToDomain);
    }

    public async Task<IEnumerable<Aluno>> ObterPorCpfResponsavelAsync(Cpf cpfResponsavel)
    {
        var entities = await _alunos
            .Include(a => a.Responsaveis)
            .Where(a => a.Responsaveis.Any(r => r.Cpf == cpfResponsavel.Numero))
            .ToListAsync();
        
        return entities.Select(AlunoMapper.ToDomain);
    }

    public async Task<IEnumerable<Aluno>> ObterPorEmailResponsavelAsync(string emailResponsavel)
    {
        var entities = await _alunos
            .Include(a => a.Responsaveis)
            .Where(a => a.Responsaveis.Any(r => r.Email == emailResponsavel))
            .ToListAsync();
        
        return entities.Select(AlunoMapper.ToDomain);
    }

    // Implementação da interface IRepository<Aluno>
    public async Task<Aluno?> GetByIdAsync(Guid id)
    {
        return await ObterPorIdAsync(id);
    }

    public async Task<IEnumerable<Aluno>> GetAllAsync()
    {
        var entities = await _alunos.ToListAsync();
        return entities.Select(AlunoMapper.ToDomain);
    }

    public async Task AddAsync(Aluno entity)
    {
        await AdicionarAsync(entity);
    }

    public async Task UpdateAsync(Aluno entity)
    {
        await AtualizarAsync(entity);
    }

    public async Task DeleteAsync(Aluno entity)
    {
        await RemoverAsync(entity);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _alunos.AnyAsync(a => a.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _alunos.CountAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}