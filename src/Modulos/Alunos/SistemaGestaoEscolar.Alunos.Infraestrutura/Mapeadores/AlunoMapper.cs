using SistemaGestaoEscolar.Alunos.Dominio.Entidades;
using SistemaGestaoEscolar.Alunos.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Alunos.Infraestrutura.Persistencia.Entidades;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Alunos.Infraestrutura.Mapeadores;

public static class AlunoMapper
{
    public static AlunoEntity ToEntity(Aluno aluno)
    {
        return new AlunoEntity
        {
            Id = aluno.Id,
            Nome = aluno.Nome.Valor,
            Cpf = aluno.Cpf.Numero,
            DataNascimento = aluno.DataNascimento.Valor,
            Logradouro = aluno.Endereco.Logradouro,
            Numero = aluno.Endereco.Numero,
            Complemento = aluno.Endereco.Complemento,
            Bairro = aluno.Endereco.Bairro,
            Cidade = aluno.Endereco.Cidade,
            Estado = aluno.Endereco.Estado,
            Cep = aluno.Endereco.Cep,
            Genero = (int)aluno.Genero.Tipo,
            TipoDeficiencia = aluno.Deficiencia.PossuiDeficiencia ? (int)aluno.Deficiencia.Tipo! : null,
            DescricaoDeficiencia = aluno.Deficiencia.PossuiDeficiencia ? aluno.Deficiencia.Descricao : null,
            Telefone = aluno.Telefone,
            Email = aluno.Email,
            Observacoes = aluno.Observacoes,
            EscolaId = aluno.EscolaId,
            DataCadastro = aluno.DataCadastro,
            Ativo = aluno.Ativo,
            CreatedAt = aluno.CreatedAt,
            UpdatedAt = aluno.UpdatedAt,
            Responsaveis = aluno.Responsaveis.Select(ResponsavelMapper.ToEntity).ToList(),
            Matriculas = aluno.Matriculas.Select(MatriculaMapper.ToEntity).ToList()
        };
    }

    public static Aluno ToDomain(AlunoEntity entity)
    {
        var nome = new NomeAluno(entity.Nome);
        var cpf = new Cpf(entity.Cpf);
        var dataNascimento = new DataNascimento(entity.DataNascimento);
        var endereco = new Endereco(
            entity.Logradouro,
            entity.Numero,
            entity.Bairro,
            entity.Cidade,
            entity.Estado,
            entity.Cep,
            entity.Complemento);
        var genero = Genero.Criar(entity.Genero);
        var deficiencia = entity.TipoDeficiencia.HasValue && !string.IsNullOrWhiteSpace(entity.DescricaoDeficiencia)
            ? Deficiencia.Criar(entity.TipoDeficiencia.Value, entity.DescricaoDeficiencia)
            : Deficiencia.Nenhuma();

        // Usar reflexão para definir o ID (já que o construtor gera um novo)
        var aluno = new Aluno(
            nome,
            cpf,
            dataNascimento,
            endereco,
            entity.EscolaId,
            genero,
            deficiencia,
            entity.Telefone,
            entity.Email,
            entity.Observacoes);

        // Definir propriedades que não são definidas no construtor
        SetPrivateProperty(aluno, "Id", entity.Id);
        SetPrivateProperty(aluno, "DataCadastro", entity.DataCadastro);
        SetPrivateProperty(aluno, "Ativo", entity.Ativo);
        SetPrivateProperty(aluno, "CreatedAt", entity.CreatedAt);
        SetPrivateProperty(aluno, "UpdatedAt", entity.UpdatedAt!);

        // Adicionar responsáveis
        foreach (var responsavelEntity in entity.Responsaveis)
        {
            var responsavel = ResponsavelMapper.ToDomain(responsavelEntity);
            aluno.AdicionarResponsavel(responsavel);
        }

        // Adicionar matrículas
        foreach (var matriculaEntity in entity.Matriculas)
        {
            var matricula = MatriculaMapper.ToDomain(matriculaEntity);
            aluno.AdicionarMatricula(matricula);
        }

        // Limpar eventos de domínio gerados durante a reconstrução
        aluno.ClearDomainEvents();

        return aluno;
    }

    public static void UpdateEntity(AlunoEntity entity, Aluno aluno)
    {
        entity.Nome = aluno.Nome.Valor;
        entity.DataNascimento = aluno.DataNascimento.Valor;
        entity.Logradouro = aluno.Endereco.Logradouro;
        entity.Numero = aluno.Endereco.Numero;
        entity.Complemento = aluno.Endereco.Complemento;
        entity.Bairro = aluno.Endereco.Bairro;
        entity.Cidade = aluno.Endereco.Cidade;
        entity.Estado = aluno.Endereco.Estado;
        entity.Cep = aluno.Endereco.Cep;
        entity.Genero = (int)aluno.Genero.Tipo;
        entity.TipoDeficiencia = aluno.Deficiencia.PossuiDeficiencia ? (int)aluno.Deficiencia.Tipo! : null;
        entity.DescricaoDeficiencia = aluno.Deficiencia.PossuiDeficiencia ? aluno.Deficiencia.Descricao : null;
        entity.Telefone = aluno.Telefone;
        entity.Email = aluno.Email;
        entity.Observacoes = aluno.Observacoes;
        entity.EscolaId = aluno.EscolaId;
        entity.Ativo = aluno.Ativo;
        entity.UpdatedAt = aluno.UpdatedAt;

        // Atualizar responsáveis - remover existentes e adicionar novos
        var responsaveisParaRemover = entity.Responsaveis.ToList();
        foreach (var responsavelExistente in responsaveisParaRemover)
        {
            entity.Responsaveis.Remove(responsavelExistente);
        }

        foreach (var responsavel in aluno.Responsaveis)
        {
            var responsavelEntity = ResponsavelMapper.ToEntity(responsavel);
            // Garantir que seja um novo responsável (INSERT)
            responsavelEntity.Id = Guid.NewGuid();
            responsavelEntity.AlunoId = entity.Id;
            entity.Responsaveis.Add(responsavelEntity);
        }
    }

    private static void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName, 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Public | 
            System.Reflection.BindingFlags.Instance);
        
        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value);
        }
        else
        {
            // Tentar campo privado
            var field = obj.GetType().GetField($"<{propertyName}>k__BackingField", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            field?.SetValue(obj, value);
        }
    }
}