using SistemaGestaoEscolar.Alunos.Dominio.Entidades;
using SistemaGestaoEscolar.Alunos.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Alunos.Infraestrutura.Persistencia.Entidades;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Alunos.Infraestrutura.Mapeadores;

public static class ResponsavelMapper
{
    public static ResponsavelEntity ToEntity(Responsavel responsavel)
    {
        return new ResponsavelEntity
        {
            Id = responsavel.Id,
            Nome = responsavel.Nome.Valor,
            Cpf = responsavel.Cpf.Numero,
            Telefone = responsavel.Telefone,
            Email = responsavel.Email,
            Tipo = (int)responsavel.Tipo,
            Profissao = responsavel.Profissao,
            LocalTrabalho = responsavel.LocalTrabalho,
            TelefoneTrabalho = responsavel.TelefoneTrabalho,
            ResponsavelFinanceiro = responsavel.ResponsavelFinanceiro,
            ResponsavelAcademico = responsavel.ResponsavelAcademico,
            AutorizadoBuscar = responsavel.AutorizadoBuscar,
            Observacoes = responsavel.Observacoes,
            Logradouro = responsavel.Endereco?.Logradouro,
            Numero = responsavel.Endereco?.Numero,
            Complemento = responsavel.Endereco?.Complemento,
            Bairro = responsavel.Endereco?.Bairro,
            Cidade = responsavel.Endereco?.Cidade,
            Estado = responsavel.Endereco?.Estado,
            Cep = responsavel.Endereco?.Cep,
            CreatedAt = responsavel.CreatedAt,
            UpdatedAt = responsavel.UpdatedAt
        };
    }

    public static Responsavel ToDomain(ResponsavelEntity entity)
    {
        var nome = new NomeAluno(entity.Nome);
        var cpf = new CPF(entity.Cpf);
        var tipo = (TipoResponsavel)entity.Tipo;

        Endereco? endereco = null;
        if (!string.IsNullOrEmpty(entity.Logradouro))
        {
            endereco = new Endereco(
                entity.Logradouro,
                entity.Numero ?? "",
                entity.Bairro ?? "",
                entity.Cidade ?? "",
                entity.Estado ?? "",
                entity.Cep ?? "",
                entity.Complemento);
        }

        var responsavel = new Responsavel(
            nome,
            cpf,
            entity.Telefone,
            tipo,
            entity.Email,
            endereco,
            entity.Profissao,
            entity.LocalTrabalho,
            entity.TelefoneTrabalho,
            entity.ResponsavelFinanceiro,
            entity.ResponsavelAcademico,
            entity.AutorizadoBuscar,
            entity.Observacoes);

        // Definir propriedades que não são definidas no construtor
        SetPrivateProperty(responsavel, "Id", entity.Id);
        SetPrivateProperty(responsavel, "CreatedAt", entity.CreatedAt);
        SetPrivateProperty(responsavel, "UpdatedAt", entity.UpdatedAt!);

        return responsavel;
    }

    public static void UpdateEntity(ResponsavelEntity entity, Responsavel responsavel)
    {
        entity.Nome = responsavel.Nome.Valor;
        entity.Telefone = responsavel.Telefone;
        entity.Email = responsavel.Email;
        entity.Tipo = (int)responsavel.Tipo;
        entity.Profissao = responsavel.Profissao;
        entity.LocalTrabalho = responsavel.LocalTrabalho;
        entity.TelefoneTrabalho = responsavel.TelefoneTrabalho;
        entity.ResponsavelFinanceiro = responsavel.ResponsavelFinanceiro;
        entity.ResponsavelAcademico = responsavel.ResponsavelAcademico;
        entity.AutorizadoBuscar = responsavel.AutorizadoBuscar;
        entity.Observacoes = responsavel.Observacoes;
        entity.Logradouro = responsavel.Endereco?.Logradouro;
        entity.Numero = responsavel.Endereco?.Numero;
        entity.Complemento = responsavel.Endereco?.Complemento;
        entity.Bairro = responsavel.Endereco?.Bairro;
        entity.Cidade = responsavel.Endereco?.Cidade;
        entity.Estado = responsavel.Endereco?.Estado;
        entity.Cep = responsavel.Endereco?.Cep;
        entity.UpdatedAt = responsavel.UpdatedAt;
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