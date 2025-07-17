using SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Escolas.Dominio.Eventos;
using SistemaGestaoEscolar.Shared.Domain.Entities;

namespace SistemaGestaoEscolar.Escolas.Dominio.Entidades;

public class UnidadeEscolar : BaseEntity
{
    public new Guid Id { get; private set; }
    public NomeEscola Nome { get; private set; } = null!;
    public Endereco Endereco { get; private set; } = null!;
    public TipoEscola Tipo { get; private set; } = null!;
    public Guid EscolaId { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public bool Ativa { get; private set; }
    public int CapacidadeMaximaAlunos { get; private set; }
    public int AlunosMatriculados { get; private set; }

    // Propriedades específicas por tipo de ensino
    public int? IdadeMinima { get; private set; } // Para ensino infantil
    public int? IdadeMaxima { get; private set; } // Para ensino infantil
    public bool TemBerçario { get; private set; } // Para ensino infantil
    public bool TemPreEscola { get; private set; } // Para ensino infantil
    
    // Séries atendidas (para fundamental e médio)
    private readonly List<string> _seriesAtendidas = new();
    public IReadOnlyCollection<string> SeriesAtendidas => _seriesAtendidas.AsReadOnly();

    private UnidadeEscolar() { } // Para EF Core

    public UnidadeEscolar(
        NomeEscola nome,
        Endereco endereco,
        TipoEscola tipo,
        Guid escolaId,
        int capacidadeMaximaAlunos)
    {
        Id = Guid.NewGuid();
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Endereco = endereco ?? throw new ArgumentNullException(nameof(endereco));
        Tipo = tipo ?? throw new ArgumentNullException(nameof(tipo));
        EscolaId = escolaId;
        CapacidadeMaximaAlunos = capacidadeMaximaAlunos > 0 ? capacidadeMaximaAlunos : 
            throw new ArgumentException("Capacidade máxima deve ser maior que zero", nameof(capacidadeMaximaAlunos));
        DataCriacao = DateTime.UtcNow;
        Ativa = true;
        AlunosMatriculados = 0;

        ConfigurarPorTipoEnsino(tipo);
    }

    private void ConfigurarPorTipoEnsino(TipoEscola tipo)
    {
        switch (tipo.Valor)
        {
            case "Infantil":
                IdadeMinima = 0;
                IdadeMaxima = 5;
                TemBerçario = true;
                TemPreEscola = true;
                break;
                
            case "Fundamental":
                _seriesAtendidas.AddRange(new[] { "1º Ano", "2º Ano", "3º Ano", "4º Ano", "5º Ano", 
                                                 "6º Ano", "7º Ano", "8º Ano", "9º Ano" });
                break;
                
            case "Médio":
                _seriesAtendidas.AddRange(new[] { "1º Ano", "2º Ano", "3º Ano" });
                break;
                
            case "Fundamental e Médio":
                _seriesAtendidas.AddRange(new[] { "1º Ano Fund", "2º Ano Fund", "3º Ano Fund", "4º Ano Fund", "5º Ano Fund", 
                                                 "6º Ano Fund", "7º Ano Fund", "8º Ano Fund", "9º Ano Fund",
                                                 "1º Ano Médio", "2º Ano Médio", "3º Ano Médio" });
                break;
        }
    }

    public void AtualizarNome(NomeEscola novoNome)
    {
        if (novoNome == null)
            throw new ArgumentNullException(nameof(novoNome));

        Nome = novoNome;
    }

    public void AtualizarEndereco(Endereco novoEndereco)
    {
        if (novoEndereco == null)
            throw new ArgumentNullException(nameof(novoEndereco));

        Endereco = novoEndereco;
    }

    public void AtualizarCapacidade(int novaCapacidade)
    {
        if (novaCapacidade <= 0)
            throw new ArgumentException("Capacidade deve ser maior que zero", nameof(novaCapacidade));

        if (novaCapacidade < AlunosMatriculados)
            throw new InvalidOperationException($"Nova capacidade ({novaCapacidade}) não pode ser menor que o número atual de alunos matriculados ({AlunosMatriculados})");

        CapacidadeMaximaAlunos = novaCapacidade;
    }

    public void RegistrarMatricula()
    {
        if (AlunosMatriculados >= CapacidadeMaximaAlunos)
            throw new InvalidOperationException("Capacidade máxima da unidade atingida");

        AlunosMatriculados++;
    }

    public void RegistrarDesmatricula()
    {
        if (AlunosMatriculados <= 0)
            throw new InvalidOperationException("Não há alunos matriculados para desmatricular");

        AlunosMatriculados--;
    }

    public void ConfigurarEnsinoInfantil(int idadeMinima, int idadeMaxima, bool temBerçario, bool temPreEscola)
    {
        if (Tipo.Valor != "Infantil")
            throw new InvalidOperationException("Esta configuração só é válida para ensino infantil");

        if (idadeMinima < 0 || idadeMaxima < 0 || idadeMinima > idadeMaxima)
            throw new ArgumentException("Idades inválidas");

        IdadeMinima = idadeMinima;
        IdadeMaxima = idadeMaxima;
        TemBerçario = temBerçario;
        TemPreEscola = temPreEscola;
    }

    public void AdicionarSerie(string serie)
    {
        if (Tipo.Valor == "Infantil")
            throw new InvalidOperationException("Ensino infantil não trabalha com séries");

        if (string.IsNullOrWhiteSpace(serie))
            throw new ArgumentException("Série não pode ser vazia", nameof(serie));

        if (_seriesAtendidas.Contains(serie))
            throw new InvalidOperationException($"Série '{serie}' já está cadastrada");

        _seriesAtendidas.Add(serie);
    }

    public void RemoverSerie(string serie)
    {
        if (Tipo.Valor == "Infantil")
            throw new InvalidOperationException("Ensino infantil não trabalha com séries");

        if (!_seriesAtendidas.Contains(serie))
            throw new InvalidOperationException($"Série '{serie}' não encontrada");

        _seriesAtendidas.Remove(serie);
    }

    public void Desativar()
    {
        if (!Ativa)
            throw new InvalidOperationException("Unidade já está desativada");

        if (AlunosMatriculados > 0)
            throw new InvalidOperationException("Não é possível desativar unidade com alunos matriculados");

        Ativa = false;
    }

    public void Ativar()
    {
        if (Ativa)
            throw new InvalidOperationException("Unidade já está ativa");

        Ativa = true;
    }

    public bool PodeReceberNovasMatriculas => Ativa && AlunosMatriculados < CapacidadeMaximaAlunos;
    public int VagasDisponiveis => CapacidadeMaximaAlunos - AlunosMatriculados;
    public double PercentualOcupacao => CapacidadeMaximaAlunos > 0 ? (double)AlunosMatriculados / CapacidadeMaximaAlunos * 100 : 0;

    public bool AtendeSerie(string serie)
    {
        return _seriesAtendidas.Contains(serie);
    }

    public bool AtendeIdade(int idade)
    {
        if (Tipo.Valor != "Infantil")
            return false;

        return idade >= IdadeMinima && idade <= IdadeMaxima;
    }
}