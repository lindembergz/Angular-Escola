# Sistema de Gestão Escolar - Módulo de Autenticação - Camada de Aplicação

## Visão Geral

Esta camada implementa os casos de uso e serviços de aplicação para o módulo de autenticação, seguindo os princípios de Clean Architecture e Domain-Driven Design (DDD).

## Estrutura

### Casos de Uso (Use Cases)

- **LoginUseCase**: Gerencia o processo de autenticação de usuários
- **LogoutUseCase**: Gerencia o processo de logout e invalidação de sessões
- **RefreshTokenUseCase**: Gerencia a renovação de tokens de acesso
- **ChangePasswordUseCase**: Gerencia a alteração de senhas
- **ForgotPasswordUseCase**: Gerencia solicitações de recuperação de senha
- **ResetPasswordUseCase**: Gerencia a redefinição de senhas via token
- **ConfirmEmailUseCase**: Gerencia a confirmação de emails
- **ResendEmailConfirmationUseCase**: Gerencia o reenvio de confirmações de email

### Serviços de Aplicação

- **AuthApplicationService**: Serviço principal que orquestra os casos de uso
- **AuthValidationService**: Serviço de validação para operações complexas

### DTOs (Data Transfer Objects)

- **LoginDto**: Dados para requisição de login
- **AuthResponseDto**: Resposta de autenticação com tokens e informações do usuário
- **RefreshTokenDto**: Dados para renovação de token
- **ChangePasswordDto**: Dados para alteração de senha
- **ForgotPasswordDto**: Dados para solicitação de recuperação de senha
- **ResetPasswordDto**: Dados para redefinição de senha

### Interfaces

- **IAuthApplicationService**: Interface principal do serviço de aplicação
- **ITokenService**: Interface para serviços de token JWT

## Funcionalidades Implementadas

### ✅ Autenticação
- Login com email e senha
- Validação de credenciais
- Geração de tokens JWT
- Controle de sessões
- Proteção contra ataques de força bruta

### ✅ Autorização
- Controle de papéis (roles)
- Permissões baseadas em contexto
- Validação de acesso a recursos

### ✅ Gestão de Senhas
- Alteração de senha
- Recuperação de senha via email
- Validação de força de senha
- Proteção contra senhas comprometidas

### ✅ Gestão de Tokens
- Geração de access tokens
- Geração de refresh tokens
- Renovação automática de tokens
- Invalidação de tokens

### ✅ Gestão de Sessões
- Controle de sessões ativas
- Invalidação de sessões
- Detecção de sessões suspeitas
- Logout de sessões específicas

### ✅ Confirmação de Email
- Envio de tokens de confirmação
- Validação de emails
- Reenvio de confirmações

## Princípios Aplicados

### Clean Architecture
- Separação clara de responsabilidades
- Dependências apontam para dentro (Domain)
- Independência de frameworks externos

### Domain-Driven Design (DDD)
- Casos de uso representam operações de negócio
- Validações de domínio centralizadas
- Eventos de domínio para comunicação

### SOLID
- **Single Responsibility**: Cada caso de uso tem uma responsabilidade específica
- **Open/Closed**: Extensível via novos casos de uso
- **Liskov Substitution**: Interfaces bem definidas
- **Interface Segregation**: Interfaces específicas e coesas
- **Dependency Inversion**: Dependência de abstrações

## Segurança

### Proteções Implementadas
- Validação rigorosa de entrada
- Proteção contra ataques de força bruta
- Detecção de IPs suspeitos
- Validação de tokens com expiração
- Hash seguro de senhas (BCrypt)
- Proteção contra senhas comprometidas

### Auditoria
- Log de todas as operações críticas
- Rastreamento de tentativas de login
- Registro de alterações de senha
- Monitoramento de sessões suspeitas

## Configuração

Para usar esta camada de aplicação, registre os serviços usando o método de extensão:

```csharp
services.AddAuthApplication();
```

## Dependências

- **Domain Layer**: Para entidades, value objects e interfaces
- **Shared Application**: Para interfaces base e utilitários
- **Microsoft.Extensions.Logging**: Para logging
- **FluentValidation**: Para validações complexas
- **System.IdentityModel.Tokens.Jwt**: Para manipulação de tokens JWT

## Próximos Passos

- Integração com serviço de email para envio de tokens
- Implementação de autenticação de dois fatores (2FA)
- Integração com provedores externos (OAuth, SAML)
- Implementação de políticas de senha mais avançadas
- Auditoria completa com Event Sourcing