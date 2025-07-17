using FluentValidation;
using SistemaGestaoEscolar.Escolas.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Escolas.Infraestrutura.Validadores;

public class ValidadorCriarEscola : AbstractValidator<CriarEscolaDto>
{
    public ValidadorCriarEscola()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Nome da escola é obrigatório")
            .Length(2, 200)
            .WithMessage("Nome da escola deve ter entre 2 e 200 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ0-9\s\-\.]+$")
            .WithMessage("Nome da escola contém caracteres inválidos");

        RuleFor(x => x.Cnpj)
            .NotEmpty()
            .WithMessage("CNPJ é obrigatório")
            .Must(ValidarCnpj)
            .WithMessage("CNPJ inválido");

        RuleFor(x => x.Tipo)
            .NotEmpty()
            .WithMessage("Tipo da escola é obrigatório")
            .Must(ValidarTipoEscola)
            .WithMessage("Tipo de escola inválido. Valores aceitos: Infantil, Fundamental, Medio, Tecnico, Superior");

        RuleFor(x => x.Endereco)
            .NotNull()
            .WithMessage("Endereço é obrigatório")
            .SetValidator(new ValidadorEndereco());

        RuleFor(x => x.RedeEscolarId)
            .Must(ValidarRedeEscolarId)
            .WithMessage("ID da rede escolar deve ser um GUID válido quando informado");
    }

    private static bool ValidarCnpj(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        // Remove caracteres não numéricos
        cnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");

        // Verifica se tem 14 dígitos
        if (cnpj.Length != 14)
            return false;

        // Verifica se todos os dígitos são iguais
        if (cnpj.All(c => c == cnpj[0]))
            return false;

        // Validação do primeiro dígito verificador
        int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int soma = 0;
        for (int i = 0; i < 12; i++)
        {
            soma += int.Parse(cnpj[i].ToString()) * multiplicador1[i];
        }
        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cnpj[12].ToString()) != digito1)
            return false;

        // Validação do segundo dígito verificador
        int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        soma = 0;
        for (int i = 0; i < 13; i++)
        {
            soma += int.Parse(cnpj[i].ToString()) * multiplicador2[i];
        }
        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;

        return int.Parse(cnpj[13].ToString()) == digito2;
    }

    private static bool ValidarTipoEscola(string tipo)
    {
        var tiposValidos = new[] { "Infantil", "Fundamental", "Medio", "Tecnico", "Superior" };
        return tiposValidos.Contains(tipo, StringComparer.OrdinalIgnoreCase);
    }

    private static bool ValidarRedeEscolarId(Guid? redeEscolarId)
    {
        return !redeEscolarId.HasValue || redeEscolarId.Value != Guid.Empty;
    }
}

public class ValidadorCriarRedeEscolar : AbstractValidator<CriarRedeEscolarDto>
{
    public ValidadorCriarRedeEscolar()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Nome da rede escolar é obrigatório")
            .Length(2, 200)
            .WithMessage("Nome da rede escolar deve ter entre 2 e 200 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ0-9\s\-\.]+$")
            .WithMessage("Nome da rede escolar contém caracteres inválidos");

        RuleFor(x => x.CnpjMantenedora)
            .NotEmpty()
            .WithMessage("CNPJ da mantenedora é obrigatório")
            .Must(ValidarCnpj)
            .WithMessage("CNPJ da mantenedora inválido");

        RuleFor(x => x.EnderecoSede)
            .NotNull()
            .WithMessage("Endereço da sede é obrigatório")
            .SetValidator(new ValidadorEndereco());
    }

    private static bool ValidarCnpj(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        // Remove caracteres não numéricos
        cnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");

        // Verifica se tem 14 dígitos
        if (cnpj.Length != 14)
            return false;

        // Verifica se todos os dígitos são iguais
        if (cnpj.All(c => c == cnpj[0]))
            return false;

        // Validação do primeiro dígito verificador
        int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int soma = 0;
        for (int i = 0; i < 12; i++)
        {
            soma += int.Parse(cnpj[i].ToString()) * multiplicador1[i];
        }
        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cnpj[12].ToString()) != digito1)
            return false;

        // Validação do segundo dígito verificador
        int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        soma = 0;
        for (int i = 0; i < 13; i++)
        {
            soma += int.Parse(cnpj[i].ToString()) * multiplicador2[i];
        }
        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;

        return int.Parse(cnpj[13].ToString()) == digito2;
    }
}

public class ValidadorEndereco : AbstractValidator<EnderecoDto>
{
    public ValidadorEndereco()
    {
        RuleFor(x => x.Logradouro)
            .NotEmpty()
            .WithMessage("Logradouro é obrigatório")
            .Length(5, 200)
            .WithMessage("Logradouro deve ter entre 5 e 200 caracteres");

        RuleFor(x => x.Numero)
            .NotEmpty()
            .WithMessage("Número é obrigatório")
            .Length(1, 20)
            .WithMessage("Número deve ter entre 1 e 20 caracteres");

        RuleFor(x => x.Complemento)
            .MaximumLength(100)
            .WithMessage("Complemento deve ter no máximo 100 caracteres");

        RuleFor(x => x.Bairro)
            .NotEmpty()
            .WithMessage("Bairro é obrigatório")
            .Length(2, 100)
            .WithMessage("Bairro deve ter entre 2 e 100 caracteres");

        RuleFor(x => x.Cidade)
            .NotEmpty()
            .WithMessage("Cidade é obrigatória")
            .Length(2, 100)
            .WithMessage("Cidade deve ter entre 2 e 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s\-]+$")
            .WithMessage("Cidade contém caracteres inválidos");

        RuleFor(x => x.Estado)
            .NotEmpty()
            .WithMessage("Estado é obrigatório")
            .Length(2, 2)
            .WithMessage("Estado deve ter exatamente 2 caracteres")
            .Matches(@"^[A-Z]{2}$")
            .WithMessage("Estado deve conter apenas letras maiúsculas (ex: SP, RJ, MG)");

        RuleFor(x => x.Cep)
            .NotEmpty()
            .WithMessage("CEP é obrigatório")
            .Must(ValidarCep)
            .WithMessage("CEP inválido. Formato aceito: 12345-678 ou 12345678");
    }

    private static bool ValidarCep(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
            return false;

        // Remove caracteres não numéricos
        cep = cep.Replace("-", "");

        // Verifica se tem 8 dígitos
        if (cep.Length != 8)
            return false;

        // Verifica se todos são dígitos
        return cep.All(char.IsDigit);
    }
}

public class ValidadorAdicionarUnidadeEscolar : AbstractValidator<AdicionarUnidadeEscolarDto>
{
    public ValidadorAdicionarUnidadeEscolar()
    {
        RuleFor(x => x.EscolaId)
            .NotEmpty()
            .WithMessage("ID da escola é obrigatório");

        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Nome da unidade é obrigatório")
            .Length(2, 200)
            .WithMessage("Nome da unidade deve ter entre 2 e 200 caracteres");

        RuleFor(x => x.Endereco)
            .NotNull()
            .WithMessage("Endereço da unidade é obrigatório")
            .SetValidator(new ValidadorEndereco());

        RuleFor(x => x.CapacidadeMaximaAlunos)
            .GreaterThan(0)
            .WithMessage("Capacidade máxima deve ser maior que zero")
            .LessThanOrEqualTo(10000)
            .WithMessage("Capacidade máxima deve ser menor ou igual a 10.000");


    }
}