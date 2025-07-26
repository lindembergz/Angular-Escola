using SistemaGestaoEscolar.Shared.Domain.Entities;
using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Academico.Dominio.Eventos;

namespace SistemaGestaoEscolar.Academico.Dominio.Entidades;

public class Horario : AggregateRoot
{
    public Guid TurmaId { get; private set; }
    public Guid DisciplinaId { get; private set; }
    public Guid ProfessorId { get; private set; }
    public SlotTempo SlotTempo { get; private set; }
    public string? Sala { get; private set; }
    public int AnoLetivo { get; private set; }
    public int Semestre { get; private set; }
    public bool Ativo { get; private set; }

    // Navigation properties
    public Turma? Turma { get; private set; }
    public Disciplina? Disciplina { get; private set; }

    private Horario() { } // EF Constructor

    private Horario(Guid turmaId, Guid disciplinaId, Guid professorId, 
                   SlotTempo slotTempo, int anoLetivo, int semestre, string? sala = null)
    {
        TurmaId = turmaId;
        DisciplinaId = disciplinaId;
        ProfessorId = professorId;
        SlotTempo = slotTempo;
        AnoLetivo = anoLetivo;
        Semestre = semestre;
        Sala = sala;
        Ativo = true;
    }

    public static Horario Criar(Guid turmaId, Guid disciplinaId, Guid professorId, 
                               SlotTempo slotTempo, int anoLetivo, int semestre, 
                               string? sala = null)
    {
        if (turmaId == Guid.Empty)
            throw new ArgumentException("ID da turma é obrigatório");

        if (disciplinaId == Guid.Empty)
            throw new ArgumentException("ID da disciplina é obrigatório");

        if (professorId == Guid.Empty)
            throw new ArgumentException("ID do professor é obrigatório");

        if (anoLetivo < DateTime.Now.Year - 1)
            throw new ArgumentException("Ano letivo não pode ser anterior ao ano passado");

        if (semestre < 1 || semestre > 2)
            throw new ArgumentException("Semestre deve ser 1 ou 2");

        if (!string.IsNullOrWhiteSpace(sala) && sala.Length > 50)
            throw new ArgumentException("Nome da sala não pode ter mais de 50 caracteres");

        var horario = new Horario(turmaId, disciplinaId, professorId, slotTempo, 
                                 anoLetivo, semestre, sala?.Trim());

        horario.AddDomainEvent(new HorarioCriadoEvento(horario.Id, turmaId, 
                                                       disciplinaId, professorId, slotTempo));
        
        return horario;
    }

    public void AtualizarSala(string? novaSala)
    {
        if (!string.IsNullOrWhiteSpace(novaSala) && novaSala.Length > 50)
            throw new ArgumentException("Nome da sala não pode ter mais de 50 caracteres");

        Sala = novaSala?.Trim();
        AddDomainEvent(new HorarioAtualizadoEvento(Id, "Sala", novaSala));
    }

    public void AtualizarProfessor(Guid novoProfessorId)
    {
        if (novoProfessorId == Guid.Empty)
            throw new ArgumentException("ID do professor é obrigatório");

        if (ProfessorId == novoProfessorId)
            return;

        var professorAnterior = ProfessorId;
        ProfessorId = novoProfessorId;
        
        AddDomainEvent(new ProfessorAlteradoNoHorarioEvento(Id, professorAnterior, novoProfessorId));
    }

    public void AtualizarSlotTempo(SlotTempo novoSlotTempo)
    {
        var slotAnterior = SlotTempo;
        SlotTempo = novoSlotTempo;
        
        AddDomainEvent(new SlotTempoAlteradoEvento(Id, slotAnterior, novoSlotTempo));
    }

    public void Cancelar()
    {
        if (!Ativo)
            throw new InvalidOperationException("Horário já está cancelado");

        Ativo = false;
        AddDomainEvent(new HorarioCanceladoEvento(Id, TurmaId, DisciplinaId, SlotTempo));
    }

    public void Reativar()
    {
        if (Ativo)
            throw new InvalidOperationException("Horário já está ativo");

        Ativo = true;
        AddDomainEvent(new HorarioReativadoEvento(Id, TurmaId, DisciplinaId, SlotTempo));
    }

    public bool ConflitaCom(Horario outroHorario)
    {
        if (!Ativo || !outroHorario.Ativo)
            return false;

        if (AnoLetivo != outroHorario.AnoLetivo || Semestre != outroHorario.Semestre)
            return false;

        // Conflito se mesmo professor ou mesma sala no mesmo horário
        var mesmoRecurso = ProfessorId == outroHorario.ProfessorId ||
                          (!string.IsNullOrEmpty(Sala) && !string.IsNullOrEmpty(outroHorario.Sala) && 
                           Sala.Equals(outroHorario.Sala, StringComparison.OrdinalIgnoreCase));

        return mesmoRecurso && SlotTempo.ConflitaCom(outroHorario.SlotTempo);
    }

    public string ObterDescricaoCompleta()
    {
        var descricao = $"{SlotTempo}";
        if (!string.IsNullOrEmpty(Sala))
            descricao += $" - Sala: {Sala}";
        return descricao;
    }
}