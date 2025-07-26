using MediatR;
using SistemaGestaoEscolar.Professores.Aplicacao.Queries;
using SistemaGestaoEscolar.Professores.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Professores.Aplicacao.QueryHandlers;

public class ObterProfessorPorIdQueryHandler : IRequestHandler<ObterProfessorPorIdQuery, ProfessorDto?>
{
    private readonly IRepositorioProfessor _repositorio;

    public ObterProfessorPorIdQueryHandler(IRepositorioProfessor repositorio)
    {
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public async Task<ProfessorDto?> Handle(ObterProfessorPorIdQuery request, CancellationToken cancellationToken)
    {
        var professor = await _repositorio.ObterPorIdAsync(request.Id);
        if (professor == null)
            return null;

        return new ProfessorDto(
            professor.Id,
            professor.Nome.Valor,
            professor.Cpf.NumeroFormatado,
            professor.Registro.NumeroFormatado,
            professor.Email,
            professor.Telefone,
            professor.DataNascimento,
            professor.DataContratacao,
            professor.EscolaId,
            professor.Ativo,
            professor.DataCadastro,
            professor.Observacoes,
            professor.ObterIdade(),
            professor.ObterTempoServico(),
            professor.ObterCargaHorariaTotal(),
            professor.Titulos.Select(t => new Queries.TituloAcademicoQueryDto(
                t.Tipo.ToString(),
                t.Curso,
                t.Instituicao,
                t.AnoFormatura,
                t.Descricao)).ToList(),
            professor.Disciplinas.Select(d => new Queries.DisciplinaDto(
                d.Id,
                d.DisciplinaId,
                d.CargaHorariaSemanal,
                d.DataAtribuicao,
                d.Ativa)).ToList());
    }
}