# SIPRI - Sistema de Investimentos e Perfil de Risco

API REST para simulação de investimentos, cálculo de perfil de risco e recomendação de produtos financeiros.

## 🚀 Tecnologias

- .NET 8.0
- Clean Architecture (CQRS)
- MediatR
- FluentValidation
- Entity Framework Core
- SQL Server
- Keycloak (Autenticação)
- Docker

## 📋 Pré-requisitos

- .NET 8.0 SDK
- Docker e Docker Compose
- SQL Server (ou via Docker)

## 🔧 Configuração e Execução

### Com Docker Compose

```bash
docker-compose up --build
```

A API estará disponível em: `http://localhost:5058`

### Localmente

```bash
cd src/SIPRI.Host
dotnet run
```

## 🔐 Autenticação

Todos os endpoints requerem autenticação via Bearer Token (JWT) do Keycloak.

**Header necessário:**

```
Authorization: Bearer {seu_token_jwt}
```

---

## 📚 Endpoints e Casos de Uso

### 1. Simular Investimento

Realiza uma simulação de investimento com cálculo de rentabilidade.

**Endpoint:** `POST /simular-investimento`

**Entrada:**

```json
{
  "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "tipoProduto": "CDB",
  "valor": 10000.0,
  "prazoMeses": 12
}
```

**Saída Esperada (200 OK):**

```json
{
  "dataSimulacao": "2025-11-21T19:30:00Z",
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

**Validações:**

- `clienteId`: Obrigatório (GUID válido)
- `tipoProduto`: Obrigatório, máximo 50 caracteres
- `valor`: Deve ser > 0 e ≤ 1.000.000.000
- `prazoMeses`: Deve ser > 0 e ≤ 360

**Erros Possíveis:**

**400 Bad Request** (Validação):

```json
{
  "type": "https://httpstatuses.com/400",
  "title": "Erro de Validação",
  "status": 400,
  "detail": "A requisição falhou na validação.",
  "instance": "/simular-investimento",
  "traceId": "00-abc123...",
  "errors": {
    "RequestData.Valor": ["O valor do investimento deve ser maior que zero."]
  }
}
```

**404 Not Found** (Produto não encontrado):

```json
{
  "type": "https://httpstatuses.com/404",
  "title": "Recurso Não Encontrado",
  "status": 404,
  "detail": "Entidade \"ProdutoInvestimento\" (FundoImobiliario) não foi encontrada.",
  "instance": "/simular-investimento",
  "traceId": "00-xyz789..."
}
```

---

### 2. Listar Histórico de Simulações

Obtém o histórico de simulações realizadas por um cliente.

**Endpoint:** `GET /simulacoes?clienteId={guid}`

**Entrada:**

```
Query Parameter: clienteId=3fa85f64-5717-4562-b3fc-2c963f66afa6
```

**Saída Esperada (200 OK):**

```json
[
  {
    "id": "a1b2c3d4-5678-90ab-cdef-1234567890ab",
    "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "produto": "CDB Prefixado",
    "valorInvestido": 10000.0,
    "valorFinal": 11300.0,
    "prazoMeses": 12,
    "dataSimulacao": "2025-11-21T19:30:00Z"
  },
  {
    "id": "b2c3d4e5-6789-01bc-def1-234567890abc",
    "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "produto": "Fundo de Renda Fixa",
    "valorInvestido": 5000.0,
    "valorFinal": 5450.0,
    "prazoMeses": 6,
    "dataSimulacao": "2025-11-20T14:15:00Z"
  }
]
```

---

### 3. Obter Dados Agregados de Simulações

Retorna estatísticas agregadas das simulações por produto e dia.

**Endpoint:** `GET /simulacoes/por-produto-dia`

**Entrada:** Nenhuma (sem parâmetros)

**Saída Esperada (200 OK):**

```json
[
  {
    "produto": "CDB Prefixado",
    "data": "2025-11-21",
    "quantidadeSimulacoes": 15,
    "mediaValorFinal": 12500.5
  },
  {
    "produto": "Fundo de Renda Fixa",
    "data": "2025-11-21",
    "quantidadeSimulacoes": 8,
    "mediaValorFinal": 8750.25
  },
  {
    "produto": "CDB Prefixado",
    "data": "2025-11-20",
    "quantidadeSimulacoes": 12,
    "mediaValorFinal": 11200.0
  }
]
```

---

### 4. Calcular Perfil de Risco

Calcula o perfil de risco de um cliente baseado em seu histórico de investimentos.

**Endpoint:** `GET /perfil-risco/{clienteId}`

**Entrada:**

```
Path Parameter: clienteId=3fa85f64-5717-4562-b3fc-2c963f66afa6
```

**Saída Esperada (200 OK):**

```json
{
  "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "perfil": "Moderado",
  "pontuacao": 65.5,
  "descricao": "Investidor com tolerância moderada a riscos, busca equilíbrio entre segurança e rentabilidade."
}
```

**Perfis Possíveis:**

- `Baixo` (pontuação < 40): Baixa tolerância a riscos
- `Moderado` (pontuação 40-70): Tolerância média a riscos
- `Alto` (pontuação > 70): Alta tolerância a riscos

---

### 5. Obter Produtos Recomendados

Lista produtos de investimento adequados para um perfil de risco específico.

**Endpoint:** `GET /produtos-recomendados/{perfil}`

**Entrada:**

```
Path Parameter: perfil=Moderado
```

**Valores Aceitos:** `Conservador`, `Moderado`, `Arrojado`

**Saída Esperada (200 OK):**

```json
[
  {
    "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "nome": "CDB Prefixado",
    "tipo": "CDB",
    "rentabilidade": 0.13,
    "risco": "Baixo"
  },
  {
    "id": "8d0f7780-8536-51ef-c15c-3d184g01bf8",
    "nome": "Fundo Multimercado",
    "tipo": "Fundo",
    "rentabilidade": 0.18,
    "risco": "Médio"
  }
]
```

---

### 6. Obter Histórico de Investimentos

Retorna a carteira de investimentos de um cliente.

**Endpoint:** `GET /investimentos/{clienteId}`

**Entrada:**

```
Path Parameter: clienteId=3fa85f64-5717-4562-b3fc-2c963f66afa6
```

**Saída Esperada (200 OK):**

```json
[
  {
    "id": "c3d4e5f6-7890-12cd-ef23-4567890abcde",
    "tipo": "CDB",
    "valor": 10000.0,
    "rentabilidade": 0.13,
    "data": "2025-01-15"
  },
  {
    "id": "d4e5f6g7-8901-23de-f234-567890abcdef",
    "tipo": "Fundo",
    "valor": 5000.0,
    "rentabilidade": 0.18,
    "data": "2025-03-20"
  }
]
```

---

### 7. Obter Telemetria da API

Retorna métricas de uso e performance da API.

**Endpoint:** `GET /telemetria`

**Entrada:** Nenhuma

**Saída Esperada (200 OK):**

```json
{
  "totalSimulacoes": 1523,
  "totalClientes": 342,
  "tempoMedioResposta": 125.5,
  "ultimaAtualizacao": "2025-11-21T19:35:00Z"
}
```

---

## 🧪 Testando a API

### Usando cURL

**1. Simular Investimento:**

```bash
curl -X POST http://localhost:5000/simular-investimento \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {seu_token}" \
  -d '{
    "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "tipoProduto": "CDB",
    "valor": 10000.00,
    "prazoMeses": 12
  }'
```

**2. Obter Perfil de Risco:**

```bash
curl -X GET "http://localhost:5000/perfil-risco/3fa85f64-5717-4562-b3fc-2c963f66afa6" \
  -H "Authorization: Bearer {seu_token}"
```

**3. Listar Simulações:**

```bash
curl -X GET "http://localhost:5000/simulacoes?clienteId=3fa85f64-5717-4562-b3fc-2c963f66afa6" \
  -H "Authorization: Bearer {seu_token}"
```

### Usando Swagger

Acesse: `http://localhost:5000/swagger`

---

## 📊 Códigos de Status HTTP

| Código | Significado           | Quando Ocorre                                    |
| ------ | --------------------- | ------------------------------------------------ |
| 200    | OK                    | Requisição bem-sucedida                          |
| 400    | Bad Request           | Dados de entrada inválidos (validação falhou)    |
| 401    | Unauthorized          | Token ausente ou inválido                        |
| 403    | Forbidden             | Token válido mas sem permissão                   |
| 404    | Not Found             | Recurso não encontrado (ex: produto inexistente) |
| 409    | Conflict              | Conflito de recurso                              |
| 500    | Internal Server Error | Erro inesperado no servidor                      |
| 503    | Service Unavailable   | Serviço de infraestrutura indisponível           |

---

## 🔍 Formato de Erro Padrão (RFC 7807)

Todos os erros seguem o padrão **Problem Details** (RFC 7807):

```json
{
  "type": "https://httpstatuses.com/{statusCode}",
  "title": "Título do Erro",
  "status": 400,
  "detail": "Descrição detalhada do erro",
  "instance": "/endpoint-que-falhou",
  "traceId": "00-identificador-unico-trace",
  "errors": {
    "campo": ["mensagem de erro"]
  }
}
```

---

## 📝 Notas Importantes

1. **Autenticação:** Todos os endpoints requerem autenticação via Keycloak
2. **GUIDs:** Use GUIDs válidos para `clienteId` (formato: `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`)
3. **Produtos Disponíveis:** CDB, Fundo, LCI, LCA, Tesouro Direto
4. **Perfis de Risco:** Conservador, Moderado, Arrojado
5. **Validação:** A API valida automaticamente todos os inputs via FluentValidation

---

## 🐛 Troubleshooting

### Erro 401 (Unauthorized)

- Verifique se o token JWT está presente no header `Authorization`
- Confirme que o token não expirou
- Valide o formato: `Bearer {token}`

### Erro 404 (Produto não encontrado)

- Verifique se o `tipoProduto` existe no banco de dados
- Produtos disponíveis: CDB, Fundo, LCI, LCA, Tesouro Direto

### Erro 400 (Validação)

- Revise os dados de entrada conforme as regras de validação
- Consulte o campo `errors` na resposta para detalhes específicos

---

## 📞 Suporte

Para dúvidas ou problemas, consulte a documentação técnica ou entre em contato comigo.
