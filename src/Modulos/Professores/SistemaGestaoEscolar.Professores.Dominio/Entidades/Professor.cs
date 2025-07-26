using SistemaGestaoEscolar.Professores.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Professores.Dominio.Eventos;
using SistemaGestaoEscolar.Shared.Domain.Entities;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Professores.Dominio.Entidades;

public class Professor : AggregateRoot
{
    public new Guid Id { get; private set; }
    public NomeProfessor Nome { get; private set; } = null!;
    public Cpf Cpf { get; private set; } = null!;
    public RegistroProfessor Registro { get; private set; } = null!;
    public string? Email { get; private set; }
    public string? Telefone { get; private set; }
    public DateTime DataNascimento { get; private set; }
    public DateTime DataContratacao { get; private set; }
    public Guid EscolaId { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public string? Observacoes { get; private set; }

    private readonly List<TituloAcademico> _titulos = new();
    public IReadOnlyCollection<TituloAcademico> Titulos => _titulos.AsReadOnly();

    private readonly List<ProfessorDisciplina> _disciplinas = new();
    public IReadOnlyCollection<ProfessorDisciplina> Disciplinas => _disciplinas.AsReadOnly();

    private Professor() { } // Para EF Core

    public Professor(
        NomeProfessor nome,
        Cpf cpf,
        RegistroProfessor registro,
        DateTime dataNascimento,
        DateTime dataContratacao,
        Guid escolaId,
        string? email = null,
        string? telefone = null,
        string? observacoes = null)
    {
        Id = Guid.NewGuid();
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Cpf = cpf ?? throw new ArgumentNullException(nameof(cpf));
        Registro = registro ?? throw new ArgumentNullException(nameof(registro));
        DataNascimento = dataNascimento;
        DataContratacao = dataContratacao;
        EscolaId = escolaId == Guid.Empty ? throw new ArgumentException("EscolaId não pode ser vazio") : escolaId;
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLower();
        Telefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone.Trim();
        Observacoes = string.IsNullOrWhiteSpace(observacoes) ? null : observacoes.Trim();
        DataCadastro = DateTime.UtcNow;
        Ativo = true;

        ValidarIdadeMinima();
        ValidarDataContratacao();
        ValidarEmail();

        AddDomainEvent(new ProfessorCriadoEvento(Id, Nome.Valor, Cpf.Numero, EscolaId));
    }

    public void AtualizarNome(NomeProfessor novoNome)
    {
        if (novoNome == null)
            throw new ArgumentNullException(nameof(novoNome));

        var nomeAnterior = Nome;
        Nome = novoNome;

        AddDomainEvent(new ProfessorAtualizadoEvento(Id, "Nome", nomeAnterior.Valor, novoNome.Valor));
    }

    public void AtualizarEmail(string? novoEmail)
    {
        var emailAnterior = Email;
        Email = string.IsNullOrWhiteSpace(novoEmail) ? null : novoEmail.Trim().ToLower();
        
        ValidarEmail();

        AddDomainEvent(new ProfessorAtualizadoEvento(Id, "Email", emailAnterior ?? "", Email ?? ""));
    }

    public void AtualizarTelefone(string? novoTelefone)
    {
        var telefoneAnterior = Telefone;
        Telefone = string.IsNullOrWhiteSpace(novoTelefone) ? null : novoTelefone.Trim();

        AddDomainEvent(new ProfessorAtualizadoEvento(Id, "Telefone", telefoneAnterior ?? "", Telefone ?? ""));
    }

    public void AtualizarObservacoes(string? novasObservacoes)
    {
        Observacoes = string.IsNullOrWhiteSpace(novasObservacoes) ? null : novasObservacoes.Trim();
    }

    public void AdicionarTitulo(TituloAcademico titulo)
    {
        if (titulo == null)
            throw new ArgumentNullException(nameof(titulo));

        // Verificar se já existe título igual
        if (_titulos.Any(t => t.Equals(titulo)))
            throw new InvalidOperationException("Professor já possui este título acadêmico");

        _titulos.Add(titulo);
        AddDomainEvent(new TituloAcademicoAdicionadoEvento(Id, titulo.Tipo, titulo.Curso, titulo.Instituicao));
    }

    public void RemoverTitulo(TituloAcademico titulo)
    {
        if (titulo == null)
            throw new ArgumentNullException(nameof(titulo));

        var tituloExistente = _titulos.FirstOrDefault(t => t.Equals(titulo));
        if (tituloExistente == null)
            throw new InvalidOperationException("Título não encontrado");

        _titulos.Remove(tituloExistente);
        AddDomainEvent(new TituloAcademicoRemovidoEvento(Id, titulo.Tipo, titulo.Curso, titulo.Instituicao));
    }

    public void LimparTitulos()
    {
        _titulos.Clear();
    }

    public void AtribuirDisciplina(Guid disciplinaId, int cargaHorariaSemanal, string? observacoes = null)
    {
        if (disciplinaId == Guid.Empty)
            throw new ArgumentException("DisciplinaId não pode ser vazio");

        if (cargaHorariaSemanal <= 0 || cargaHorariaSemanal > 40)
            throw new ArgumentException("Carga horária semanal deve estar entre 1 e 40 horas");

        // Verificar se já leciona esta disciplina
        if (_disciplinas.Any(d => d.DisciplinaId == disciplinaId))
            throw new InvalidOperationException("Professor já leciona esta disciplina");

        // Verificar carga horária total
        var cargaHorariaTotal = _disciplinas.Sum(d => d.CargaHorariaSemanal) + cargaHorariaSemanal;
        if (cargaHorariaTotal > 40)
            throw new InvalidOperationException($"Carga horária total excederia 40 horas semanais. Atual: {cargaHorariaTotal - cargaHorariaSemanal}h");

        var professorDisciplina = new ProfessorDisciplina(Id, disciplinaId, cargaHorariaSemanal, observacoes);
        _disciplinas.Add(professorDisciplina);

        AddDomainEvent(new DisciplinaAtribuidaEvento(Id, disciplinaId, cargaHorariaSemanal));
    }

    public void RemoverDisciplina(Guid disciplinaId)
    {
        var disciplina = _disciplinas.FirstOrDefault(d => d.DisciplinaId == disciplinaId);
        if (disciplina == null)
            throw new InvalidOperationException("Professor não leciona esta disciplina");

        _disciplinas.Remove(disciplina);
        AddDomainEvent(new DisciplinaRemovidaEvento(Id, disciplinaId));
    }

    public void AtualizarCargaHorariaDisciplina(Guid disciplinaId, int novaCargaHoraria)
    {
        var disciplina = _disciplinas.FirstOrDefault(d => d.DisciplinaId == disciplinaId);
        if (disciplina == null)
            throw new InvalidOperationException("Professor não leciona esta disciplina");

        if (novaCargaHoraria <= 0 || novaCargaHoraria > 40)
            throw new ArgumentException("Carga horária semanal deve estar entre 1 e 40 horas");

        // Verificar carga horária total
        var cargaHorariaOutrasDisciplinas = _disciplinas.Where(d => d.DisciplinaId != disciplinaId).Sum(d => d.CargaHorariaSemanal);
        if (cargaHorariaOutrasDisciplinas + novaCargaHoraria > 40)
            throw new InvalidOperationException($"Carga horária total excederia 40 horas semanais");

        var cargaAnterior = disciplina.CargaHorariaSemanal;
        disciplina.AtualizarCargaHoraria(novaCargaHoraria);

        AddDomainEvent(new CargaHorariaDisciplinaAtualizadaEvento(Id, disciplinaId, cargaAnterior, novaCargaHoraria));
    }

    public void LimparDisciplinas()
    {
        _disciplinas.Clear();
    }

    public void Desativar(string motivo)
    {
        if (!Ativo)
            throw new InvalidOperationException("Professor já está inativo");

        Ativo = false;
        AddDomainEvent(new ProfessorDesativadoEvento(Id, Nome.Valor, motivo));
    }

    public void Ativar()
    {
        if (Ativo)
            throw new InvalidOperationException("Professor já está ativo");

        Ativo = true;
        AddDomainEvent(new ProfessorAtivadoEvento(Id, Nome.Valor));
    }

    public void TransferirEscola(Guid novaEscolaId, string motivo)
    {
        if (novaEscolaId == Guid.Empty)
            throw new ArgumentException("Nova escola ID não pode ser vazio");

        if (novaEscolaId == EscolaId)
            throw new InvalidOperationException("Professor já está nesta escola");

        var escolaAnterior = EscolaId;
        EscolaId = novaEscolaId;

        // Limpar disciplinas da escola anterior
        _disciplinas.Clear();

        AddDomainEvent(new ProfessorTransferidoEvento(Id, escolaAnterior, novaEscolaId, motivo));
    }

    public int ObterIdade()
    {
        var hoje = DateTime.Today;
        var idade = hoje.Year - DataNascimento.Year;
        if (DataNascimento.Date > hoje.AddYears(-idade))
            idade--;
        return idade;
    }

    public int ObterTempoServico()
    {
        var hoje = DateTime.Today;
        var tempoServico = hoje.Year - DataContratacao.Year;
        if (DataContratacao.Date > hoje.AddYears(-tempoServico))
            tempoServico--;
        return Math.Max(0, tempoServico);
    }

    public int ObterCargaHorariaTotal()
    {
        return _disciplinas.Sum(d => d.CargaHorariaSemanal);
    }

    public TituloAcademico? ObterMaiorTitulo()
    {
        return _titulos.OrderByDescending(t => t.ObterNivelAcademico()).FirstOrDefault();
    }

    public bool PossuiTituloSuperior()
    {
        return _titulos.Any(t => t.EhTituloSuperior());
    }

    public bool PossuiPosGraduacao()
    {
        return _titulos.Any(t => t.EhTituloPosGraduacao());
    }

    public IEnumerable<Guid> ObterIdsDisciplinas()
    {
        return _disciplinas.Select(d => d.DisciplinaId);
    }

    private void ValidarIdadeMinima()
    {
        var idade = ObterIdade();
        if (idade < 18)
            throw new ArgumentException("Professor deve ter pelo menos 18 anos");
    }

    private void ValidarDataContratacao()
    {
        if (DataContratacao > DateTime.Today)
            throw new ArgumentException("Data de contratação não pode ser futura");

        if (DataContratacao < DataNascimento.AddYears(16))
            throw new ArgumentException("Data de contratação deve ser posterior ao 16º aniversário");
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
}