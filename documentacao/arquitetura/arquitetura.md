# SIPRI - Sistema de Investimentos e Perfil de Risco

API REST para simulação de investimentos, cálculo de perfil de risco e recomendação de produtos financeiros baseada em **Clean Architecture** e **CQRS**.

---

## 📋 Índice

1. [Visão Geral](#-visão-geral)
2. [Arquitetura do Sistema](#-arquitetura-do-sistema)
3. [Tecnologias](#-tecnologias)
4. [Configuração e Execução](#-configuração-e-execução)
5. [Endpoints da API](#-endpoints-da-api)
6. [Autenticação](#-autenticação)
7. [Testes](#-testes)
8. [Documentação Adicional](#-documentação-adicional)

---

## 🎯 Visão Geral

O **SIPRI** (Sistema de Investimentos e Perfil de Risco Inteligente) é uma API REST que permite:

- ✅ **Simular investimentos** com cálculo automático de rentabilidade
- ✅ **Calcular perfil de risco** baseado no histórico do investidor
- ✅ **Recomendar produtos** adequados ao perfil de cada cliente
- ✅ **Gerenciar carteira** de investimentos
- ✅ **Visualizar métricas** e dados agregados

### Princípios de Design

- **Clean Architecture** - Separação clara de responsabilidades em camadas
- **CQRS** - Commands e Queries separados para melhor organização
- **Domain-Driven Design** - Lógica de negócio rica no domínio
- **Strategy Pattern** - Cálculos flexíveis por tipo de produto
- **Result Pattern** - Tratamento de erros sem exceções desnecessárias

---

## 🏗️ Arquitetura do Sistema

### Diagrama de Contexto (Nível 1)

Visão de alto nível mostrando os principais atores e sistemas externos:

![Diagrama de Contexto - Nível 1](diagramas/diagrama-contexto-nivel-1.png)

**Componentes:**

- **Investidor** - Usuário autenticado que consome a API
- **SIPRI Backend API** - Sistema principal que gerencia cálculos e dados
- **Keycloak (IdP)** - Provedor de identidade OAuth2/OIDC para autenticação
- **SQL Server** - Banco de dados relacional para persistência

---

### Diagrama de Container (Nível 2)

Arquitetura detalhada mostrando a topologia Docker e comunicação entre containers:

![Diagrama de Container - Nível 2](diagramas/diagrama-container-nivel-2.png)

**Containers:**

- **Frontend/Swagger UI** - Interface para testes (porta 5058)
- **SIPRI Backend API** - API .NET 8.0 (porta 5058 externa, 80 interna)
- **Keycloak IDP** - Servidor de autenticação (porta 8080)
- **SQL Server** - Banco de dados (porta 1433)

**Comunicação:**

- Usuário → Swagger → Keycloak (autenticação PKCE)
- Swagger → API (HTTPS/JSON com Bearer Token)
- API → Keycloak (validação de assinatura JWT via JWKS)
- API → SQL Server (TCP/IP via Entity Framework Core)

---

### Fluxo de Autenticação

Diagrama de sequência mostrando o fluxo completo OAuth2 com PKCE:

![Diagrama de Sequência - Autenticação](diagramas/diagrama-sequencia-autenticacao.png)

**Fases:**

1. **Aquisição de Token (PKCE)**

   - Cliente gera `code_verifier` e `code_challenge`
   - Usuário faz login no Keycloak
   - Keycloak retorna Authorization Code
   - Cliente troca código por Access Token (JWT)

2. **Validação e Consumo**
   - Cliente envia requisição com `Authorization: Bearer {token}`
   - Middleware valida assinatura (JWKS)
   - Middleware valida claims (issuer, audience, expiração)
   - Se válido, permite acesso ao controller

> 📖 **Documentação Completa:** [AUTHENTICATION.md](AUTHENTICATION.md)

---

### Fluxo de Simulação de Investimento

Diagrama mostrando o **Strategy Pattern** em ação:

![Diagrama de Sequência - Simulação](diagramas/simulacao-investimento.png)

**Fluxo:**

1. **Validação** - FluentValidation valida dados de entrada
2. **Busca de Produto** - Repository busca produto por tipo (CDB, Fundo, etc.)
3. **Cálculo (Strategy)** - `CalculadoraService` seleciona estratégia correta
   - `RegraCalculoCDB` para CDB
   - `RegraCalculoFundo` para Fundos
4. **Persistência** - Salva simulação no banco via UnitOfWork
5. **Resposta** - Retorna DTO com resultado calculado

**Padrões Aplicados:**

- ✅ **CQRS** - Command separado da Query
- ✅ **MediatR** - Mediação entre Controller e Handler
- ✅ **Strategy Pattern** - Cálculo polimórfico por tipo de produto
- ✅ **Repository Pattern** - Abstração de acesso a dados
- ✅ **Unit of Work** - Transações consistentes

---

### Fluxo de Cálculo de Perfil de Risco

Diagrama mostrando o **Motor de Regras** de pontuação:

![Diagrama de Sequência - Perfil de Risco](diagramas/calculo-perfil-risco.png)

**Fluxo:**

1. **Hidratação** - Busca histórico de investimentos do cliente
2. **Motor de Regras** - `MotorPerfilRiscoServico` orquestra cálculo
3. **Pontuação Iterativa** - Aplica cada regra injetada:
   - `RegraPontuacaoVolume` - Pontos por volume investido
   - `RegraPontuacaoFrequencia` - Pontos por frequência de investimentos
   - `RegraPontuacaoPreferencia` - Pontos por tipos de produtos escolhidos
4. **Classificação** - Value Object `PerfilRisco` classifica:
   - **Conservador** (< 40 pontos)
   - **Moderado** (40-70 pontos)
   - **Arrojado** (> 70 pontos)

**Padrões Aplicados:**

- ✅ **Strategy Pattern** - Múltiplas regras de pontuação
- ✅ **Value Object** - `PerfilRisco` com lógica de classificação
- ✅ **Dependency Injection** - Regras injetadas via DI

---

### Modelo de Dados (DER)

Diagrama Entidade-Relacionamento do banco de dados:

![Diagrama Entidade-Relacionamento](diagramas/diagrama-entidade-relacionamento-der.png)

**Entidades Principais:**

- **ProdutoInvestimento** - Catálogo de produtos (CDB, Fundos, LCI, etc.)
- **Investimento** - Investimentos realizados pelo cliente
- **Simulacao** - Histórico de simulações
- **Cliente** - Dados do investidor (gerenciado pelo Keycloak)

**Relacionamentos:**

- Cliente → Investimentos (1:N)
- Cliente → Simulações (1:N)
- ProdutoInvestimento → Investimentos (1:N)
- ProdutoInvestimento → Simulações (1:N)

---

## 🚀 Tecnologias

### Backend

- **.NET 8.0** - Framework principal
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM
- **MediatR** - Mediação CQRS
- **FluentValidation** - Validação de requests
- **Keycloak** - Autenticação OAuth2/OIDC

### Arquitetura

- **Clean Architecture** - Separação em camadas
- **CQRS** - Commands/Queries/Handlers
- **Domain-Driven Design** - Lógica rica no domínio
- **Strategy Pattern** - Cálculos polimórficos
- **Repository Pattern** - Abstração de dados

### Infraestrutura

- **Docker & Docker Compose** - Containerização
- **SQL Server 2022** - Banco de dados
- **Swagger/OpenAPI** - Documentação interativa

---

## 🔧 Configuração e Execução

### Pré-requisitos

- Docker e Docker Compose
- .NET 8.0 SDK (opcional, para desenvolvimento local)

### Executar com Docker Compose

```bash
# Clone o repositório
git clone https://github.com/seu-usuario/sipri.git
cd sipri

# Inicie todos os containers
docker-compose up --build

# A API estará disponível em:
# - API: http://localhost:5058
# - Swagger: http://localhost:5058/swagger
# - Keycloak: http://localhost:8080
```

### Executar Localmente (Desenvolvimento)

```bash
# Restaurar dependências
dotnet restore

# Aplicar migrations
dotnet ef database update --project src/SIPRI.Infrastructure --startup-project src/SIPRI.Host

# Executar a API
cd src/SIPRI.Host
dotnet run

# Acessar Swagger
# http://localhost:5058/swagger
```

---

## 📚 Endpoints da API

### 1. Simular Investimento

**POST** `/simular-investimento`

Realiza simulação de investimento com cálculo de rentabilidade.

**Request:**

```json
{
  "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "tipoProduto": "CDB",
  "valor": 10000.0,
  "prazoMeses": 12
}
```

**Response (200 OK):**

```json
{
  "dataSimulacao": "2025-11-21T19:30:00Z",
  "produtoValidado": {
    "id": "7c9e6679-7425-40de-944b-e07fc-1f90ae7",
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

### 2. Calcular Perfil de Risco

**GET** `/perfil-risco/{clienteId}`

Calcula perfil de risco baseado no histórico de investimentos.

**Response (200 OK):**

```json
{
  "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "perfil": "Moderado",
  "pontuacao": 65.5,
  "descricao": "Investidor com tolerância moderada a riscos."
}
```

---

### 3. Obter Produtos Recomendados

**GET** `/produtos-recomendados/{perfil}`

Lista produtos adequados para um perfil de risco.

**Perfis:** `Conservador`, `Moderado`, `Arrojado`

**Response (200 OK):**

```json
[
  {
    "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "nome": "CDB Prefixado",
    "tipo": "CDB",
    "rentabilidade": 0.13,
    "risco": "Baixo"
  }
]
```

---

### 4. Listar Histórico de Simulações

**GET** `/simulacoes?clienteId={guid}`

Retorna histórico de simulações do cliente.

---

### 5. Obter Dados Agregados

**GET** `/simulacoes/por-produto-dia`

Estatísticas agregadas por produto e dia.

---

### 6. Listar Investimentos

**GET** `/investimentos/{clienteId}`

Retorna carteira de investimentos do cliente.

---

### 7. Telemetria

**GET** `/telemetria`

Métricas de uso e performance da API.

---

## 🔐 Autenticação

Todos os endpoints requerem autenticação via **Bearer Token (JWT)**.

### Obter Token

```bash
curl -X POST "http://localhost:8080/realms/sipri-realm/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials" \
  -d "client_id=cli-sir-sipri" \
  -d "client_secret=Z3T3Jz3QWZ1Hdb0TpyW8JTKXytnmAylR"
```

### Usar Token

```bash
curl -X GET "http://localhost:5058/perfil-risco/3fa85f64-5717-4562-b3fc-2c963f66afa6" \
  -H "Authorization: Bearer {seu_access_token}"
```

> 📖 **Guia Completo:** [AUTHENTICATION.md](AUTHENTICATION.md)

---

## 🧪 Testes

### Executar Todos os Testes

```bash
dotnet test
```

### Executar Testes por Projeto

```bash
# Testes de Domínio
dotnet test tests/SIPRI.Domain.Tests

# Testes de Aplicação
dotnet test tests/SIPRI.Application.Tests

# Testes de Infraestrutura
dotnet test tests/SIPRI.Infrastructure.Tests

# Testes de Apresentação
dotnet test tests/SIPRI.Presentation.Tests
```

### Cobertura de Testes

- ✅ **Domain** - Testes de entidades, value objects e serviços de domínio
- ✅ **Application** - Testes de handlers, validators e commands/queries
- ✅ **Infrastructure** - Testes de repositórios e persistência
- ✅ **Presentation** - Testes de controllers e middleware

---

## 📊 Códigos de Status HTTP

| Código | Significado           | Quando Ocorre             |
| ------ | --------------------- | ------------------------- |
| 200    | OK                    | Requisição bem-sucedida   |
| 400    | Bad Request           | Validação falhou          |
| 401    | Unauthorized          | Token ausente ou inválido |
| 403    | Forbidden             | Sem permissão             |
| 404    | Not Found             | Recurso não encontrado    |
| 500    | Internal Server Error | Erro no servidor          |

**Formato de Erro (RFC 7807):**

```json
{
  "type": "https://httpstatuses.com/400",
  "title": "Erro de Validação",
  "status": 400,
  "detail": "A requisição falhou na validação.",
  "instance": "/simular-investimento",
  "traceId": "00-abc123...",
  "errors": {
    "RequestData.Valor": ["O valor deve ser maior que zero."]
  }
}
```

---

## 📖 Documentação Adicional

- **[AUTHENTICATION.md](AUTHENTICATION.md)** - Guia completo de autenticação OAuth2/JWT
- **[Swagger UI](http://localhost:5058/swagger)** - Documentação interativa da API
- **[Keycloak Admin](http://localhost:8080)** - Console de administração (admin/admin)

---

## 🏛️ Estrutura do Projeto

```
SIPRI/
├── src/
│   ├── SIPRI.Domain/           # Entidades, VOs, Interfaces
│   ├── SIPRI.Application/      # Commands, Queries, Handlers, Validators
│   ├── SIPRI.Infrastructure/   # Repositórios, DbContext, Serviços
│   ├── SIPRI.Presentation/     # Controllers, Middleware
│   └── SIPRI.Host/             # Configuração e Startup
├── tests/
│   ├── SIPRI.Domain.Tests/
│   ├── SIPRI.Application.Tests/
│   ├── SIPRI.Infrastructure.Tests/
│   └── SIPRI.Presentation.Tests/
├── documentacao/
│   └── diagramas/              # Diagramas de arquitetura
├── docker-compose.yml
└── README.md
```

---

## 👨‍💻 Autor

**Diego da Rosa**

---

## 📄 Licença

MIT
