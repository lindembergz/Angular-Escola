using MediatR;

namespace SistemaGestaoEscolar.Alunos.Aplicacao.Commands;

public class CriarAlunoCommand : IRequest<CriarAlunoResponse>
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public int? Genero { get; set; } // TipoGenero (opcional, padrão: NaoInformado)
    public int? TipoDeficiencia { get; set; } // TipoDeficiencia (opcional)
    public string? DescricaoDeficiencia { get; set; } // Descrição das adaptações necessárias
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Observacoes { get; set; }
    public Guid EscolaId { get; set; }
    public List<CriarResponsavelDto> Responsaveis { get; set; } = new();
}

public class CriarResponsavelDto
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int Tipo { get; set; } // TipoResponsavel
    public string? Profissao { get; set; }
    public string? LocalTrabalho { get; set; }
    public string? TelefoneTrabalho { get; set; }
    public bool ResponsavelFinanceiro { get; set; } = true;
    public bool ResponsavelAcademico { get; set; } = true;
    public bool AutorizadoBuscar { get; set; } = true;
    public string? Observacoes { get; set; }
    
    // Endereço do responsável (opcional, pode ser diferente do aluno)
    public string? Logradouro { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Cep { get; set; }
}

public class CriarAlunoResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string NumeroMatricula { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
    public List<string> Responsaveis { get; set; } = new();
    public bool Sucesso { get; set; }
    public List<string> Erros { get; set; } = new();
}