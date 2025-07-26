using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.Commands;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Academico.Aplicacao.CommandHandlers;

public class AtualizarHorarioCommandHandler : IRequestHandler<AtualizarHorarioCommand>
{
    private readonly IRepositorioHorario _repositorioHorario;

    public AtualizarHorarioCommandHandler(IRepositorioHorario repositorioHorario)
    {
        _repositorioHorario = repositorioHorario;
    }

    public async Task Handle(AtualizarHorarioCommand request, CancellationToken cancellationToken)
    {
        var horario = await _repositorioHorario.ObterPorIdAsync(request.Id);
        
        if (horario == null)
            throw new InvalidOperationException("Horário não encontrado");

        horario.AtualizarProfessor(request.ProfessorId);
        
        if (!string.IsNullOrEmpty(request.Sala))
            horario.AtualizarSala(request.Sala);

        await _repositorioHorario.UpdateAsync(horario);
        await _repositorioHorario.SaveChangesAsync();
    }
}