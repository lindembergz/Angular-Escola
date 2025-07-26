using MediatR;
using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands;

public class AtualizarTurmaCommand : IRequest
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int CapacidadeMaxima { get; set; }
}