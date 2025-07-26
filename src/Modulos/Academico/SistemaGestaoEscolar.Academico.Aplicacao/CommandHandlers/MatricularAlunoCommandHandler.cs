
using MediatR;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;
using SistemaGestaoEscolar.Academico.Dominio.Servicos;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaGestaoEscolar.Academico.Aplicacao.CommandHandlers
{
    public class MatricularAlunoCommandHandler : IRequestHandler<Commands.MatricularAlunoCommand, bool>
    {
        private readonly IRepositorioTurma _repositorioTurma;
        private readonly IServicosDominioTurma _servicosDominioTurma;

        public MatricularAlunoCommandHandler(IRepositorioTurma repositorioTurma, IServicosDominioTurma servicosDominioTurma)
        {
            _repositorioTurma = repositorioTurma;
            _servicosDominioTurma = servicosDominioTurma;
        }

        public async Task<bool> Handle(Commands.MatricularAlunoCommand request, CancellationToken cancellationToken)
        {
            var turma = await _repositorioTurma.ObterPorIdAsync(request.TurmaId);
            // Em um cenário real, o Aluno seria obtido de seu próprio módulo/repositório
            // Aqui, estamos apenas passando o ID.
            
            turma.MatricularAluno(request.AlunoId);//, _servicosDominioTurma

            await _repositorioTurma.AtualizarAsync(turma);

            return true;
        }
    }
}
