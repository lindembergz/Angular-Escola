using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;
using SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Repositorios;

public class ReadModelRepositoryAcademico
{
    private readonly DbContext _context;

    public ReadModelRepositoryAcademico(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // Turma Read Models
    public async Task<IEnumerable<TurmaResumoReadDto>> ObterTurmasResumoAsync(Guid unidadeEscolarId)
    {
        return await _context.Set<TurmaEntity>()
            .Where(t => t.EscolaId == unidadeEscolarId && t.Ativa)
            .Select(t => new TurmaResumoReadDto
            {
                Id = t.Id,
                Nome = t.Nome,
                Serie = t.AnoSerie, // Using int as per existing DTO
                Turno = ObterDescricaoTurno(t.TipoTurno),
                CapacidadeMaxima = t.CapacidadeMaxima,
                AlunosMatriculados = t.TurmaAlunos.Count(ta => ta.Ativa)
            })
            .OrderBy(t => t.Serie)
            .ThenBy(t => t.Nome)
            .ToListAsync();
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

    // Disciplina Read Models
    public async Task<IEnumerable<DisciplinaReadDto>> ObterDisciplinasAsync(Guid escolaId)
    {
        return await _context.Set<DisciplinaEntity>()
            .Where(d => d.EscolaId == escolaId && d.Ativa)
            .Select(d => new DisciplinaReadDto(
                d.Id,
                d.Nome,
                d.Codigo,
                d.CargaHoraria,
                ObterDescricaoSerie(d.TipoSerie, d.AnoSerie),
                d.Obrigatoria,
                d.Descricao,
                d.EscolaId,
                d.Ativa,
                d.PreRequisitos.Select(pr => pr.PreRequisitoId).ToList(),
                d.CreatedAt
            ))
            .OrderBy(d => d.Serie)
            .ThenBy(d => d.Nome)
            .ToListAsync();
    }

    public async Task<DisciplinaReadDto?> ObterDisciplinaDetalheAsync(Guid disciplinaId)
    {
        return await _context.Set<DisciplinaEntity>()
            .Where(d => d.Id == disciplinaId)
            .Select(d => new DisciplinaReadDto(
                d.Id,
                d.Nome,
                d.Codigo,
                d.CargaHoraria,
                ObterDescricaoSerie(d.TipoSerie, d.AnoSerie),
                d.Obrigatoria,
                d.Descricao,
                d.EscolaId,
                d.Ativa,
                d.PreRequisitos.Select(pr => pr.PreRequisitoId).ToList(),
                d.CreatedAt
            ))
            .FirstOrDefaultAsync();
    }

    // Horario Read Models
    public async Task<IEnumerable<HorarioReadDto>> ObterHorariosAsync(Guid turmaId, int anoLetivo, int semestre)
    {
        return await _context.Set<HorarioEntity>()
            .Where(h => h.TurmaId == turmaId && 
                       h.AnoLetivo == anoLetivo && 
                       h.Semestre == semestre && 
                       h.Ativo)
            .Select(h => new HorarioReadDto(
                h.Id,
                h.TurmaId,
                h.DisciplinaId,
                h.ProfessorId,
                ObterDescricaoDiaSemana(h.DiaSemana),
                TimeSpan.FromTicks(h.HoraInicio.Ticks),
                TimeSpan.FromTicks(h.HoraFim.Ticks),
                h.Sala,
                h.AnoLetivo,
                h.Semestre,
                h.Ativo,
                h.CreatedAt
            ))
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToListAsync();
    }

    public async Task<GradeHorariaReadDto> ObterGradeHorariaAsync(Guid turmaId, int anoLetivo, int semestre)
    {
        var turma = await _context.Set<TurmaEntity>()
            .Where(t => t.Id == turmaId)
            .Select(t => new { t.Nome, t.EscolaId })
            .FirstOrDefaultAsync();

        if (turma == null)
            return new GradeHorariaReadDto();

        var horarios = await ObterHorariosAsync(turmaId, anoLetivo, semestre);
        
        // Group horarios by day of week
        var horariosPorDia = horarios.GroupBy(h => Enum.Parse<DayOfWeek>(h.DiaSemana))
            .ToDictionary(g => g.Key, g => g.ToList());

        return new GradeHorariaReadDto
        {
            TurmaId = turmaId,
            NomeTurma = turma.Nome,
            Horarios = horariosPorDia
        };
    }

    public async Task<IEnumerable<HorarioReadDto>> ObterHorariosProfessorAsync(Guid professorId, int anoLetivo, int semestre)
    {
        return await _context.Set<HorarioEntity>()
            .Where(h => h.ProfessorId == professorId && 
                       h.AnoLetivo == anoLetivo && 
                       h.Semestre == semestre && 
                       h.Ativo)
            .Select(h => new HorarioReadDto(
                h.Id,
                h.TurmaId,
                h.DisciplinaId,
                h.ProfessorId,
                ObterDescricaoDiaSemana(h.DiaSemana),
                TimeSpan.FromTicks(h.HoraInicio.Ticks),
                TimeSpan.FromTicks(h.HoraFim.Ticks),
                h.Sala,
                h.AnoLetivo,
                h.Semestre,
                h.Ativo,
                h.CreatedAt
            ))
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToListAsync();
    }

    // Métodos auxiliares para conversão de enums
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

    private static string ObterDescricaoDiaSemana(int diaSemana)
    {
        return diaSemana switch
        {
            1 => "Segunda-feira",
            2 => "Terça-feira",
            3 => "Quarta-feira",
            4 => "Quinta-feira",
            5 => "Sexta-feira",
            6 => "Sábado",
            0 => "Domingo",
            _ => "Dia Desconhecido"
        };
    }

    // Métodos de estatísticas e relatórios
    public async Task<Dictionary<string, int>> ObterEstatisticasTurmasAsync(Guid escolaId)
    {
        var turmas = await _context.Set<TurmaEntity>()
            .Where(t => t.EscolaId == escolaId && t.Ativa)
            .ToListAsync();

        return new Dictionary<string, int>
        {
            ["Total"] = turmas.Count,
            ["Matutino"] = turmas.Count(t => t.TipoTurno == 1),
            ["Vespertino"] = turmas.Count(t => t.TipoTurno == 2),
            ["Noturno"] = turmas.Count(t => t.TipoTurno == 3),
            ["Integral"] = turmas.Count(t => t.TipoTurno == 4),
            ["Infantil"] = turmas.Count(t => t.TipoSerie == 1),
            ["Fundamental"] = turmas.Count(t => t.TipoSerie == 2),
            ["Médio"] = turmas.Count(t => t.TipoSerie == 3)
        };
    }

    public async Task<Dictionary<string, int>> ObterEstatisticasDisciplinasAsync(Guid escolaId)
    {
        var disciplinas = await _context.Set<DisciplinaEntity>()
            .Where(d => d.EscolaId == escolaId && d.Ativa)
            .ToListAsync();

        return new Dictionary<string, int>
        {
            ["Total"] = disciplinas.Count,
            ["Obrigatórias"] = disciplinas.Count(d => d.Obrigatoria),
            ["Optativas"] = disciplinas.Count(d => !d.Obrigatoria),
            ["Infantil"] = disciplinas.Count(d => d.TipoSerie == 1),
            ["Fundamental"] = disciplinas.Count(d => d.TipoSerie == 2),
            ["Médio"] = disciplinas.Count(d => d.TipoSerie == 3)
        };
    }

    public async Task<int> ContarHorariosAtivosAsync(Guid escolaId, int anoLetivo, int semestre)
    {
        return await _context.Set<HorarioEntity>()
            .Join(_context.Set<TurmaEntity>(),
                  h => h.TurmaId,
                  t => t.Id,
                  (h, t) => new { Horario = h, Turma = t })
            .CountAsync(ht => ht.Turma.EscolaId == escolaId && 
                             ht.Horario.AnoLetivo == anoLetivo && 
                             ht.Horario.Semestre == semestre && 
                             ht.Horario.Ativo);
    }
}