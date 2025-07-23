using MediatR;

namespace SistemaGestaoEscolar.Alunos.Aplicacao.Commands;

public class AtualizarAlunoCommand : IRequest<AtualizarAlunoResponse>
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public int? Genero { get; set; } // TipoGenero (opcional)
    public int? TipoDeficiencia { get; set; } // TipoDeficiencia (opcional)
    public string? DescricaoDeficiencia { get; set; } // Descrição das adaptações necessárias
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Observacoes { get; set; }
    public List<CriarResponsavelRequest> Responsaveis { get; set; } = new();
}

public class CriarResponsavelRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int Tipo { get; set; } // TipoResponsavel
    public string? Profissao { get; set; }
    public string? LocalTrabalho { get; set; }
    public string? TelefoneTrabalho { get; set; }
    public bool ResponsavelFinanceiro { get; set; }
    public bool ResponsavelAcademico { get; set; }
    public bool AutorizadoBuscar { get; set; }
    public string? Observacoes { get; set; }
    public string? Logradouro { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Cep { get; set; }
}

public class AtualizarAlunoResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataAtualizacao { get; set; }
    public bool Sucesso { get; set; }
    public List<string> Erros { get; set; } = new();
}