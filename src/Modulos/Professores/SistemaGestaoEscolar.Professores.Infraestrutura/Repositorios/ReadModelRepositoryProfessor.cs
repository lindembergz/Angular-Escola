using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.Professores.Aplicacao.Queries;
using SistemaGestaoEscolar.Professores.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Professores.Infraestrutura.Repositorios;

public interface IReadModelRepositoryProfessor
{
    Task<PaginatedResult<ProfessorResumoDto>> ListarProfessoresAsync(
        Guid? escolaId = null,
        bool? ativo = null,
        string? nome = null,
        int pagina = 1,
        int tamanhoPagina = 10);

    Task<ProfessorDto?> ObterProfessorPorIdAsync(Guid id);
    Task<IEnumerable<ProfessorResumoDto>> ObterProfessoresPorDisciplinaAsync(Guid disciplinaId);
    Task<IEnumerable<ProfessorResumoDto>> ObterProfessoresAtivosAsync(Guid escolaId);
    Task<Dictionary<string, int>> ObterEstatisticasPorTituloAsync(Guid escolaId);
    Task<Dictionary<string, int>> ObterEstatisticasPorCargaHorariaAsync(Guid escolaId);
}

public class ReadModelRepositoryProfessor : IReadModelRepositoryProfessor
{
    private readonly DbContext _context;
    private readonly DbSet<ProfessorEntity> _professores;

    public ReadModelRepositoryProfessor(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _professores = _context.Set<ProfessorEntity>();
    }

    public async Task<PaginatedResult<ProfessorResumoDto>> ListarProfessoresAsync(
        Guid? escolaId = null,
        bool? ativo = null,
        string? nome = null,
        int pagina = 1,
        int tamanhoPagina = 10)
    {
        var query = _professores.AsNoTracking();

        // Apply filters
        if (escolaId.HasValue)
            query = query.Where(p => p.EscolaId == escolaId.Value);

        if (ativo.HasValue)
            query = query.Where(p => p.Ativo == ativo.Value);

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(p => EF.Functions.Like(p.Nome, $"%{nome}%"));

        var totalItems = await query.CountAsync();

        var items = await query
            .Include(p => p.Titulos)
            .Include(p => p.Disciplinas.Where(d => d.Ativa))
            .OrderBy(p => p.Nome)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .Select(p => new ProfessorResumoDto(
                p.Id,
                p.Nome,
                FormatarCpf(p.Cpf),
                FormatarRegistro(p.Registro),
                p.Email,
                p.EscolaId,
                p.Ativo,
                p.Disciplinas.Where(d => d.Ativa).Sum(d => d.CargaHorariaSemanal),
                p.Titulos.OrderByDescending(t => t.Tipo).FirstOrDefault() != null 
                    ? ObterDescricaoTitulo(p.Titulos.OrderByDescending(t => t.Tipo).First())
                    : "Não informado",
                p.Disciplinas.Count(d => d.Ativa)))
            .ToListAsync();

        var totalPaginas = (int)Math.Ceiling((double)totalItems / tamanhoPagina);

        return new PaginatedResult<ProfessorResumoDto>(
            items,
            totalItems,
            pagina,
            tamanhoPagina,
            totalPaginas);
    }

    public async Task<ProfessorDto?> ObterProfessorPorIdAsync(Guid id)
    {
        var professor = await _professores
            .AsNoTracking()
            .Include(p => p.Titulos)
            .Include(p => p.Disciplinas)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (professor == null)
            return null;

        return new ProfessorDto(
            professor.Id,
            professor.Nome,
            FormatarCpf(professor.Cpf),
            FormatarRegistro(professor.Registro),
            professor.Email,
            professor.Telefone,
            professor.DataNascimento,
            professor.DataContratacao,
            professor.EscolaId,
            professor.Ativo,
            professor.DataCadastro,
            professor.Observacoes,
            CalcularIdade(professor.DataNascimento),
            CalcularTempoServico(professor.DataContratacao),
            professor.Disciplinas.Where(d => d.Ativa).Sum(d => d.CargaHorariaSemanal),
            professor.Titulos.Select(t => new TituloAcademicoQueryDto(
                ObterNomeTipoTitulo(t.Tipo),
                t.Curso,
                t.Instituicao,
                t.AnoFormatura,
                ObterDescricaoTitulo(t))).ToList(),
            professor.Disciplinas.Select(d => new DisciplinaDto(
                d.Id,
                d.DisciplinaId,
               // d.NomeDisciplina,
                d.CargaHorariaSemanal,
                d.DataAtribuicao,
                d.Ativa)).ToList());
    }

    public async Task<IEnumerable<ProfessorResumoDto>> ObterProfessoresPorDisciplinaAsync(Guid disciplinaId)
    {
        return await _professores
            .AsNoTracking()
            .Include(p => p.Titulos)
            .Include(p => p.Disciplinas.Where(d => d.Ativa))
            .Where(p => p.Disciplinas.Any(d => d.DisciplinaId == disciplinaId && d.Ativa))
            .Select(p => new ProfessorResumoDto(
                p.Id,
                p.Nome,
                FormatarCpf(p.Cpf),
                FormatarRegistro(p.Registro),
                p.Email,
                p.EscolaId,
                p.Ativo,
                p.Disciplinas.Where(d => d.Ativa).Sum(d => d.CargaHorariaSemanal),
                p.Titulos.OrderByDescending(t => t.Tipo).FirstOrDefault() != null 
                    ? ObterDescricaoTitulo(p.Titulos.OrderByDescending(t => t.Tipo).First())
                    : "Não informado",
                p.Disciplinas.Count(d => d.Ativa)))
            .ToListAsync();
    }

    public async Task<IEnumerable<ProfessorResumoDto>> ObterProfessoresAtivosAsync(Guid escolaId)
    {
        return await _professores
            .AsNoTracking()
            .Include(p => p.Titulos)
            .Include(p => p.Disciplinas.Where(d => d.Ativa))
            .Where(p => p.EscolaId == escolaId && p.Ativo)
            .OrderBy(p => p.Nome)
            .Select(p => new ProfessorResumoDto(
                p.Id,
                p.Nome,
                FormatarCpf(p.Cpf),
                FormatarRegistro(p.Registro),
                p.Email,
                p.EscolaId,
                p.Ativo,
                p.Disciplinas.Where(d => d.Ativa).Sum(d => d.CargaHorariaSemanal),
                p.Titulos.OrderByDescending(t => t.Tipo).FirstOrDefault() != null 
                    ? ObterDescricaoTitulo(p.Titulos.OrderByDescending(t => t.Tipo).First())
                    : "Não informado",
                p.Disciplinas.Count(d => d.Ativa)))
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> ObterEstatisticasPorTituloAsync(Guid escolaId)
    {
        var professores = await _professores
            .AsNoTracking()
            .Include(p => p.Titulos)
            .Where(p => p.EscolaId == escolaId && p.Ativo)
            .ToListAsync();

        var estatisticas = new Dictionary<string, int>();

        foreach (var professor in professores)
        {
            var maiorTitulo = professor.Titulos.OrderByDescending(t => t.Tipo).FirstOrDefault();
            var tipoTitulo = maiorTitulo != null ? ObterNomeTipoTitulo(maiorTitulo.Tipo) : "Não informado";
            
            estatisticas[tipoTitulo] = estatisticas.GetValueOrDefault(tipoTitulo, 0) + 1;
        }

        return estatisticas;
    }

    public async Task<Dictionary<string, int>> ObterEstatisticasPorCargaHorariaAsync(Guid escolaId)
    {
        var professores = await _professores
            .AsNoTracking()
            .Include(p => p.Disciplinas.Where(d => d.Ativa))
            .Where(p => p.EscolaId == escolaId && p.Ativo)
            .ToListAsync();

        var estatisticas = new Dictionary<string, int>();

        foreach (var professor in professores)
        {
            var cargaTotal = professor.Disciplinas.Where(d => d.Ativa).Sum(d => d.CargaHorariaSemanal);
            var faixa = cargaTotal switch
            {
                0 => "Sem disciplinas",
                <= 10 => "1-10 horas",
                <= 20 => "11-20 horas",
                <= 30 => "21-30 horas",
                <= 40 => "31-40 horas",
                _ => "Mais de 40 horas"
            };

            estatisticas[faixa] = estatisticas.GetValueOrDefault(faixa, 0) + 1;
        }

        return estatisticas;
    }

    private static string FormatarCpf(string cpf)
    {
        if (cpf.Length == 11)
            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        return cpf;
    }

    private static string FormatarRegistro(string registro)
    {
        if (registro.Length <= 6)
            return registro;

        if (registro.Length <= 10)
        {
            return $"{registro.Substring(0, 4)}.{registro.Substring(4, Math.Min(4, registro.Length - 4))}" +
                   (registro.Length > 8 ? $".{registro.Substring(8)}" : "");
        }

        var grupos = new List<string>();
        for (int i = 0; i < registro.Length; i += 4)
        {
            grupos.Add(registro.Substring(i, Math.Min(4, registro.Length - i)));
        }
        return string.Join(".", grupos);
    }

    private static int CalcularIdade(DateTime dataNascimento)
    {
        var hoje = DateTime.Today;
        var idade = hoje.Year - dataNascimento.Year;
        if (dataNascimento.Date > hoje.AddYears(-idade))
            idade--;
        return idade;
    }

    private static int CalcularTempoServico(DateTime dataContratacao)
    {
        var hoje = DateTime.Today;
        var tempoServico = hoje.Year - dataContratacao.Year;
        if (dataContratacao.Date > hoje.AddYears(-tempoServico))
            tempoServico--;
        return Math.Max(0, tempoServico);
    }

    private static string ObterNomeTipoTitulo(int tipo)
    {
        return tipo switch
        {
            1 => "Ensino Médio",
            2 => "Tecnólogo",
            3 => "Graduação",
            4 => "Pós-Graduação",
            5 => "Mestrado",
            6 => "Doutorado",
            7 => "Pós-Doutorado",
            _ => "Não Informado"
        };
    }

    private static string ObterDescricaoTitulo(TituloAcademicoEntity titulo)
    {
        var tipoDescricao = ObterNomeTipoTitulo(titulo.Tipo);
        return $"{tipoDescricao} em {titulo.Curso} - {titulo.Instituicao} ({titulo.AnoFormatura})";
    }
}