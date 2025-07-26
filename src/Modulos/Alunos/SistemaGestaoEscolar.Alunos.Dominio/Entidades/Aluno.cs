
using SistemaGestaoEscolar.Alunos.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Alunos.Dominio.Eventos;
using SistemaGestaoEscolar.Shared.Domain.Entities;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Alunos.Dominio.Entidades;


public class Aluno : AggregateRoot
{
    public new Guid Id { get; private set; }
    public NomeAluno Nome { get; private set; } = null!;
    public Cpf Cpf { get; private set; } = null!;
    public DataNascimento DataNascimento { get; private set; } = null!;
    public Endereco Endereco { get; private set; } = null!;
    public Genero Genero { get; private set; } = null!;
    public Deficiencia Deficiencia { get; private set; } = null!;
    public string? Telefone { get; private set; }
    public string? Email { get; private set; }
    public string? Observacoes { get; private set; }
    public Guid EscolaId { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public bool Ativo { get; private set; }
    
    private readonly List<Responsavel> _responsaveis = new();
    public IReadOnlyCollection<Responsavel> Responsaveis => _responsaveis.AsReadOnly();
    
    private readonly List<Matricula> _matriculas = new();
    public IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();

    private Aluno() { } // Para EF Core

    public Aluno(
        NomeAluno nome,
        Cpf cpf,
        DataNascimento dataNascimento,
        Endereco endereco,
        Guid escolaId,
        Genero? genero = null,
        Deficiencia? deficiencia = null,
        string? telefone = null,
        string? email = null,
        string? observacoes = null)
    {
        Id = Guid.NewGuid();
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Cpf = cpf ?? throw new ArgumentNullException(nameof(cpf));
        DataNascimento = dataNascimento ?? throw new ArgumentNullException(nameof(dataNascimento));
        Endereco = endereco ?? throw new ArgumentNullException(nameof(endereco));
        Genero = genero ?? Genero.NaoInformado; // Padrão: Não Informado
        Deficiencia = deficiencia ?? Deficiencia.Nenhuma(); // Padrão: Nenhuma deficiência
        EscolaId = escolaId == Guid.Empty ? throw new ArgumentException("EscolaId não pode ser vazio") : escolaId;
        Telefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLower();
        Observacoes = string.IsNullOrWhiteSpace(observacoes) ? null : observacoes.Trim();
        DataCadastro = DateTime.UtcNow;
        Ativo = true;

        ValidarEmail();
        ValidarIdadeEscolar();

        AddDomainEvent(new AlunoCriadoEvento(Id, Nome.Valor, Cpf.Numero, EscolaId));
    }

    public void AtualizarNome(NomeAluno novoNome)
    {
        if (novoNome == null)
            throw new ArgumentNullException(nameof(novoNome));

        var nomeAnterior = Nome;
        Nome = novoNome;

        AddDomainEvent(new AlunoAtualizadoEvento(Id, "Nome", nomeAnterior.Valor, novoNome.Valor));
    }

    public void AtualizarEndereco(Endereco novoEndereco)
    {
        if (novoEndereco == null)
            throw new ArgumentNullException(nameof(novoEndereco));

        var enderecoAnterior = Endereco;
        Endereco = novoEndereco;

        AddDomainEvent(new AlunoAtualizadoEvento(Id, "Endereço", enderecoAnterior.ToString(), novoEndereco.ToString()));
    }

    public void AtualizarTelefone(string? novoTelefone)
    {
        var telefoneAnterior = Telefone;
        Telefone = string.IsNullOrWhiteSpace(novoTelefone) ? null : novoTelefone.Trim();

        AddDomainEvent(new AlunoAtualizadoEvento(Id, "Telefone", telefoneAnterior ?? "", Telefone ?? ""));
    }

    public void AtualizarEmail(string? novoEmail)
    {
        var emailAnterior = Email;
        Email = string.IsNullOrWhiteSpace(novoEmail) ? null : novoEmail.Trim().ToLower();
        
        ValidarEmail();

        AddDomainEvent(new AlunoAtualizadoEvento(Id, "Email", emailAnterior ?? "", Email ?? ""));
    }

    public void AtualizarObservacoes(string? novasObservacoes)
    {
        Observacoes = string.IsNullOrWhiteSpace(novasObservacoes) ? null : novasObservacoes.Trim();
    }

    public void AtualizarGenero(Genero novoGenero)
    {
        if (novoGenero == null)
            throw new ArgumentNullException(nameof(novoGenero));

        var generoAnterior = Genero;
        Genero = novoGenero;

        AddDomainEvent(new AlunoAtualizadoEvento(Id, "Gênero", generoAnterior.ToString(), novoGenero.ToString()));
    }

    public void AtualizarDeficiencia(Deficiencia novaDeficiencia)
    {
        if (novaDeficiencia == null)
            throw new ArgumentNullException(nameof(novaDeficiencia));

        var deficienciaAnterior = Deficiencia;
        Deficiencia = novaDeficiencia;

        AddDomainEvent(new AlunoAtualizadoEvento(Id, "Deficiência", deficienciaAnterior.ToString(), novaDeficiencia.ToString()));
    }

    public void AdicionarResponsavel(Responsavel responsavel)
    {
        if (responsavel == null)
            throw new ArgumentNullException(nameof(responsavel));

        if (_responsaveis.Any(r => r.Cpf.Numero == responsavel.Cpf.Numero))
            throw new InvalidOperationException("Já existe um responsável com este CPF para este aluno");

        if (_responsaveis.Count >= 3)
            throw new InvalidOperationException("Um aluno pode ter no máximo 3 responsáveis");

        _responsaveis.Add(responsavel);
        AddDomainEvent(new ResponsavelAdicionadoEvento(Id, responsavel.Id, responsavel.Nome.Valor));
    }

    public void RemoverResponsavel(Guid responsavelId)
    {
        var responsavel = _responsaveis.FirstOrDefault(r => r.Id == responsavelId);
        if (responsavel == null)
            throw new InvalidOperationException("Responsável não encontrado");

        if (_responsaveis.Count == 1)
            throw new InvalidOperationException("Um aluno deve ter pelo menos um responsável");

        _responsaveis.Remove(responsavel);
        AddDomainEvent(new ResponsavelRemovidoEvento(Id, responsavelId, responsavel.Nome.Valor));
    }

    public void LimparResponsaveis()
    {
        _responsaveis.Clear();
    }

    public void AdicionarMatricula(Matricula matricula)
    {
        if (matricula == null)
            throw new ArgumentNullException(nameof(matricula));

        // Verificar se já existe matrícula ativa para o mesmo ano letivo
        if (_matriculas.Any(m => m.AnoLetivo == matricula.AnoLetivo && m.Ativa))
            throw new InvalidOperationException($"Aluno já possui matrícula ativa para o ano letivo {matricula.AnoLetivo}");

        _matriculas.Add(matricula);
        AddDomainEvent(new AlunoMatriculadoEvento(Id, matricula.Id, matricula.TurmaId, matricula.AnoLetivo));
    }

    public void TransferirEscola(Guid novaEscolaId, string motivo)
    {
        if (novaEscolaId == Guid.Empty)
            throw new ArgumentException("Nova escola ID não pode ser vazio");

        if (novaEscolaId == EscolaId)
            throw new InvalidOperationException("Aluno já está matriculado nesta escola");

        var escolaAnterior = EscolaId;
        EscolaId = novaEscolaId;

        // Inativar matrículas da escola anterior
        foreach (var matricula in _matriculas.Where(m => m.Ativa))
        {
            matricula.Inativar("Transferência de escola");
        }

        AddDomainEvent(new AlunoTransferidoEvento(Id, escolaAnterior, novaEscolaId, motivo));
    }

    public void Desativar(string motivo)
    {
        if (!Ativo)
            throw new InvalidOperationException("Aluno já está inativo");

        Ativo = false;

        // Inativar todas as matrículas ativas
        foreach (var matricula in _matriculas.Where(m => m.Ativa))
        {
            matricula.Inativar(motivo);
        }

        AddDomainEvent(new AlunoDesativadoEvento(Id, Nome.Valor, motivo));
    }

    public void Ativar()
    {
        if (Ativo)
            throw new InvalidOperationException("Aluno já está ativo");

        Ativo = true;
        AddDomainEvent(new AlunoAtivadoEvento(Id, Nome.Valor));
    }

    public Matricula? ObterMatriculaAtiva()
    {
        return _matriculas.FirstOrDefault(m => m.Ativa);
    }

    public IEnumerable<Matricula> ObterMatriculasPorAno(int anoLetivo)
    {
        return _matriculas.Where(m => m.AnoLetivo == anoLetivo);
    }

    public bool PossuiMatriculaAtiva()
    {
        return _matriculas.Any(m => m.Ativa);
    }

    public bool EhMenorIdade()
    {
        return !DataNascimento.EhMaiorIdade;
    }

    public string ObterFaixaEtariaEscolar()
    {
        return DataNascimento.ObterFaixaEtariaEscolar();
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

    private void ValidarIdadeEscolar()
    {
        if (!DataNascimento.EhIdadeEscolar())
        {
            // Permitir idades fora do padrão, mas registrar evento para análise
            AddDomainEvent(new AlunoIdadeForaPadraoEvento(Id, DataNascimento.Idade, DataNascimento.ObterFaixaEtariaEscolar()));
        }
    }
}