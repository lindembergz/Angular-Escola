# Sistema de Gestão Escolar - Módulo de Autenticação - Camada de Infraestrutura

## Visão Geral

Esta camada implementa os detalhes técnicos e de infraestrutura para o módulo de autenticação, incluindo persistência de dados, serviços externos, controladores Web API e configurações de segurança.

## Estrutura

### Controllers
- **AuthController**: Endpoints RESTful para operações de autenticação
  - `POST /api/auth/login` - Login de usuário
  - `POST /api/auth/logout` - Logout de usuário
  - `POST /api/auth/refresh` - Renovação de token
  - `POST /api/auth/change-password` - Alteração de senha
  - `POST /api/auth/forgot-password` - Solicitação de recuperação de senha
  - `POST /api/auth/reset-password` - Redefinição de senha
  - `GET /api/auth/confirm-email` - Confirmação de email
  - `POST /api/auth/resend-confirmation` - Reenvio de confirmação
  - `GET /api/auth/me` - Informações do usuário atual
  - `GET /api/auth/check-email` - Verificação de disponibilidade de email
  - `POST /api/auth/validate-password` - Validação de força de senha
  - `POST /api/auth/invalidate-sessions` - Invalidação de todas as sessões

### Repositórios
- **MySqlUserRepository**: Implementação do repositório de usuários para MySQL
- **MySqlSessionRepository**: Implementação do repositório de sessões para MySQL

### Serviços
- **TokenService**: Geração e validação de tokens JWT
- **PasswordHashService**: Hash e validação de senhas com BCrypt
- **AuthDomainService**: Lógica de domínio para autenticação

### Context
- **AuthDbContext**: Contexto do Entity Framework para o módulo de autenticação

### Mappings
- **UserMapping**: Configuração de mapeamento para entidade User
- **SessionMapping**: Configuração de mapeamento para entidade Session

### Middleware
- **JwtMiddleware**: Middleware para validação automática de tokens JWT

### Validators
- **LoginDtoValidator**: Validação para dados de login
- **RefreshTokenDtoValidator**: Validação para refresh token
- **ChangePasswordDtoValidator**: Validação para alteração de senha
- **ForgotPasswordDtoValidator**: Validação para recuperação de senha
- **ResetPasswordDtoValidator**: Validação para redefinição de senha

## Funcionalidades Implementadas

### ✅ Persistência de Dados
- Entity Framework Core com MySQL
- Mapeamentos otimizados para performance
- Índices para consultas frequentes
- Soft delete para usuários
- Timestamps automáticos

### ✅ Autenticação JWT
- Geração de tokens seguros
- Validação de tokens
- Refresh tokens
- Tokens específicos para recuperação de senha
- Tokens específicos para confirmação de email
- Configuração de expiração flexível

### ✅ Segurança
- Hash de senhas com BCrypt (work factor 12)
- Proteção contra ataques de força bruta
- Detecção de IPs suspeitos
- Validação rigorosa de senhas
- Rate limiting configurável
- CORS configurado

### ✅ Validação
- FluentValidation para DTOs
- Validações de domínio
- Validações de segurança
- Mensagens de erro localizadas

### ✅ Logging e Auditoria
- Logging estruturado com Serilog
- Auditoria de tentativas de login
- Rastreamento de sessões
- Logs de segurança

### ✅ Cache e Performance
- Suporte a Redis para cache distribuído
- Fallback para cache em memória
- Otimizações de consulta
- Pool de conexões configurado

## Configuração

### Banco de Dados

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DBEscolar;Uid=root;Pwd=password;",
    "Redis": "localhost:6379"
  }
}
```

### JWT

```json
{
  "Jwt": {
    "Secret": "sua-chave-secreta-muito-segura-com-pelo-menos-32-caracteres",
    "Issuer": "SistemaGestaoEscolar",
    "Audience": "SistemaGestaoEscolar.Users",
    "ExpirationInMinutes": 60,
    "PasswordResetSecret": "chave-para-reset-de-senha",
    "EmailConfirmationSecret": "chave-para-confirmacao-email"
  }
}
```

### CORS

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:4200",
      "https://localhost:4200",
      "https://app.sistemagestaoescolar.com"
    ]
  }
}
```

### Rate Limiting

```json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "POST:/api/auth/login",
        "Period": "1m",
        "Limit": 5
      },
      {
        "Endpoint": "POST:/api/auth/forgot-password",
        "Period": "1h",
        "Limit": 3
      }
    ]
  }
}
```

## Uso

### Registro dos Serviços

```csharp
// Program.cs ou Startup.cs
services.AddAuthInfrastructure(configuration);

// Configure o pipeline
app.UseAuthInfrastructure();
```

### Exemplo de Uso do Controller

```csharp
// Login
var loginDto = new LoginDto
{
    Email = "usuario@exemplo.com",
    Password = "MinhaSenh@123",
    RememberMe = true
};

var response = await authController.Login(loginDto);
```

### Proteção de Endpoints

```csharp
[Authorize] // Requer autenticação
[Authorize(Policy = "Admin")] // Requer papel de Admin
[Authorize(Policy = "EmailConfirmed")] // Requer email confirmado
public class MinhaController : ControllerBase
{
    // Seus endpoints protegidos
}
```

## Migrations

Para criar e aplicar migrations:

```bash
# Adicionar migration
dotnet ef migrations add InitialCreate --project SistemaGestaoEscolar.Auth.Infrastructure

# Aplicar migrations
dotnet ef database update --project SistemaGestaoEscolar.Auth.Infrastructure
```

## Testes

### Testes de Integração

```csharp
[Test]
public async Task Login_ValidCredentials_ReturnsToken()
{
    // Arrange
    var loginDto = new LoginDto { Email = "test@test.com", Password = "Test@123" };
    
    // Act
    var result = await _authController.Login(loginDto);
    
    // Assert
    Assert.IsType<OkObjectResult>(result);
}
```

### Testes de Repositório

```csharp
[Test]
public async Task GetByEmailAsync_ExistingUser_ReturnsUser()
{
    // Arrange
    var email = new Email("test@test.com");
    
    // Act
    var user = await _userRepository.GetByEmailAsync(email);
    
    // Assert
    Assert.NotNull(user);
    Assert.Equal(email.Value, user.Email.Value);
}
```

## Segurança

### Práticas Implementadas

- **Hash de Senhas**: BCrypt com work factor 12
- **Tokens JWT**: Assinados com chave secreta forte
- **Rate Limiting**: Proteção contra ataques de força bruta
- **Validação de Entrada**: Sanitização e validação rigorosa
- **Logs de Auditoria**: Rastreamento de atividades suspeitas
- **CORS**: Configurado para origens específicas
- **HTTPS**: Recomendado para produção

### Recomendações de Produção

1. **Chaves Secretas**: Use chaves diferentes para cada ambiente
2. **HTTPS**: Sempre use HTTPS em produção
3. **Rate Limiting**: Configure limites apropriados
4. **Monitoring**: Configure alertas para atividades suspeitas
5. **Backup**: Configure backup automático do banco de dados
6. **Logs**: Configure logging centralizado

## Troubleshooting

### Problemas Comuns

1. **Token Inválido**: Verifique se a chave secreta está correta
2. **CORS**: Verifique se a origem está na lista permitida
3. **Banco de Dados**: Verifique a connection string
4. **Migrations**: Execute as migrations pendentes

### Logs Úteis

```bash
# Verificar logs de autenticação
grep "Authentication" logs/app.log

# Verificar tentativas de login falhadas
grep "Login failed" logs/app.log

# Verificar tokens inválidos
grep "Invalid token" logs/app.log
```

## Performance

### Otimizações Implementadas

- Índices otimizados no banco de dados
- Cache de sessões com Redis
- Pool de conexões configurado
- Consultas otimizadas com Entity Framework
- Lazy loading desabilitado onde apropriado

### Métricas Recomendadas

- Tempo de resposta dos endpoints de autenticação
- Taxa de sucesso/falha de login
- Número de sessões ativas
- Uso de cache
- Tempo de consulta ao banco de dados