using SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Escolas.Dominio.Eventos;
using SistemaGestaoEscolar.Shared.Domain.Entities;

namespace SistemaGestaoEscolar.Escolas.Dominio.Entidades;

public class Escola : AggregateRoot
{
    public new Guid Id { get; private set; }
    public NomeEscola Nome { get; private set; } = null!;
    public CNPJ Cnpj { get; private set; } = null!;
    public Endereco Endereco { get; private set; } = null!;
    public TipoEscola Tipo { get; private set; } = null!;
    public Guid? RedeEscolarId { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public bool Ativa { get; private set; }
    
    private readonly List<UnidadeEscolar> _unidades = new();
    public IReadOnlyCollection<UnidadeEscolar> Unidades => _unidades.AsReadOnly();

    private Escola() { } // Para EF Core

    public Escola(
        NomeEscola nome,
        CNPJ cnpj,
        Endereco endereco,
        TipoEscola tipo,
        Guid? redeEscolarId = null)
    {
        Id = Guid.NewGuid();
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Cnpj = cnpj ?? throw new ArgumentNullException(nameof(cnpj));
        Endereco = endereco ?? throw new ArgumentNullException(nameof(endereco));
        Tipo = tipo ?? throw new ArgumentNullException(nameof(tipo));
        RedeEscolarId = redeEscolarId;
        DataCriacao = DateTime.UtcNow;
        Ativa = true;

        AddDomainEvent(new EscolaCriadaEvento(Id, Nome.Valor, Cnpj.Numero));
    }

    public void AtualizarNome(NomeEscola novoNome)
    {
        if (novoNome == null)
            throw new ArgumentNullException(nameof(novoNome));

        var nomeAnterior = Nome;
        Nome = novoNome;

        AddDomainEvent(new EscolaAtualizadaEvento(Id, nomeAnterior.Valor, novoNome.Valor));
    }

    public void AtualizarEndereco(Endereco novoEndereco)
    {
        if (novoEndereco == null)
            throw new ArgumentNullException(nameof(novoEndereco));

        Endereco = novoEndereco;
        AddDomainEvent(new EscolaAtualizadaEvento(Id, "Endereço atualizado", novoEndereco.ToString()));
    }

    public void AdicionarUnidade(UnidadeEscolar unidade)
    {
        if (unidade == null)
            throw new ArgumentNullException(nameof(unidade));

        if (_unidades.Any(u => u.Nome.Valor.Equals(unidade.Nome.Valor, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Já existe uma unidade com este nome nesta escola");

        _unidades.Add(unidade);
        AddDomainEvent(new UnidadeEscolarAdicionadaEvento(Id, unidade.Id, unidade.Nome.Valor));
    }

    public void RemoverUnidade(Guid unidadeId)
    {
        var unidade = _unidades.FirstOrDefault(u => u.Id == unidadeId);
        if (unidade == null)
            throw new InvalidOperationException("Unidade não encontrada");

        _unidades.Remove(unidade);
        AddDomainEvent(new UnidadeEscolarRemovidaEvento(Id, unidadeId, unidade.Nome.Valor));
    }

    public void Desativar()
    {
        if (!Ativa)
            throw new InvalidOperationException("Escola já está desativada");

        Ativa = false;
        AddDomainEvent(new EscolaDesativadaEvento(Id, Nome.Valor));
    }

    public void Ativar()
    {
        if (Ativa)
            throw new InvalidOperationException("Escola já está ativa");

        Ativa = true;
        AddDomainEvent(new EscolaAtivadaEvento(Id, Nome.Valor));
    }

    public void AssociarRede(Guid redeEscolarId)
    {
        if (RedeEscolarId.HasValue)
            throw new InvalidOperationException("Escola já está associada a uma rede");

        RedeEscolarId = redeEscolarId;
        AddDomainEvent(new EscolaAssociadaRedeEvento(Id, redeEscolarId));
    }

    public void DesassociarRede()
    {
        if (!RedeEscolarId.HasValue)
            throw new InvalidOperationException("Escola não está associada a nenhuma rede");

        var redeAnterior = RedeEscolarId.Value;
        RedeEscolarId = null;
        AddDomainEvent(new EscolaDesassociadaRedeEvento(Id, redeAnterior));
    }
}