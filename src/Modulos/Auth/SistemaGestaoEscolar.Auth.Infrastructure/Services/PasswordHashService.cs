using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Auth.Domain.Services;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SistemaGestaoEscolar.Auth.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de hash de senha usando BCrypt.
/// Fornece funcionalidades seguras para hash e validação de senhas.
/// </summary>
public class PasswordHashService : IPasswordHashService
{
    private readonly ILogger<PasswordHashService> _logger;
    private const int DefaultWorkFactor = 12;

    // Regex patterns para validação de senha
    private static readonly Regex UpperCaseRegex = new(@"[A-Z]", RegexOptions.Compiled);
    private static readonly Regex LowerCaseRegex = new(@"[a-z]", RegexOptions.Compiled);
    private static readonly Regex DigitRegex = new(@"[0-9]", RegexOptions.Compiled);
    private static readonly Regex SpecialCharRegex = new(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]", RegexOptions.Compiled);

    // Senhas comuns que devem ser rejeitadas
    private static readonly HashSet<string> CommonPasswords = new(StringComparer.OrdinalIgnoreCase)
    {
        "password", "123456", "123456789", "12345678", "12345", "1234567", "password123",
        "admin", "qwerty", "abc123", "Password1", "welcome", "monkey", "1234567890",
        "dragon", "princess", "letmein", "654321", "superman", "qwerty123", "freedom",
        "696969", "batman", "master", "hello", "charlie", "aa123456", "donald", "password1",
        "qwertyuiop", "123123", "football", "secret", "admin123", "welcome123", "login",
        "starwars", "solo", "flower", "hottie", "loveme", "zaq1zaq1", "password123456",
        "temp", "guest", "123qwe", "zxcvbnm", "killer", "trustno1", "jordan", "jennifer",
        "hunter", "computer", "michelle", "tigger", "iloveyou", "2000", "charlie",
        "robert", "thomas", "hockey", "ranger", "daniel", "starwars", "klaster",
        "112233", "george", "asshole", "computer", "michelle", "jessica", "pepper",
        "1111", "zxcvbn", "555555", "11111111", "131313", "freedom", "777777", "pass",
        "fuck", "maggie", "159753", "aaaaaa", "ginger", "princess", "joshua", "cheese",
        "amanda", "summer", "love", "ashley", "6969", "nicole", "chelsea", "biteme",
        "matthew", "access", "yankees", "987654321", "dallas", "austin", "thunder",
        "taylor", "matrix", "william", "corvette", "hello", "martin", "heather", "secret",
        "fucker", "merlin", "diamond", "1234qwer", "gfhjkm", "hammer", "silver", "222222",
        "88888888", "anthony", "justin", "test", "bailey", "q1w2e3r4t5", "patrick", "internet",
        "scooter", "orange", "11111", "golfer", "cookie", "richard", "samantha", "bigdog",
        "guitar", "jackson", "whatever", "mickey", "chicken", "sparky", "snoopy", "maverick",
        "phoenix", "camaro", "sexy", "peanut", "morgan", "welcome", "falcon", "cowboy",
        "ferrari", "samsung", "andrea", "smokey", "steelers", "joseph", "mercedes", "dakota",
        "arsenal", "eagles", "melissa", "boomer", "booboo", "spider", "nascar", "monster",
        "tigers", "yellow", "xxxxxx", "123123123", "gateway", "marina", "diablo", "bulldog",
        "qwer1234", "compaq", "purple", "hardcore", "banana", "junior", "hannah", "123654",
        "porsche", "lakers", "iceman", "money", "cowboys", "987654", "london", "tennis",
        "999999", "ncc1701", "coffee", "scooby", "0000", "miller", "boston", "q1w2e3r4",
        "fuckoff", "brandon", "yamaha", "chester", "mother", "forever", "johnny", "edward",
        "333333", "oliver", "redsox", "player", "nikita", "knight", "fender", "barney",
        "midnight", "please", "brandy", "chicago", "badboy", "iwantu", "slayer", "rangers",
        "charles", "angel", "flower", "bigdaddy", "rabbit", "wizard", "bigdick", "jasper",
        "enter", "rachel", "chris", "steven", "winner", "adidas", "victoria", "natasha",
        "1q2w3e4r", "jasmine", "winter", "prince", "panties", "marine", "ghbdtn", "fishing",
        "cocacola", "casper", "james", "232323", "raiders", "888888", "marlboro", "gandalf",
        "asdfgh", "crystal", "87654321", "12344321", "sexsex", "golden", "blowme", "bigtits",
        "8675309", "panther", "lauren", "angela", "bitch", "spanky", "thx1138", "angels",
        "madison", "winston", "shannon", "mike", "toyota", "blowjob", "jordan23", "canada",
        "sophie", "Password", "apples", "dick", "tiger", "razz", "123abc", "pokemon", "qazxsw",
        "55555", "qwaszx", "muffin", "johnson", "murphy", "cooper", "jonathan", "liverpoo",
        "david", "danielle", "159357", "jackie", "1990", "123456a", "789456", "turtle",
        "horny", "abcd1234", "scorpion", "qazwsxedc", "101010", "butter", "carlos", "password1",
        "dennis", "slipknot", "qwerty123", "booger", "asdf", "1991", "black", "startrek",
        "12341234", "cameron", "newyork", "rainbow", "nathan", "john", "1992", "rocket",
        "viking", "redskins", "butthead", "asdfasdf", "beavis", "james1", "robert", "prince",
        "booty", "jennifer", "shit", "asshole", "green", "charlie1", "doggie", "irish",
        "lucky", "physical", "cool", "cooper", "1985", "chicken", "stupid", "shit", "monica",
        "bonnie", "concentrate", "little", "12many", "bitches", "michelle", "cowboys", "golden",
        "fire", "sandra", "pookie", "packers", "einstein", "dolphins", "0", "chevy", "winston",
        "warrior", "sammy", "slut", "8888", "zxcvbn", "nipples", "power", "victoria", "asdf1234",
        "vagina", "toyota", "travis", "hotdog", "paris", "rock", "xxxx", "extreme", "redskins",
        "erotic", "dirty", "ford", "freddy", "arsenal", "access14", "wolf", "nipple", "iloveu",
        "alex", "florida", "eric", "legend", "movie", "success", "rosebud", "jaguar", "great",
        "cool", "cooper", "1985", "chicken", "stupid", "monica", "bonnie", "concentrate",
        "little", "12many", "bitches", "michelle", "cowboys", "golden", "fire", "sandra"
    };

    public PasswordHashService(ILogger<PasswordHashService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gera um hash seguro para a senha
    /// </summary>
    public string HashPassword(string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Senha não pode ser vazia", nameof(password));

            var salt = GenerateSalt();
            var hash = BCrypt.Net.BCrypt.HashPassword(password, salt);
            
            _logger.LogDebug("Hash de senha gerado com sucesso");
            
            return hash;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar hash da senha");
            throw new InvalidOperationException("Erro ao processar senha", ex);
        }
    }

    /// <summary>
    /// Verifica se a senha corresponde ao hash
    /// </summary>
    public bool VerifyPassword(string password, string hash)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;

            var result = BCrypt.Net.BCrypt.Verify(password, hash);
            
            _logger.LogDebug("Verificação de senha: {Result}", result ? "Sucesso" : "Falha");
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao verificar senha");
            return false;
        }
    }

    /// <summary>
    /// Verifica se o hash precisa ser atualizado (para upgrade de segurança)
    /// </summary>
    public bool NeedsRehash(string hash)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(hash))
                return true;

            return BCrypt.Net.BCrypt.PasswordNeedsRehash(hash, DefaultWorkFactor);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao verificar necessidade de rehash");
            return true; // Em caso de erro, assume que precisa rehash
        }
    }

    /// <summary>
    /// Gera um salt aleatório
    /// </summary>
    public string GenerateSalt()
    {
        try
        {
            return BCrypt.Net.BCrypt.GenerateSalt(DefaultWorkFactor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar salt");
            throw new InvalidOperationException("Erro ao gerar salt", ex);
        }
    }

    /// <summary>
    /// Calcula a força de uma senha (0-100)
    /// </summary>
    public int CalculatePasswordStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return 0;

        int score = 0;

        // Comprimento (máximo 25 pontos)
        if (password.Length >= 8) score += 5;
        if (password.Length >= 10) score += 5;
        if (password.Length >= 12) score += 5;
        if (password.Length >= 16) score += 5;
        if (password.Length >= 20) score += 5;

        // Complexidade de caracteres (máximo 40 pontos)
        if (UpperCaseRegex.IsMatch(password)) score += 10;
        if (LowerCaseRegex.IsMatch(password)) score += 10;
        if (DigitRegex.IsMatch(password)) score += 10;
        if (SpecialCharRegex.IsMatch(password)) score += 10;

        // Diversidade de caracteres (máximo 15 pontos)
        var uniqueChars = password.Distinct().Count();
        var diversityRatio = (double)uniqueChars / password.Length;
        if (diversityRatio >= 0.7) score += 15;
        else if (diversityRatio >= 0.5) score += 10;
        else if (diversityRatio >= 0.3) score += 5;

        // Padrões avançados (máximo 20 pontos)
        if (!ContainsCommonSequences(password)) score += 5;
        if (!ContainsExcessiveRepetition(password)) score += 5;
        if (!ContainsDictionaryWords(password)) score += 5;
        if (ContainsVariedCharacterTypes(password)) score += 5;

        // Penalidades
        if (IsCommonPassword(password)) score -= 30;
        if (ContainsPersonalInfo(password)) score -= 20;
        if (IsKeyboardPattern(password)) score -= 15;

        return Math.Max(0, Math.Min(100, score));
    }

    /// <summary>
    /// Valida se uma senha atende aos critérios mínimos
    /// </summary>
    public PasswordValidationResult ValidatePassword(string password)
    {
        var errors = new List<string>();
        var suggestions = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Senha é obrigatória");
            return new PasswordValidationResult(false, 0, errors, suggestions);
        }

        // Validações básicas
        if (password.Length < 8)
        {
            errors.Add("Senha deve ter pelo menos 8 caracteres");
            suggestions.Add("Use pelo menos 8 caracteres");
        }

        if (password.Length > 128)
        {
            errors.Add("Senha não pode ter mais de 128 caracteres");
        }

        if (!UpperCaseRegex.IsMatch(password))
        {
            errors.Add("Senha deve conter pelo menos uma letra maiúscula");
            suggestions.Add("Adicione pelo menos uma letra maiúscula (A-Z)");
        }

        if (!LowerCaseRegex.IsMatch(password))
        {
            errors.Add("Senha deve conter pelo menos uma letra minúscula");
            suggestions.Add("Adicione pelo menos uma letra minúscula (a-z)");
        }

        if (!DigitRegex.IsMatch(password))
        {
            errors.Add("Senha deve conter pelo menos um número");
            suggestions.Add("Adicione pelo menos um número (0-9)");
        }

        if (!SpecialCharRegex.IsMatch(password))
        {
            errors.Add("Senha deve conter pelo menos um caractere especial");
            suggestions.Add("Adicione pelo menos um caractere especial (!@#$%^&*...)");
        }

        // Validações avançadas
        if (IsCommonPassword(password))
        {
            errors.Add("Esta senha é muito comum e insegura");
            suggestions.Add("Use uma senha única que não seja facilmente adivinhada");
        }

        if (ContainsCommonSequences(password))
        {
            errors.Add("Senha não pode conter sequências comuns (123, abc, qwerty, etc.)");
            suggestions.Add("Evite sequências óbvias como 123, abc ou qwerty");
        }

        if (ContainsExcessiveRepetition(password))
        {
            errors.Add("Senha não pode conter muitos caracteres repetidos consecutivos");
            suggestions.Add("Evite repetir o mesmo caractere mais de 2 vezes seguidas");
        }

        if (IsKeyboardPattern(password))
        {
            errors.Add("Senha não pode ser um padrão de teclado");
            suggestions.Add("Evite padrões de teclado como qwerty, asdf, 123456");
        }

        // Sugestões adicionais baseadas na força
        var strength = CalculatePasswordStrength(password);
        if (strength < 60)
        {
            suggestions.Add("Considere usar uma senha mais longa");
            suggestions.Add("Misture diferentes tipos de caracteres");
            suggestions.Add("Use uma frase secreta com espaços e números");
        }

        var isValid = !errors.Any();
        return new PasswordValidationResult(isValid, strength, errors, suggestions);
    }

    /// <summary>
    /// Gera uma senha temporária segura
    /// </summary>
    public string GenerateTemporaryPassword(int length = 12)
    {
        try
        {
            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            var allChars = upperCase + lowerCase + digits + specialChars;
            var password = new StringBuilder();

            using var rng = RandomNumberGenerator.Create();

            // Garantir pelo menos um caractere de cada tipo
            password.Append(GetRandomChar(upperCase, rng));
            password.Append(GetRandomChar(lowerCase, rng));
            password.Append(GetRandomChar(digits, rng));
            password.Append(GetRandomChar(specialChars, rng));

            // Preencher o restante aleatoriamente
            for (int i = 4; i < length; i++)
            {
                password.Append(GetRandomChar(allChars, rng));
            }

            // Embaralhar a senha
            var passwordArray = password.ToString().ToCharArray();
            for (int i = passwordArray.Length - 1; i > 0; i--)
            {
                var randomBytes = new byte[4];
                rng.GetBytes(randomBytes);
                var randomIndex = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % (i + 1);
                
                (passwordArray[i], passwordArray[randomIndex]) = (passwordArray[randomIndex], passwordArray[i]);
            }

            var result = new string(passwordArray);
            
            _logger.LogDebug("Senha temporária gerada com {Length} caracteres", length);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar senha temporária");
            throw new InvalidOperationException("Erro ao gerar senha temporária", ex);
        }
    }

    /// <summary>
    /// Verifica se uma senha está na lista de senhas comuns/vazadas
    /// </summary>
    public bool IsPasswordCompromised(string password)
    {
        try
        {
            // Verificar contra lista local de senhas comuns
            if (IsCommonPassword(password))
                return true;

            // TODO: Implementar verificação contra APIs como HaveIBeenPwned
            // Por enquanto, apenas verificação local
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao verificar se senha está comprometida");
            return false; // Em caso de erro, assume que não está comprometida
        }
    }

    #region Private Methods

    private static char GetRandomChar(string chars, RandomNumberGenerator rng)
    {
        var randomBytes = new byte[4];
        rng.GetBytes(randomBytes);
        var randomIndex = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % chars.Length;
        return chars[randomIndex];
    }

    private static bool IsCommonPassword(string password)
    {
        return CommonPasswords.Contains(password);
    }

    private static bool ContainsCommonSequences(string password)
    {
        var commonSequences = new[]
        {
            "123", "234", "345", "456", "567", "678", "789", "890",
            "abc", "bcd", "cde", "def", "efg", "fgh", "ghi", "hij",
            "qwe", "wer", "ert", "rty", "tyu", "yui", "uio", "iop",
            "asd", "sdf", "dfg", "fgh", "ghj", "hjk", "jkl",
            "zxc", "xcv", "cvb", "vbn", "bnm"
        };

        var lowerPassword = password.ToLowerInvariant();
        return commonSequences.Any(seq => lowerPassword.Contains(seq));
    }

    private static bool ContainsExcessiveRepetition(string password)
    {
        for (int i = 0; i < password.Length - 2; i++)
        {
            if (password[i] == password[i + 1] && password[i + 1] == password[i + 2])
                return true;
        }
        return false;
    }

    private static bool ContainsDictionaryWords(string password)
    {
        // Lista básica de palavras comuns em português e inglês
        var commonWords = new[]
        {
            "senha", "password", "admin", "user", "login", "sistema", "escola", "aluno",
            "professor", "diretor", "secretaria", "gestao", "educacao", "ensino",
            "casa", "amor", "vida", "trabalho", "familia", "brasil", "futebol",
            "home", "love", "life", "work", "family", "school", "teacher", "student"
        };

        var lowerPassword = password.ToLowerInvariant();
        return commonWords.Any(word => lowerPassword.Contains(word));
    }

    private static bool ContainsVariedCharacterTypes(string password)
    {
        int types = 0;
        if (UpperCaseRegex.IsMatch(password)) types++;
        if (LowerCaseRegex.IsMatch(password)) types++;
        if (DigitRegex.IsMatch(password)) types++;
        if (SpecialCharRegex.IsMatch(password)) types++;
        
        return types >= 3;
    }

    private static bool ContainsPersonalInfo(string password)
    {
        // Verificações básicas para informações pessoais comuns
        var personalPatterns = new[]
        {
            @"\b(19|20)\d{2}\b", // Anos (1900-2099)
            @"\b\d{2}/\d{2}/\d{4}\b", // Datas
            @"\b\d{11}\b", // CPF
            @"\b\d{8,9}\b" // Telefones
        };

        return personalPatterns.Any(pattern => Regex.IsMatch(password, pattern));
    }

    private static bool IsKeyboardPattern(string password)
    {
        var keyboardPatterns = new[]
        {
            "qwerty", "qwertyui", "asdf", "asdfgh", "zxcv", "zxcvbn",
            "123456", "1234567", "12345678", "987654", "9876543",
            "qazwsx", "wsxedc", "edcrfv", "rfvtgb", "tgbyhn", "yhnujm"
        };

        var lowerPassword = password.ToLowerInvariant();
        return keyboardPatterns.Any(pattern => lowerPassword.Contains(pattern));
    }

    #endregion
}