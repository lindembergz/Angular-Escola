
using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;
using SistemaGestaoEscolar.Academico.Aplicacao.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Academico.Aplicacao.QueryHandlers
{
    public class ObterGradeHorariaQueryHandler : IRequestHandler<ObterGradeHorariaQuery, GradeHorariaReadDto>
    {
        private readonly IRepositorioHorario _repositorioHorario;
        private readonly IRepositorioTurma _repositorioTurma;
        // Faltaria IRepositorioProfessor e IRepositorioDisciplina para obter os nomes

        public ObterGradeHorariaQueryHandler(IRepositorioHorario repositorioHorario, IRepositorioTurma repositorioTurma)
        {
            _repositorioHorario = repositorioHorario;
            _repositorioTurma = repositorioTurma;
        }

        public async Task<GradeHorariaReadDto> Handle(ObterGradeHorariaQuery request, CancellationToken cancellationToken)
        {
            var turma = await _repositorioTurma.ObterPorIdAsync(request.TurmaId);
            var horarios = await _repositorioHorario.ObterPorTurmaAsync(request.TurmaId);

            if (turma == null)
            {
                return new GradeHorariaReadDto
                {
                    TurmaId = request.TurmaId,
                    NomeTurma = "Turma não encontrada",
                    Horarios = new Dictionary<DayOfWeek, List<HorarioReadDto>>()
                };
            }

            var grade = new GradeHorariaReadDto
            {
                TurmaId = turma.Id,
                NomeTurma = turma.Nome.Valor
            };

            var horariosPorDia = horarios
                .GroupBy(h => h.SlotTempo.DiaSemana)
                .ToDictionary(g => g.Key, g => g.Select(h => new HorarioReadDto(
                    h.Id,
                    h.TurmaId,
                    h.DisciplinaId,
                    h.ProfessorId,
                    h.SlotTempo.ObterDescricaoDiaSemana(),
                    TimeSpan.FromTicks(h.SlotTempo.HorarioInicio.Ticks),
                    TimeSpan.FromTicks(h.SlotTempo.HorarioFim.Ticks),
                    h.Sala,
                    h.AnoLetivo,
                    h.Semestre,
                    h.Ativo,
                    h.CreatedAt
                )).ToList());

            grade.Horarios = horariosPorDia;

            return grade;
        }
    }
}
