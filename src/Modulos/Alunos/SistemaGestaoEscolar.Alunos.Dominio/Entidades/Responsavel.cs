using SistemaGestaoEscolar.Alunos.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Shared.Domain.Entities;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Alunos.Dominio.Entidades;

public class Responsavel : BaseEntity
{
    public new Guid Id { get; private set; }
    public NomeAluno Nome { get; private set; } = null!;
    public CPF Cpf { get; private set; } = null!;
    public string Telefone { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public Endereco? Endereco { get; private set; }
    public TipoResponsavel Tipo { get; private set; }
    public string? Profissao { get; private set; }
    public string? LocalTrabalho { get; private set; }
    public string? TelefoneTrabalho { get; private set; }
    public bool ResponsavelFinanceiro { get; private set; }
    public bool ResponsavelAcademico { get; private set; }
    public bool AutorizadoBuscar { get; private set; }
    public string? Observacoes { get; private set; }

    private Responsavel() { } // Para EF Core

    public Responsavel(
        NomeAluno nome,
        CPF cpf,
        string telefone,
        TipoResponsavel tipo,
        string? email = null,
        Endereco? endereco = null,
        string? profissao = null,
        string? localTrabalho = null,
        string? telefoneTrabalho = null,
        bool responsavelFinanceiro = true,
        bool responsavelAcademico = true,
        bool autorizadoBuscar = true,
        string? observacoes = null)
    {
        Id = Guid.NewGuid();
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Cpf = cpf ?? throw new ArgumentNullException(nameof(cpf));
        Telefone = !string.IsNullOrWhiteSpace(telefone) ? telefone.Trim() : throw new ArgumentException("Telefone é obrigatório");
        Tipo = tipo;
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLower();
        Endereco = endereco;
        Profissao = string.IsNullOrWhiteSpace(profissao) ? null : profissao.Trim();
        LocalTrabalho = string.IsNullOrWhiteSpace(localTrabalho) ? null : localTrabalho.Trim();
        TelefoneTrabalho = string.IsNullOrWhiteSpace(telefoneTrabalho) ? null : telefoneTrabalho.Trim();
        ResponsavelFinanceiro = responsavelFinanceiro;
        ResponsavelAcademico = responsavelAcademico;
        AutorizadoBuscar = autorizadoBuscar;
        Observacoes = string.IsNullOrWhiteSpace(observacoes) ? null : observacoes.Trim();

        ValidarEmail();
        ValidarTelefone();
    }

    public void AtualizarNome(NomeAluno novoNome)
    {
        Nome = novoNome ?? throw new ArgumentNullException(nameof(novoNome));
        MarkAsUpdated();
    }

    public void AtualizarTelefone(string novoTelefone)
    {
        if (string.IsNullOrWhiteSpace(novoTelefone))
            throw new ArgumentException("Telefone é obrigatório");

        Telefone = novoTelefone.Trim();
        ValidarTelefone();
        MarkAsUpdated();
    }

    public void AtualizarEmail(string? novoEmail)
    {
        Email = string.IsNullOrWhiteSpace(novoEmail) ? null : novoEmail.Trim().ToLower();
        ValidarEmail();
        MarkAsUpdated();
    }

    public void AtualizarEndereco(Endereco? novoEndereco)
    {
        Endereco = novoEndereco;
        MarkAsUpdated();
    }

    public void AtualizarProfissao(string? novaProfissao, string? novoLocalTrabalho = null, string? novoTelefoneTrabalho = null)
    {
        Profissao = string.IsNullOrWhiteSpace(novaProfissao) ? null : novaProfissao.Trim();
        LocalTrabalho = string.IsNullOrWhiteSpace(novoLocalTrabalho) ? null : novoLocalTrabalho.Trim();
        TelefoneTrabalho = string.IsNullOrWhiteSpace(novoTelefoneTrabalho) ? null : novoTelefoneTrabalho.Trim();
        MarkAsUpdated();
    }

    public void DefinirResponsabilidades(
        bool responsavelFinanceiro,
        bool responsavelAcademico,
        bool autorizadoBuscar)
    {
        ResponsavelFinanceiro = responsavelFinanceiro;
        ResponsavelAcademico = responsavelAcademico;
        AutorizadoBuscar = autorizadoBuscar;
        MarkAsUpdated();
    }

    public void AtualizarObservacoes(string? novasObservacoes)
    {
        Observacoes = string.IsNullOrWhiteSpace(novasObservacoes) ? null : novasObservacoes.Trim();
        MarkAsUpdated();
    }

    public bool PodeReceberCobrancas()
    {
        return ResponsavelFinanceiro && !string.IsNullOrEmpty(Email);
    }

    public bool PodeReceberBoletins()
    {
        return ResponsavelAcademico && !string.IsNullOrEmpty(Email);
    }

    public bool PodeBuscarAluno()
    {
        return AutorizadoBuscar;
    }

    public string ObterContatoPrincipal()
    {
        return Telefone;
    }

    public string? ObterContatoSecundario()
    {
        return TelefoneTrabalho;
    }

    public string ObterDescricaoCompleta()
    {
        var descricao = $"{Nome.Valor} ({Tipo.ObterDescricao()})";
        
        if (!string.IsNullOrEmpty(Profissao))
            descricao += $" - {Profissao}";
            
        return descricao;
    }

    private void ValidarEmail()
    {
        if (!string.IsNullOrEmpty(Email))
        {
            var emailRegex = new System.Text.RegularExpressions.Regex(
                @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            
            if (!emailRegex.IsMatch(Email))
                throw new ArgumentException("Email inválido");
        }
    }

    private void ValidarTelefone()
    {
        // Remover caracteres não numéricos para validação
        var telefoneNumeros = System.Text.RegularExpressions.Regex.Replace(Telefone, @"[^\d]", "");
        
        // Validar se tem pelo menos 10 dígitos (telefone fixo) ou 11 (celular)
        if (telefoneNumeros.Length < 10 || telefoneNumeros.Length > 11)
            throw new ArgumentException("Telefone deve ter 10 ou 11 dígitos");
    }
}

public enum TipoResponsavel
{
    Pai = 1,
    Mae = 2,
    Avo = 3,
    Ava = 4,
    Tio = 5,
    Tia = 6,
    Irmao = 7,
    Irma = 8,
    Padrasto = 9,
    Madrasta = 10,
    Tutor = 11,
    Outro = 99
}

public static class TipoResponsavelExtensions
{
    public static string ObterDescricao(this TipoResponsavel tipo)
    {
        return tipo switch
        {
            TipoResponsavel.Pai => "Pai",
            TipoResponsavel.Mae => "Mãe",
            TipoResponsavel.Avo => "Avô",
            TipoResponsavel.Ava => "Avó",
            TipoResponsavel.Tio => "Tio",
            TipoResponsavel.Tia => "Tia",
            TipoResponsavel.Irmao => "Irmão",
            TipoResponsavel.Irma => "Irmã",
            TipoResponsavel.Padrasto => "Padrasto",
            TipoResponsavel.Madrasta => "Madrasta",
            TipoResponsavel.Tutor => "Tutor Legal",
            TipoResponsavel.Outro => "Outro",
            _ => "Não Informado"
        };
    }

    public static bool EhResponsavelLegal(this TipoResponsavel tipo)
    {
        return tipo is TipoResponsavel.Pai or TipoResponsavel.Mae or TipoResponsavel.Tutor;
    }
}