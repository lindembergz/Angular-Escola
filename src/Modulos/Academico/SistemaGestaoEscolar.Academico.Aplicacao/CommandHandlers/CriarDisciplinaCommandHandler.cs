
using MediatR;
using SistemaGestaoEscolar.Academico.Dominio.Entidades;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaGestaoEscolar.Academico.Aplicacao.CommandHandlers
{
    public class CriarDisciplinaCommandHandler : IRequestHandler<Commands.CriarDisciplinaCommand, Guid>
    {
        private readonly IRepositorioDisciplina _repositorioDisciplina;

        public CriarDisciplinaCommandHandler(IRepositorioDisciplina repositorioDisciplina)
        {
            _repositorioDisciplina = repositorioDisciplina;
        }

        public async Task<Guid> Handle(Commands.CriarDisciplinaCommand request, CancellationToken cancellationToken)
        {
            var disciplina = Disciplina.Criar(
                request.Nome,
                request.Codigo,
                request.CargaHoraria,
                Dominio.ObjetosDeValor.Serie.Criar(request.TipoSerie, request.AnoSerie),
                request.Obrigatoria,
                request.EscolaId,
                request.Descricao
            );

            await _repositorioDisciplina.AdicionarAsync(disciplina);

            return disciplina.Id;
        }
    }
}
