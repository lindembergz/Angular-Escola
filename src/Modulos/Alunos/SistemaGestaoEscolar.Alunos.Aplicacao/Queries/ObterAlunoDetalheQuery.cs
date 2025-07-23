using MediatR;

namespace SistemaGestaoEscolar.Alunos.Aplicacao.Queries;

public class ObterAlunoDetalheQuery : IRequest<ObterAlunoDetalheResponse>
{
    public Guid Id { get; set; }
    public bool IncluirResponsaveis { get; set; } = true;
    public bool IncluirMatriculas { get; set; } = true;
}

public class ObterAlunoDetalheResponse
{
    public AlunoDetalheDto? Aluno { get; set; }
    public bool Sucesso { get; set; }
    public List<string> Erros { get; set; } = new();
}

public class AlunoDetalheDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public int Idade { get; set; }
    public string FaixaEtariaEscolar { get; set; } = string.Empty;
    public EnderecoDto Endereco { get; set; } = new();
    public GeneroDto Genero { get; set; } = new();
    public DeficienciaDto Deficiencia { get; set; } = new();
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Observacoes { get; set; }
    public Guid EscolaId { get; set; }
    public string NomeEscola { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
    public List<ResponsavelDto> Responsaveis { get; set; } = new();
    public List<MatriculaDto> Matriculas { get; set; } = new();
    public MatriculaDto? MatriculaAtiva { get; set; }
    public List<string> Pendencias { get; set; } = new();
}

public class EnderecoDto
{
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string CepFormatado { get; set; } = string.Empty;
    public string EnderecoCompleto { get; set; } = string.Empty;
}

public class ResponsavelDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string TipoDescricao { get; set; } = string.Empty;
    public string? Profissao { get; set; }
    public string? LocalTrabalho { get; set; }
    public string? TelefoneTrabalho { get; set; }
    public bool ResponsavelFinanceiro { get; set; }
    public bool ResponsavelAcademico { get; set; }
    public bool AutorizadoBuscar { get; set; }
    public EnderecoDto? Endereco { get; set; }
}

public class MatriculaDto
{
    public Guid Id { get; set; }
    public Guid TurmaId { get; set; }
    public string NomeTurma { get; set; } = string.Empty;
    public int AnoLetivo { get; set; }
    public string NumeroMatricula { get; set; } = string.Empty;
    public DateTime DataMatricula { get; set; }
    public DateTime? DataCancelamento { get; set; }
    public string? MotivoCancelamento { get; set; }
    public bool Ativa { get; set; }
    public string StatusDescricao { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    public int DiasMatriculado { get; set; }
}

public class GeneroDto
{
    public int Tipo { get; set; }
    public string Descricao { get; set; } = string.Empty;
}

public class DeficienciaDto
{
    public int? Tipo { get; set; }
    public string? TipoDescricao { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool PossuiDeficiencia { get; set; }
}