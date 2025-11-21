# SIPRI - Guia de Autenticação

Documentação completa sobre como funciona a autenticação no SIPRI usando **Keycloak** e **JWT (JSON Web Tokens)**.

---

## 📋 Índice

1. [Visão Geral](#-visão-geral)
2. [Arquitetura de Autenticação](#-arquitetura-de-autenticação)
3. [Keycloak - Configuração](#-keycloak---configuração)
4. [Obtendo um Token JWT](#-obtendo-um-token-jwt)
5. [Usando o Token nas Requisições](#-usando-o-token-nas-requisições)
6. [Validação de Token](#-validação-de-token)
7. [Tratamento de Erros](#-tratamento-de-erros)
8. [Fluxo Completo](#-fluxo-completo)
9. [Troubleshooting](#-troubleshooting)

---

## 🔐 Visão Geral

O SIPRI utiliza **autenticação baseada em JWT (JSON Web Tokens)** fornecidos pelo **Keycloak**, um servidor de gerenciamento de identidade e acesso open-source.

### Características Principais

- ✅ **OAuth 2.0 / OpenID Connect** - Padrões modernos de autenticação
- ✅ **JWT Tokens** - Tokens auto-contidos e stateless
- ✅ **Keycloak** - Gerenciamento centralizado de usuários e permissões
- ✅ **Bearer Authentication** - Tokens enviados no header `Authorization`
- ✅ **Validação Automática** - Middleware valida tokens em todas as requisições
- ✅ **Claims Customizadas** - Informações do usuário no token (sub, preferred_username, etc.)

---

## 🏗️ Arquitetura de Autenticação

```mermaid
sequenceDiagram
    participant Client as Cliente (Postman/App)
    participant Keycloak as Keycloak Server
    participant API as SIPRI API
    participant Middleware as Auth Middleware
    participant Controller as Controller

    Client->>Keycloak: 1. POST /token (credenciais)
    Keycloak->>Keycloak: 2. Valida credenciais
    Keycloak->>Client: 3. Retorna JWT Token

    Client->>API: 4. GET /endpoint (Authorization: Bearer {token})
    API->>Middleware: 5. Intercepta requisição
    Middleware->>Keycloak: 6. Valida token (chave pública)
    Keycloak->>Middleware: 7. Token válido
    Middleware->>Controller: 8. Permite acesso
    Controller->>Client: 9. Retorna dados
```

### Componentes

1. **Keycloak Server** - Gerencia autenticação e emite tokens JWT
2. **SIPRI API** - Valida tokens e protege endpoints
3. **JWT Middleware** - Intercepta requisições e valida tokens automaticamente
4. **Cliente** - Aplicação que consome a API (Postman, frontend, etc.)

---

## ⚙️ Keycloak - Configuração

### Informações do Realm

| Configuração      | Valor                                                                    |
| ----------------- | ------------------------------------------------------------------------ |
| **Realm**         | `sipri-realm`                                                            |
| **Client ID**     | `cli-sir-sipri`                                                          |
| **Client Secret** | `Z3T3Jz3QWZ1Hdb0TpyW8JTKXytnmAylR`                                       |
| **Authority**     | `http://localhost:8080/realms/sipri-realm`                               |
| **Token URL**     | `http://localhost:8080/realms/sipri-realm/protocol/openid-connect/token` |

### Acessando o Keycloak Admin

**URL:** `http://localhost:8080`

**Credenciais de Admin:**

- **Usuário:** `admin`
- **Senha:** `admin`

### Estrutura do Realm

O realm `sipri-realm` já vem pré-configurado com:

- ✅ Cliente `cli-sir-sipri` configurado
- ✅ Usuários de teste
- ✅ Roles e permissões
- ✅ Client Secret configurado

---

## 🎫 Obtendo um Token JWT

### Método 1: Client Credentials Flow (Recomendado para Testes)

Usado para autenticação **máquina-a-máquina** (service-to-service).

**Endpoint:**

```
POST http://localhost:8080/realms/sipri-realm/protocol/openid-connect/token
```

**Headers:**

```
Content-Type: application/x-www-form-urlencoded
```

**Body (x-www-form-urlencoded):**

```
grant_type=client_credentials
client_id=cli-sir-sipri
client_secret=Z3T3Jz3QWZ1Hdb0TpyW8JTKXytnmAylR
```

**Exemplo com cURL:**

```bash
curl -X POST "http://localhost:8080/realms/sipri-realm/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials" \
  -d "client_id=cli-sir-sipri" \
  -d "client_secret=Z3T3Jz3QWZ1Hdb0TpyW8JTKXytnmAylR"
```

**Resposta (200 OK):**

```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJxN...",
  "expires_in": 300,
  "refresh_expires_in": 0,
  "token_type": "Bearer",
  "not-before-policy": 0,
  "scope": "profile email"
}
```

---

### Método 2: Password Flow (Autenticação de Usuário)

Usado quando você tem **credenciais de usuário** (username/password).

**Body (x-www-form-urlencoded):**

```
grant_type=password
client_id=cli-sir-sipri
client_secret=Z3T3Jz3QWZ1Hdb0TpyW8JTKXytnmAylR
username=seu_usuario
password=sua_senha
```

**Exemplo com cURL:**

```bash
curl -X POST "http://localhost:8080/realms/sipri-realm/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=cli-sir-sipri" \
  -d "client_secret=Z3T3Jz3QWZ1Hdb0TpyW8JTKXytnmAylR" \
  -d "username=testuser" \
  -d "password=testpass123"
```

---

### Método 3: Usando Postman

1. **Criar uma nova Request**
2. **Configurar Authorization:**
   - Type: `OAuth 2.0`
   - Grant Type: `Client Credentials` ou `Password Credentials`
   - Access Token URL: `http://localhost:8080/realms/sipri-realm/protocol/openid-connect/token`
   - Client ID: `cli-sir-sipri`
   - Client Secret: `Z3T3Jz3QWZ1Hdb0TpyW8JTKXytnmAylR`
3. **Clicar em "Get New Access Token"**
4. **Usar o token gerado**

---

## 🔑 Usando o Token nas Requisições

### Formato do Header

Todas as requisições à API SIPRI devem incluir o token JWT no header `Authorization`:

```
Authorization: Bearer {seu_access_token}
```

### Exemplo Completo

**Request:**

```bash
curl -X POST "http://localhost:5000/simular-investimento" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJxN..." \
  -d '{
    "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "tipoProduto": "CDB",
    "valor": 10000.00,
    "prazoMeses": 12
  }'
```

### No Postman

1. Vá para a aba **Authorization**
2. Type: `Bearer Token`
3. Cole o `access_token` obtido do Keycloak
4. Envie a requisição

---

## ✅ Validação de Token

### Como a API Valida o Token

O SIPRI valida automaticamente cada token JWT usando o **middleware de autenticação**:

```csharp
// Configuração em DependencyInjection.cs
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8080/realms/sipri-realm";
        options.Audience = "cli-sir-sipri";
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero // Sem tolerância de expiração
        };
    });
```

### Validações Realizadas

1. ✅ **Assinatura** - Verifica se o token foi assinado pelo Keycloak
2. ✅ **Issuer** - Valida se o emissor é o realm correto
3. ✅ **Audience** - Confirma se o token é para este cliente
4. ✅ **Expiração** - Verifica se o token ainda é válido (não expirou)
5. ✅ **Chave de Assinatura** - Valida a chave pública do Keycloak

### Estrutura do Token JWT

Um token JWT é composto por 3 partes separadas por `.`:

```
eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
```

**Decodificado:**

**Header:**

```json
{
  "alg": "RS256",
  "typ": "JWT",
  "kid": "q7..."
}
```

**Payload (Claims):**

```json
{
  "sub": "service-account-cli-sir-sipri",
  "aud": "cli-sir-sipri",
  "iss": "http://localhost:8080/realms/sipri-realm",
  "exp": 1732220400,
  "iat": 1732220100,
  "preferred_username": "service-account-cli-sir-sipri",
  "email_verified": false,
  "scope": "profile email"
}
```

**Signature:** Assinatura criptográfica (RS256)

> 💡 **Dica:** Use [jwt.io](https://jwt.io) para decodificar e inspecionar tokens JWT.

---

## ⚠️ Tratamento de Erros

### Erros Comuns de Autenticação

#### 1. Token Ausente (401 Unauthorized)

**Causa:** Header `Authorization` não foi enviado.

**Resposta:**

```json
{
  "type": "https://httpstatuses.com/401",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Token de autenticação ausente ou inválido.",
  "instance": "/simular-investimento",
  "traceId": "00-abc123..."
}
```

**Solução:** Adicione o header `Authorization: Bearer {token}`

---

#### 2. Token Expirado (401 Unauthorized)

**Causa:** O token JWT expirou (padrão: 5 minutos).

**Resposta:**

```json
{
  "type": "https://httpstatuses.com/401",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Token de autenticação ausente ou inválido.",
  "instance": "/simular-investimento",
  "traceId": "00-xyz789..."
}
```

**Headers Adicionais:**

```
Token-Expired: true
```

**Solução:** Obtenha um novo token do Keycloak.

---

#### 3. Token Inválido (401 Unauthorized)

**Causa:** Token malformado, assinatura inválida ou audience incorreto.

**Logs da API:**

```
[Error] Autenticação falhou: Token inválido ou erro de validação.
```

**Solução:** Verifique se:

- O token foi copiado corretamente (sem espaços extras)
- O token foi obtido do realm correto (`sipri-realm`)
- O `client_id` está correto

---

#### 4. Permissão Negada (403 Forbidden)

**Causa:** Token válido, mas usuário não tem permissão para o recurso.

**Resposta:**

```json
{
  "type": "https://httpstatuses.com/403",
  "title": "Acesso Negado",
  "status": 403,
  "detail": "Você não tem permissão para acessar este recurso.",
  "instance": "/admin/configuracoes",
  "traceId": "00-def456..."
}
```

**Logs da API:**

```
[Warning] Acesso proibido (403): Usuário autenticado, mas sem permissão para este recurso.
```

**Solução:** Verifique as roles/permissões do usuário no Keycloak.

---

## 🔄 Fluxo Completo

### Passo a Passo: Da Autenticação à Requisição

#### **Passo 1: Obter Token do Keycloak**

```bash
curl -X POST "http://localhost:8080/realms/sipri-realm/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials" \
  -d "client_id=cli-sir-sipri" \
  -d "client_secret=Z3T3Jz3QWZ1Hdb0TpyW8JTKXytnmAylR"
```

**Resposta:**

```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expires_in": 300,
  "token_type": "Bearer"
}
```

---

#### **Passo 2: Extrair o Access Token**

Copie o valor de `access_token` da resposta.

---

#### **Passo 3: Fazer Requisição à API**

```bash
curl -X POST "http://localhost:5000/simular-investimento" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -d '{
    "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "tipoProduto": "CDB",
    "valor": 10000.00,
    "prazoMeses": 12
  }'
```

---

#### **Passo 4: API Valida o Token**

1. Middleware intercepta a requisição
2. Extrai o token do header `Authorization`
3. Valida assinatura com a chave pública do Keycloak
4. Verifica issuer, audience, expiração
5. Se válido, permite acesso ao controller

---

#### **Passo 5: Resposta da API**

```json
{
  "dataSimulacao": "2025-11-21T19:42:00Z",
  "produtoValidado": {
    "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "nome": "CDB Prefixado",
    "tipo": "CDB",
    "rentabilidade": 0.13,
    "risco": "Baixo"
  },
  "resultadoSimulacao": {
    "valorFinal": 11300.0,
    "prazoMeses": 12,
    "rentabilidadeEfetiva": 0.13
  }
}
```

---

## 🛠️ Troubleshooting

### Problema: "Keycloak não está acessível"

**Sintomas:**

- Erro ao obter token: `Connection refused`
- API não consegue validar tokens

**Soluções:**

1. Verifique se o Keycloak está rodando:
   ```bash
   docker ps | grep keycloak
   ```
2. Acesse `http://localhost:8080` no navegador
3. Reinicie o container:
   ```bash
   docker-compose restart sipri-keycloak
   ```

---

### Problema: "Token sempre retorna 401"

**Possíveis Causas:**

1. **Token mal formatado:**

   - Verifique se há espaços extras
   - Certifique-se de usar `Bearer {token}` (com espaço)

2. **Configuração incorreta:**

   - Verifique `appsettings.json`:
     ```json
     "Authentication": {
       "Authority": "http://localhost:8080/realms/sipri-realm",
       "Audience": "cli-sir-sipri"
     }
     ```

3. **Realm ou Client ID errado:**
   - Confirme que está usando `sipri-realm` e `cli-sir-sipri`

---

### Problema: "Token expira muito rápido"

**Solução:**

Ajuste o tempo de expiração no Keycloak:

1. Acesse Keycloak Admin Console
2. Vá para `Realm Settings` → `Tokens`
3. Ajuste `Access Token Lifespan` (padrão: 5 minutos)
4. Salve as alterações

---

### Problema: "CORS ao chamar Keycloak do frontend"

**Solução:**

Configure CORS no Keycloak:

1. Acesse o cliente `cli-sir-sipri` no Keycloak
2. Vá para `Settings`
3. Em `Web Origins`, adicione: `http://localhost:3000` (ou sua origem)
4. Salve

---

## 📚 Referências

- [Keycloak Documentation](https://www.keycloak.org/documentation)
- [JWT.io - Decodificador de Tokens](https://jwt.io)
- [OAuth 2.0 Specification](https://oauth.net/2/)
- [OpenID Connect](https://openid.net/connect/)
- [RFC 7519 - JSON Web Token](https://tools.ietf.org/html/rfc7519)

---

## 🔐 Segurança - Boas Práticas

### ✅ Recomendações

1. **Nunca exponha o Client Secret** em código frontend
2. **Use HTTPS em produção** (`RequireHttpsMetadata = true`)
3. **Implemente refresh tokens** para sessões longas
4. **Configure roles e permissões** no Keycloak
5. **Monitore tokens expirados** e renove automaticamente
6. **Use variáveis de ambiente** para secrets (`.env`)
7. **Habilite rate limiting** para prevenir ataques de força bruta
8. **Implemente logout** para invalidar tokens

### ⚠️ Avisos

- ❌ **Não compartilhe tokens** entre diferentes aplicações
- ❌ **Não armazene tokens em localStorage** (use httpOnly cookies)
- ❌ **Não use tokens expirados** (sempre renove)
- ❌ **Não desabilite validação de HTTPS em produção**

---

## 📞 Suporte

Para problemas de autenticação:

1. Verifique os logs da API: `docker logs sipri-api`
2. Verifique os logs do Keycloak: `docker logs sipri-keycloak`
3. Consulte este guia de troubleshooting
4. Entre em contato com Diego da Rosa
