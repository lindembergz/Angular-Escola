using MediatR;
using SistemaGestaoEscolar.Professores.Dominio.ObjetosDeValor;

namespace SistemaGestaoEscolar.Professores.Aplicacao.Commands;

public record CriarProfessorCommand(
    string Nome,
    string Cpf,
    string Registro,
    DateTime DataNascimento,
    DateTime DataContratacao,
    Guid EscolaId,
    string? Email = null,
    string? Telefone = null,
    string? Observacoes = null,
    List<CriarTituloAcademicoDto>? Titulos = null) : IRequest<Guid>;

public record CriarTituloAcademicoDto(
    TipoTitulo Tipo,
    string Curso,
    string Instituicao,
    int AnoFormatura);