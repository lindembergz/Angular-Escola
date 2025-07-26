using SistemaGestaoEscolar.Academico.Dominio.Entidades;
using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Academico.Dominio.Servicos;

public class ServicosDominioHorario : IServicosDominioHorario
{
    private readonly IRepositorioHorario _repositorioHorario;
    private readonly IRepositorioDisciplina _repositorioDisciplina;

    public ServicosDominioHorario(IRepositorioHorario repositorioHorario, 
                                 IRepositorioDisciplina repositorioDisciplina)
    {
        _repositorioHorario = repositorioHorario;
        _repositorioDisciplina = repositorioDisciplina;
    }

    public async Task<bool> PodeCriarHorarioAsync(Guid turmaId, Guid disciplinaId, Guid professorId, 
                                                 SlotTempo slotTempo, int anoLetivo, int semestre, 
                                                 string? sala = null)
    {
        var conflitoProfessor = await _repositorioHorario.ExisteConflitoProfessorAsync(
            professorId, slotTempo.DiaSemana, slotTempo.HorarioInicio, slotTempo.HorarioFim, anoLetivo, semestre);

        if (conflitoProfessor)
            return false;

        if (!string.IsNullOrEmpty(sala))
        {
            var conflitoSala = await _repositorioHorario.ExisteConflitoSalaAsync(
                sala, slotTempo.DiaSemana, slotTempo.HorarioInicio, slotTempo.HorarioFim, anoLetivo, semestre);
            
            if (conflitoSala)
                return false;
        }

        return await ValidarCargaHorariaDisciplinaAsync(disciplinaId, turmaId, anoLetivo, semestre) &&
               await ValidarLimiteCargaHorariaProfessorAsync(professorId, slotTempo, anoLetivo, semestre);
    }

    public async Task<IEnumerable<SlotTempo>> ObterHorariosDisponiveisProfessorAsync(Guid professorId, 
                                                                                   DayOfWeek diaSemana, 
                                                                                   int anoLetivo, int semestre)
    {
        var horariosOcupados = await _repositorioHorario.ObterPorProfessorAsync(professorId);
        var horariosOcupadosDia = horariosOcupados
            .Where(h => h.SlotTempo.DiaSemana == diaSemana && 
                       h.AnoLetivo == anoLetivo && 
                       h.Semestre == semestre && 
                       h.Ativo)
            .Select(h => h.SlotTempo);

        var horariosDisponiveis = new List<SlotTempo>();
        var horarios = ObterHorariosPadraoEscola(diaSemana);

        foreach (var horario in horarios)
        {
            var temConflito = horariosOcupadosDia.Any(ocupado => ocupado.ConflitaCom(horario));
            if (!temConflito)
                horariosDisponiveis.Add(horario);
        }

        return horariosDisponiveis;
    }

    public async Task<IEnumerable<string>> ObterSalasDisponiveisAsync(SlotTempo slotTempo, 
                                                                     int anoLetivo, int semestre, 
                                                                     Guid escolaId)
    {
        var todasSalas = ObterSalasPadraoEscola(); // TODO: Buscar do repositório de escolas
        var salasOcupadas = new List<string>();

        foreach (var sala in todasSalas)
        {
            var ocupada = await _repositorioHorario.ExisteConflitoSalaAsync(
                sala, slotTempo.DiaSemana, slotTempo.HorarioInicio, slotTempo.HorarioFim, anoLetivo, semestre);
            
            if (ocupada)
                salasOcupadas.Add(sala);
        }

        return todasSalas.Except(salasOcupadas);
    }

    public async Task<bool> ValidarCargaHorariaDisciplinaAsync(Guid disciplinaId, Guid turmaId, 
                                                              int anoLetivo, int semestre)
    {
        var disciplina = await _repositorioDisciplina.ObterPorIdAsync(disciplinaId);
        if (disciplina == null)
            return false;

        var horariosExistentes = await _repositorioHorario.ObterPorDisciplinaAsync(disciplinaId);
        var cargaHorariaAtual = horariosExistentes
            .Where(h => h.TurmaId == turmaId && 
                       h.AnoLetivo == anoLetivo && 
                       h.Semestre == semestre && 
                       h.Ativo)
            .Sum(h => h.SlotTempo.DuracaoMinutos);

        var cargaHorariaSemestral = disciplina.CargaHoraria * 60 / 2; // Dividir por 2 semestres
        return cargaHorariaAtual < cargaHorariaSemestral;
    }

    public async Task<bool> ValidarLimiteCargaHorariaProfessorAsync(Guid professorId, SlotTempo novoSlot, 
                                                                  int anoLetivo, int semestre)
    {
        var cargaAtual = await CalcularCargaHorariaSemanalProfessorAsync(professorId, anoLetivo, semestre);
        var novaCarga = cargaAtual.Add(TimeSpan.FromMinutes(novoSlot.DuracaoMinutos));

        // Limite máximo de 40 horas semanais
        return novaCarga.TotalHours <= 40;
    }

    public async Task<IEnumerable<Horario>> DetectarConflitosAsync(Horario horario)
    {
        var todosHorarios = await _repositorioHorario.ObterPorPeriodoAsync(
            horario.AnoLetivo, horario.Semestre);

        return todosHorarios.Where(h => h.Id != horario.Id && h.ConflitaCom(horario));
    }

    public async Task<bool> PodeAlterarHorarioAsync(Guid horarioId, SlotTempo novoSlotTempo)
    {
        var horario = await _repositorioHorario.ObterPorIdAsync(horarioId);
        if (horario == null || !horario.Ativo)
            return false;

        return !await _repositorioHorario.ExisteConflitoProfessorOuSalaAsync(
            horario.ProfessorId, horario.Sala, novoSlotTempo.DiaSemana, 
            novoSlotTempo.HorarioInicio, novoSlotTempo.HorarioFim, 
            horario.AnoLetivo, horario.Semestre, horarioId);
    }

    public async Task<TimeSpan> CalcularCargaHorariaSemanalProfessorAsync(Guid professorId, 
                                                                         int anoLetivo, int semestre)
    {
        var horarios = await _repositorioHorario.ObterPorProfessorAsync(professorId);
        var horariosAtivos = horarios.Where(h => 
            h.AnoLetivo == anoLetivo && 
            h.Semestre == semestre && 
            h.Ativo);

        var totalMinutos = horariosAtivos.Sum(h => h.SlotTempo.DuracaoMinutos);
        return TimeSpan.FromMinutes(totalMinutos);
    }

    private static IEnumerable<SlotTempo> ObterHorariosPadraoEscola(DayOfWeek diaSemana)
    {
        var horarios = new List<SlotTempo>();
        
        // Horários matutinos
        horarios.Add(SlotTempo.Criar(new TimeOnly(7, 0), new TimeOnly(7, 50), diaSemana));
        horarios.Add(SlotTempo.Criar(new TimeOnly(7, 50), new TimeOnly(8, 40), diaSemana));
        horarios.Add(SlotTempo.Criar(new TimeOnly(9, 0), new TimeOnly(9, 50), diaSemana));
        horarios.Add(SlotTempo.Criar(new TimeOnly(9, 50), new TimeOnly(10, 40), diaSemana));
        horarios.Add(SlotTempo.Criar(new TimeOnly(11, 0), new TimeOnly(11, 50), diaSemana));

        // Horários vespertinos
        horarios.Add(SlotTempo.Criar(new TimeOnly(13, 0), new TimeOnly(13, 50), diaSemana));
        horarios.Add(SlotTempo.Criar(new TimeOnly(13, 50), new TimeOnly(14, 40), diaSemana));
        horarios.Add(SlotTempo.Criar(new TimeOnly(15, 0), new TimeOnly(15, 50), diaSemana));
        horarios.Add(SlotTempo.Criar(new TimeOnly(15, 50), new TimeOnly(16, 40), diaSemana));
        horarios.Add(SlotTempo.Criar(new TimeOnly(17, 0), new TimeOnly(17, 50), diaSemana));

        return horarios;
    }

    private static IEnumerable<string> ObterSalasPadraoEscola()
    {
        return new[] { "Sala 01", "Sala 02", "Sala 03", "Sala 04", "Sala 05", 
                      "Laboratório", "Biblioteca", "Auditório" };
    }
}