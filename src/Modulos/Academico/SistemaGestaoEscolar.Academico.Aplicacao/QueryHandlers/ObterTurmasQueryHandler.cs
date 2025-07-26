
using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;
using SistemaGestaoEscolar.Academico.Aplicacao.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
// Usando um repositório de leitura fictício por enquanto
// Em uma implementação real, isso viria da camada de Infraestrutura
using SistemaGestaoEscolar.Academico.Dominio.Repositorios; 
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Academico.Aplicacao.QueryHandlers
{
    public class ObterTurmasQueryHandler : IRequestHandler<ObterTurmasQuery, IEnumerable<TurmaResumoReadDto>>
    {
        private readonly IRepositorioTurma _repositorioTurma; // Simulação, idealmente seria um IReadRepository

        public ObterTurmasQueryHandler(IRepositorioTurma repositorioTurma)
        {
            _repositorioTurma = repositorioTurma;
        }

        public async Task<IEnumerable<TurmaResumoReadDto>> Handle(ObterTurmasQuery request, CancellationToken cancellationToken)
        {
            var turmas = await _repositorioTurma.ObterTodasPorUnidadeEscolarAsync(request.UnidadeEscolarId);

            return turmas.Select(t => new TurmaResumoReadDto
            {
                Id = t.Id,
                Nome = t.Nome.Valor,
                Serie = t.Serie.Ano,
                Turno = t.Turno.Descricao,
                AlunosMatriculados = t.AlunosMatriculados.Count,
                CapacidadeMaxima = t.CapacidadeMaxima
            });
        }
    }
}
