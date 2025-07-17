using SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Escolas.Dominio.Eventos;
using SistemaGestaoEscolar.Shared.Domain.Entities;

namespace SistemaGestaoEscolar.Escolas.Dominio.Entidades;

public class RedeEscolar : AggregateRoot
{
    public new Guid Id { get; private set; }
    public NomeEscola Nome { get; private set; } = null!;
    public CNPJ CnpjMantenedora { get; private set; } = null!;
    public Endereco EnderecoSede { get; private set; } = null!;
    public DateTime DataCriacao { get; private set; }
    public bool Ativa { get; private set; }
    
    private readonly List<Escola> _escolas = new();
    public IReadOnlyCollection<Escola> Escolas => _escolas.AsReadOnly();

    private RedeEscolar() { } // Para EF Core

    public RedeEscolar(
        NomeEscola nome,
        CNPJ cnpjMantenedora,
        Endereco enderecoSede)
    {
        Id = Guid.NewGuid();
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        CnpjMantenedora = cnpjMantenedora ?? throw new ArgumentNullException(nameof(cnpjMantenedora));
        EnderecoSede = enderecoSede ?? throw new ArgumentNullException(nameof(enderecoSede));
        DataCriacao = DateTime.UtcNow;
        Ativa = true;

        AddDomainEvent(new RedeEscolarCriadaEvento(Id, Nome.Valor, CnpjMantenedora.Numero));
    }

    public void AtualizarNome(NomeEscola novoNome)
    {
        if (novoNome == null)
            throw new ArgumentNullException(nameof(novoNome));

        var nomeAnterior = Nome;
        Nome = novoNome;

        AddDomainEvent(new RedeEscolarAtualizadaEvento(Id, nomeAnterior.Valor, novoNome.Valor));
    }

    public void AtualizarEnderecoSede(Endereco novoEndereco)
    {
        if (novoEndereco == null)
            throw new ArgumentNullException(nameof(novoEndereco));

        EnderecoSede = novoEndereco;
        AddDomainEvent(new RedeEscolarAtualizadaEvento(Id, "Endereço da sede atualizado", novoEndereco.ToString()));
    }

    public void AdicionarEscola(Escola escola)
    {
        if (escola == null)
            throw new ArgumentNullException(nameof(escola));

        if (_escolas.Any(e => e.Cnpj.Numero == escola.Cnpj.Numero))
            throw new InvalidOperationException("Já existe uma escola com este CNPJ nesta rede");

        if (_escolas.Any(e => e.Nome.Valor.Equals(escola.Nome.Valor, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Já existe uma escola com este nome nesta rede");

        escola.AssociarRede(Id);
        _escolas.Add(escola);
        
        AddDomainEvent(new EscolaAdicionadaRedeEvento(Id, escola.Id, escola.Nome.Valor));
    }

    public void RemoverEscola(Guid escolaId)
    {
        var escola = _escolas.FirstOrDefault(e => e.Id == escolaId);
        if (escola == null)
            throw new InvalidOperationException("Escola não encontrada nesta rede");

        escola.DesassociarRede();
        _escolas.Remove(escola);
        
        AddDomainEvent(new EscolaRemovidaRedeEvento(Id, escolaId, escola.Nome.Valor));
    }

    public void Desativar()
    {
        if (!Ativa)
            throw new InvalidOperationException("Rede escolar já está desativada");

        // Desativar todas as escolas da rede
        foreach (var escola in _escolas.Where(e => e.Ativa))
        {
            escola.Desativar();
        }

        Ativa = false;
        AddDomainEvent(new RedeEscolarDesativadaEvento(Id, Nome.Valor));
    }

    public void Ativar()
    {
        if (Ativa)
            throw new InvalidOperationException("Rede escolar já está ativa");

        Ativa = true;
        AddDomainEvent(new RedeEscolarAtivadaEvento(Id, Nome.Valor));
    }

    public int TotalEscolas => _escolas.Count;
    public int EscolasAtivas => _escolas.Count(e => e.Ativa);
    public int EscolasInativas => _escolas.Count(e => !e.Ativa);

    public IEnumerable<Escola> ObterEscolasPorTipo(TipoEscola tipo)
    {
        return _escolas.Where(e => e.Tipo.Equals(tipo));
    }

    public bool PossuiEscolaComCnpj(CNPJ cnpj)
    {
        return _escolas.Any(e => e.Cnpj.Numero == cnpj.Numero);
    }
}