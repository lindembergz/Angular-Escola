using MediatR;
using SistemaGestaoEscolar.Professores.Aplicacao.Commands;
using SistemaGestaoEscolar.Professores.Dominio.Repositorios;
using SistemaGestaoEscolar.Professores.Dominio.Servicos;

namespace SistemaGestaoEscolar.Professores.Aplicacao.CommandHandlers;

public class DesativarProfessorCommandHandler : IRequestHandler<DesativarProfessorCommand, Unit>
{
    private readonly IRepositorioProfessor _repositorio;
    private readonly IServicosDominioProfessor _servicosDominio;

    public DesativarProfessorCommandHandler(
        IRepositorioProfessor repositorio,
        IServicosDominioProfessor servicosDominio)
    {
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        _servicosDominio = servicosDominio ?? throw new ArgumentNullException(nameof(servicosDominio));
    }

    public async Task<Unit> Handle(DesativarProfessorCommand request, CancellationToken cancellationToken)
    {
        var professor = await _repositorio.ObterPorIdAsync(request.Id);
        if (professor == null)
            throw new InvalidOperationException("Professor não encontrado");

        // Validar se pode ser desativado
        var podeDesativar = await _servicosDominio.ProfessorPodeSerDesativadoAsync(request.Id);
        if (!podeDesativar)
            throw new InvalidOperationException("Professor não pode ser desativado pois possui disciplinas ativas");

        professor.Desativar(request.Motivo);

        await _repositorio.AtualizarAsync(professor);
        return Unit.Value;
    }
}