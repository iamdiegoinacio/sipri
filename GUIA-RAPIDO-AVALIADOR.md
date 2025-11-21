# 🚀 Guia Rápido para Avaliadores - SIPRI

> **⚡ Comece a testar a aplicação em menos de 2 minutos!**

---

## 📦 Passo 1: Baixar e Levantar o Ambiente

```bash
# Clone o repositório
git clone https://github.com/iamdiegoinacio/sipri.git
cd sipri

# Inicie todos os serviços com Docker Compose
docker-compose up --build -d

# Aguarde ~30 segundos para inicialização completa
```

**✅ Serviços disponíveis:**

- 🌐 **API SIPRI:** http://localhost:5058
- 🔐 **Keycloak:** http://localhost:8080
- 💾 **SQL Server:** localhost:1433

---

## 🎯 Escolha sua Forma de Teste

### ✅ OPÇÃO 1: Testar via Swagger (Recomendado - Mais Rápido)

#### 1️⃣ Acesse o Swagger UI

Abra no navegador: **http://localhost:5058/swagger/index.html**

#### 2️⃣ Autentique-se

1. Clique no botão **"Authorize"** 🔓 (canto superior direito)
2. Na janela que abrir, clique novamente em **"Authorize"**
3. Você será redirecionado para o Keycloak
4. **Faça login:**
   - **Username:** `user`
   - **Password:** `user`
5. Após login, você retorna ao Swagger autenticado ✅

#### 3️⃣ Teste os Endpoints

Agora você pode expandir qualquer endpoint e clicar em **"Try it out"** para testá-lo!

**Exemplos de testes:**

- **POST** `/api/simulacoes/simular` - Simular investimento
- **GET** `/api/perfil/{clienteId}` - Calcular perfil de risco
- **GET** `/api/perfil/produtos-recomendados/{perfil}` - Produtos recomendados
- **GET** `/api/investimentos/{clienteId}` - Carteira de investimentos
- **GET** `/api/telemetria` - Métricas da API

#### 4️⃣ Consulte os Casos de Uso

Para exemplos detalhados de cada endpoint com payloads de entrada/saída:

👉 **[Documentação de Casos de Uso e Testes](documentacao/casos-de-uso-e-testes/casos-de-uso-e-testes.md)**

---

### ✅ OPÇÃO 2: Testar via Postman (Mais Completo)

#### 1️⃣ Baixe e Importe a Coleção

1. Baixe o arquivo: **[SIPRI.postman_collection.json](SIPRI.postman_collection.json)**
2. Abra o Postman
3. Clique em **"Import"**
4. Selecione o arquivo baixado
5. A coleção será importada com todas as variáveis pré-configuradas ✅

#### 2️⃣ Autentique-se (Escolha um dos clientes)

##### 🌐 Opção A: Cliente Web Público (cli-web-sipri)

**Mais simples - Recomendado para testes rápidos**

1. Vá para a pasta **"🔐 Autenticação"**
2. Execute a requisição **"1️⃣ Obter Token - Cliente Web (PKCE)"**
3. O token será salvo automaticamente ✅

**Credenciais:**

- Username: `user`
- Password: `user`

##### 🔧 Opção B: Cliente de Serviço (cli-sir-sipri)

**Demonstra autenticação service-to-service**

> **✨ O client secret já está pré-configurado na coleção!**

1. Vá para a pasta **"🔐 Autenticação"**
2. Execute a requisição **"2️⃣ Obter Token - Cliente de Serviço (Client Credentials)"**
3. O token será salvo automaticamente ✅

**Client Secret já configurado:** `Z3T3Jz3QWZ1Hdb0TpyW8JTKXytnmAylR`

#### 3️⃣ Teste os Endpoints

Agora você pode executar qualquer requisição das pastas:

- 💰 **Simulações** - Simular investimentos
- 👤 **Perfil de Risco** - Calcular perfil e obter recomendações
- 📊 **Investimentos** - Consultar carteira
- 📈 **Telemetria** - Métricas da API

**Todos os tokens são automaticamente incluídos nas requisições!**

---

## 🔐 Demonstração de Autenticação OAuth2

A aplicação suporta **dois tipos de clientes OAuth2**:

| Cliente           | Tipo         | Grant Type                           | Uso                | Status       |
| ----------------- | ------------ | ------------------------------------ | ------------------ | ------------ |
| **cli-web-sipri** | Público      | Authorization Code + PKCE / Password | Aplicações Web/SPA | ✅ Funcional |
| **cli-sir-sipri** | Confidencial | Client Credentials                   | Serviços Backend   | ✅ Funcional |

**Ambos os fluxos estão funcionais e podem ser testados via Swagger ou Postman!**

---

## 📖 Casos de Uso Pré-Configurados

### 💰 Simulação de Investimento

```json
POST /api/simulacoes/simular
{
  "clienteId": "123e4567-e89b-12d3-a456-426614174000",
  "produtoId": 1,
  "valorInicial": 10000.00,
  "prazoMeses": 12
}
```

### 👤 Cálculo de Perfil de Risco

```
GET /api/perfil/123e4567-e89b-12d3-a456-426614174000
```

### 🎯 Produtos Recomendados

```
GET /api/perfil/produtos-recomendados/Moderado
```

**Perfis válidos:** `Conservador`, `Moderado`, `Arrojado`

### 📊 Carteira de Investimentos

```
GET /api/investimentos/123e4567-e89b-12d3-a456-426614174000
```

### 📈 Métricas da API

```
GET /api/telemetria
```

---

## 🛠️ Gerenciar Usuários (Opcional)

Se precisar criar mais usuários ou ajustar permissões:

1. Acesse o **Painel Admin do Keycloak:** http://localhost:8080/admin
2. **Credenciais de Admin:**
   - **Username:** `admin`
   - **Password:** `admin`
3. Navegue para: **Realm: sipri-realm** → **Users** → **Add User**

---

## 📚 Documentação Completa

Para mais detalhes sobre arquitetura, padrões e decisões técnicas:

- 📘 **[README Principal](README.md)** - Visão geral completa
- 🏗️ **[Arquitetura do Sistema](documentacao/arquitetura/arquitetura.md)** - Detalhes arquiteturais
- 🔐 **[Guia de Autenticação](documentacao/autenticacao/autenticacao.md)** - OAuth2, JWT e Keycloak
- 📖 **[Casos de Uso e Testes](documentacao/casos-de-uso-e-testes/casos-de-uso-e-testes.md)** - Exemplos detalhados

---

## ✅ Checklist de Testes

- [ ] Ambiente Docker levantado com sucesso
- [ ] Swagger acessível em http://localhost:5058/swagger
- [ ] Autenticação via Swagger funcionando
- [ ] Simulação de investimento testada
- [ ] Cálculo de perfil de risco testado
- [ ] Produtos recomendados consultados
- [ ] Telemetria acessada
- [ ] (Opcional) Coleção Postman importada
- [ ] (Opcional) Autenticação com ambos os clientes testada

---

## 🆘 Troubleshooting

### Problema: Serviços não sobem

```bash
# Verificar logs
docker-compose logs

# Recriar containers
docker-compose down -v
docker-compose up --build -d
```

### Problema: Erro de autenticação

- Verifique se o Keycloak está rodando: http://localhost:8080
- Aguarde ~30 segundos após o `docker-compose up` para inicialização completa
- Limpe o cache do navegador e tente novamente

### Problema: Banco de dados não conecta

```bash
# Verificar status do SQL Server
docker-compose ps

# Verificar logs do SQL Server
docker-compose logs sipri-sql
```

---

<div align="center">

**⭐ Boa avaliação! ⭐**

Se tiver dúvidas, consulte a [documentação completa](README.md)

</div>
