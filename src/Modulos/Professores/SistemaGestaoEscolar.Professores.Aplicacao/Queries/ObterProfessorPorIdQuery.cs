using MediatR;

namespace SistemaGestaoEscolar.Professores.Aplicacao.Queries;

public record ObterProfessorPorIdQuery(Guid Id) : IRequest<ProfessorDto?>;

public record ProfessorDto(
    Guid Id,
    string Nome,
    string Cpf,
    string Registro,
    string? Email,
    string? Telefone,
    DateTime DataNascimento,
    DateTime DataContratacao,
    Guid EscolaId,
    bool Ativo,
    DateTime DataCadastro,
    string? Observacoes,
    int Idade,
    int TempoServico,
    int CargaHorariaTotal,
    List<TituloAcademicoQueryDto> Titulos,
    List<DisciplinaDto> Disciplinas);

public record DisciplinaDto(
    Guid Id,
    Guid DisciplinaId,
    int CargaHorariaSemanal,
    DateTime DataAtribuicao,
    bool Ativa);

public record TituloAcademicoQueryDto(
    string Tipo,
    string Curso,
    string Instituicao,
    int AnoFormatura,
    string Descricao);