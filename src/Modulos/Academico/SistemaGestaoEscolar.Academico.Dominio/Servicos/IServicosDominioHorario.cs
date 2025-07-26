using SistemaGestaoEscolar.Academico.Dominio.Entidades;
using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;

namespace SistemaGestaoEscolar.Academico.Dominio.Servicos;

public interface IServicosDominioHorario
{
    Task<bool> PodeCriarHorarioAsync(Guid turmaId, Guid disciplinaId, Guid professorId, 
                                    SlotTempo slotTempo, int anoLetivo, int semestre, 
                                    string? sala = null);
    
    Task<IEnumerable<SlotTempo>> ObterHorariosDisponiveisProfessorAsync(Guid professorId, 
                                                                       DayOfWeek diaSemana, 
                                                                       int anoLetivo, int semestre);
    
    Task<IEnumerable<string>> ObterSalasDisponiveisAsync(SlotTempo slotTempo, 
                                                        int anoLetivo, int semestre, 
                                                        Guid escolaId);
    
    Task<bool> ValidarCargaHorariaDisciplinaAsync(Guid disciplinaId, Guid turmaId, 
                                                 int anoLetivo, int semestre);
    
    Task<bool> ValidarLimiteCargaHorariaProfessorAsync(Guid professorId, SlotTempo novoSlot, 
                                                      int anoLetivo, int semestre);
    
    Task<IEnumerable<Horario>> DetectarConflitosAsync(Horario horario);
    
    Task<bool> PodeAlterarHorarioAsync(Guid horarioId, SlotTempo novoSlotTempo);
    
    Task<TimeSpan> CalcularCargaHorariaSemanalProfessorAsync(Guid professorId, 
                                                           int anoLetivo, int semestre);
}