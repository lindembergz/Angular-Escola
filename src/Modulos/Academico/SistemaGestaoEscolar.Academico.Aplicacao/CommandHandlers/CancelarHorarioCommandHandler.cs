using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.Commands;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Academico.Aplicacao.CommandHandlers;

public class CancelarHorarioCommandHandler : IRequestHandler<CancelarHorarioCommand>
{
    private readonly IRepositorioHorario _repositorioHorario;

    public CancelarHorarioCommandHandler(IRepositorioHorario repositorioHorario)
    {
        _repositorioHorario = repositorioHorario;
    }

    public async Task Handle(CancelarHorarioCommand request, CancellationToken cancellationToken)
    {
        var horario = await _repositorioHorario.ObterPorIdAsync(request.Id);
        
        if (horario == null)
            throw new InvalidOperationException("Horário não encontrado");

        horario.Cancelar();

        await _repositorioHorario.UpdateAsync(horario);
        await _repositorioHorario.SaveChangesAsync();
    }
}