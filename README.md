# HelpDesk API

API RESTful desenvolvida em **ASP.NET Core .NET 8** para gerenciamento de chamados de suporte de TI.

O projeto foi desenvolvido com foco em arquitetura em camadas, otimização de performance, resiliência, testes automatizados e observabilidade.

A aplicação permite cadastrar usuários e técnicos, abrir e gerenciar chamados, atribuir técnicos responsáveis e registrar comentários associados aos atendimentos.

---

# 📋 Descrição do Projeto

O **HelpDesk API** tem como objetivo fornecer uma API para gerenciamento de chamados de suporte técnico.

O sistema trabalha com quatro entidades principais:

- **Usuário**: pessoa responsável pela abertura do chamado.
- **Técnico**: profissional responsável pelo atendimento.
- **Chamado**: solicitação de suporte registrada no sistema.
- **Comentário**: interação ou observação registrada durante o atendimento.

Um chamado possui informações como:

- Título
- Descrição
- Prioridade
- Status
- Usuário solicitante
- Técnico responsável
- Data de abertura
- Data de atualização
- Data de fechamento

O fluxo principal de status de um chamado é:

```text
Aberto
   ↓
Em Atendimento
   ↓
Resolvido
   ↓
Fechado
```

Algumas regras de negócio implementadas:

- Um chamado sempre é criado inicialmente com status `Aberto`.
- Apenas usuários ativos podem abrir chamados.
- Apenas técnicos ativos podem ser atribuídos a chamados.
- Um chamado não pode ser fechado diretamente sem ter sido resolvido.
- Chamados fechados não podem ser alterados.
- Comentários não podem ser adicionados a chamados fechados.
- A data de fechamento é preenchida automaticamente quando o chamado é finalizado.

---

# 🏗️ Arquitetura

O projeto utiliza uma organização em camadas inspirada nos conceitos de **Clean Architecture**, separando responsabilidades dentro da aplicação.

A Solution possui dois projetos:

```text
HelpDesk.sln
│
├── HelpDesk.Api
│
└── HelpDesk.Tests
```

A API está organizada da seguinte maneira:

```text
HelpDesk.Api/
│
├── Application/
│   ├── Dtos/
│   ├── Interfaces/
│   ├── Mappers/
│   └── UseCases/
│
├── Domain/
│   ├── Entities/
│   └── Interfaces/
│
├── Infrastructure/
│   ├── Data/
│   │   ├── Migrations/
│   │   └── Repositories/
│   ├── IoC/
│   └── Observability/
│
├── Presentation/
│   └── Controllers/
│
├── Doc/
│   └── Samples/
│
├── Program.cs
├── appsettings.json
└── HelpDesk.Api.csproj
```

## Domain

Responsável pelas entidades e pelos contratos dos repositories.

Principais entidades:

```text
Usuario
Tecnico
Chamado
Comentario
```

## Application

Responsável pelas regras e fluxos da aplicação.

Contém:

- DTOs
- Mappers
- Interfaces dos UseCases
- Implementação dos UseCases

Fluxo principal:

```text
Controller
    ↓
UseCase
    ↓
Repository
    ↓
Entity Framework
    ↓
Banco de Dados
```

## Infrastructure

Responsável pelo acesso a dados e recursos de infraestrutura.

Contém:

- `ApplicationContext`
- Repository Pattern
- Migrations
- IoC / Dependency Injection
- Métricas e observabilidade

## Presentation

Responsável pelos endpoints HTTP da API.

Contém os Controllers:

```text
UsuariosController
TecnicosController
ChamadosController
ComentariosController
HealthController
```

---

# 🧩 Componentes Utilizados

## .NET 8

A aplicação utiliza:

```text
.NET 8
ASP.NET Core Web API
Controllers
```

---

## Entity Framework Core

O **Entity Framework Core** é utilizado como ORM para persistência e consulta dos dados.

Banco utilizado:

```text
Oracle
```

O acesso ao banco é realizado através do:

```text
ApplicationContext
```

---

## Repository Pattern

Cada entidade possui uma interface de repository na camada Domain e sua implementação na Infrastructure.

Exemplo:

```text
IChamadoRepository
        ↓
ChamadoRepository
```

O objetivo é separar a lógica de negócio da implementação de acesso ao banco.

---

# 📦 DTOs e Mapeamentos

Os dados recebidos pela API são tratados através de DTOs.

Exemplos:

```text
UsuarioRequestDto
TecnicoRequestDto
ChamadoRequestDto
ComentarioRequestDto
```

Os Mappers realizam a conversão entre DTOs e entidades.

Exemplo:

```text
ChamadoRequestDto
        ↓
ChamadoMapper
        ↓
Chamado
```

---

# 📄 Paginação

Os endpoints de listagem possuem paginação utilizando:

```text
pageNumber
pageSize
```

Exemplo:

```http
GET /api/chamados?pageNumber=1&pageSize=10
```

A paginação é aplicada no Repository através de:

```csharp
.Skip((pageNumber - 1) * pageSize)
.Take(pageSize)
```

O tamanho máximo de página é controlado pela aplicação para evitar consultas excessivamente grandes.

---

# ⚡ Índices de Banco de Dados

Foram configurados índices nas entidades para melhorar o desempenho das principais consultas.

Exemplos:

- E-mail do usuário
- E-mail do técnico
- Status do chamado
- Prioridade do chamado
- Status + prioridade
- Relacionamentos entre chamados, usuários e técnicos

Exemplo de índice composto:

```text
Status + Prioridade
```

Esse índice auxilia consultas como:

```text
Chamados com status "Aberto"
e prioridade "Crítica"
```

---

# 🗜️ Response Compression

A API utiliza compressão de resposta para reduzir o tamanho dos dados enviados ao cliente.

São utilizados os algoritmos:

```text
Brotli
Gzip
```

A compressão também está habilitada para conexões HTTPS.

---

# 🚦 Rate Limiting

A aplicação possui proteção contra excesso de requisições utilizando o middleware nativo de Rate Limiting do ASP.NET Core.

A política utilizada é:

```text
politica_5_tentativas
```

Configuração:

```text
5 requisições
por cliente/IP
a cada 20 segundos
```

Ao ultrapassar o limite, a API retorna:

```http
429 Too Many Requests
```

A política é aplicada aos endpoints através de:

```csharp
[EnableRateLimiting("politica_5_tentativas")]
```

---

# 📖 Swagger / OpenAPI

A documentação dos endpoints é disponibilizada através do Swagger.

A aplicação utiliza:

```text
Swashbuckle.AspNetCore
Swashbuckle.AspNetCore.Annotations
Swashbuckle.AspNetCore.Filters
```

Os Controllers possuem:

- `SwaggerOperation`
- `SwaggerResponse`
- `SwaggerParameter`
- Exemplos de Request
- Exemplos de Response

Ao executar o projeto em ambiente de desenvolvimento, acesse:

```text
/swagger
```

---

# 📝 Logging

O projeto utiliza o sistema nativo:

```text
ILogger<T>
```

em Controllers e UseCases.

São utilizados principalmente os níveis:

```text
Information
Warning
Error
```

Exemplo:

```csharp
_logger.LogInformation(
    "Obtendo chamado com id {ChamadoId}",
    id);
```

Quando um chamado não é encontrado:

```csharp
_logger.LogWarning(
    "Chamado com id {ChamadoId} não encontrado",
    id);
```

Em situações de erro:

```csharp
_logger.LogError(
    ex,
    "Erro ao obter chamado com id {ChamadoId}",
    id);
```

---

# 📂 Serilog

Além do `ILogger<T>`, o projeto utiliza **Serilog**.

Os logs são enviados para:

```text
Console
+
Arquivo
```

Os arquivos ficam no diretório:

```text
logs/
```

Exemplo:

```text
logs/api-20260915.log
```

Os arquivos possuem rotação diária e retenção configurada.

---

# ❤️ Health Checks

A API utiliza Health Checks para monitorar sua própria disponibilidade e a conexão com o banco de dados.

## Liveness

Verifica se a aplicação está em execução.

```http
GET /api/health/live
```

## Banco de dados

Verifica a disponibilidade do Oracle.

```http
GET /api/health/db
```

Quando saudável:

```http
200 OK
```

Quando uma dependência crítica não está disponível:

```http
503 Service Unavailable
```

---

# 📊 Observabilidade

A aplicação utiliza **OpenTelemetry** para coleta de:

- Traces
- Métricas
- Requisições ASP.NET Core
- Chamadas HTTP
- Métricas personalizadas da API

Exemplos de métricas:

```text
helpdesk.api.requests
helpdesk.api.errors
helpdesk.api.response_time
helpdesk.chamados.criados
```

---

# ☁️ Application Insights

A telemetria coletada através do OpenTelemetry pode ser enviada ao **Azure Application Insights**.

Pacotes utilizados:

```text
Microsoft.ApplicationInsights.AspNetCore
Azure.Monitor.OpenTelemetry.AspNetCore
```

A integração é realizada através do:

```csharp
UseAzureMonitor()
```

## Criando o recurso

No Portal do Azure:

```text
Azure
→ Application Insights
→ Create
```

Após criar o recurso, copie a:

```text
Connection String
```



## Configuração

O projeto utiliza a configuração:

```json
{
  "ApplicationInsights": {
    "ConnectionString": ""
  }
}
```
Adicione:

```json
{
  "ApplicationInsights": {
    "ConnectionString": "SUA_CONNECTION_STRING"
  }
}
```


## Validando no Azure

Após iniciar a aplicação e realizar algumas requisições, é possível acompanhar as informações em:

```text
Application Insights
→ Live Metrics
```

Também é possível consultar os dados através da área:

```text
Application Insights
→ Logs
```

Exemplo para visualizar requisições:

```kusto
requests
| where timestamp > ago(30m)
| order by timestamp desc
```

Exemplo para visualizar logs:

```kusto
traces
| where timestamp > ago(30m)
| order by timestamp desc
```

Exemplo para visualizar métricas personalizadas:

```kusto
customMetrics
| where timestamp > ago(30m)
| where name startswith "helpdesk."
| order by timestamp desc
```

---

# 🗄️ Configuração do Banco de Dados

A aplicação utiliza Oracle através do Entity Framework Core.

A Connection String deve ser configurada em:

```text
appsettings.Development.json
```

Exemplo:

```json
{
  "ConnectionStrings": {
    "OracleDbConnection": "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521))) (CONNECT_DATA=(SERVER=DEDICATED)(SID=ORCL)));User Id=SEU_ID_LOGIN;Password=SUA_SENHA;"
  }
}
```



---

# 🛠️ Preparação do Ambiente

É necessário possuir:

```text
.NET SDK 8
Visual Studio 2022
Conta Oracle acessível
```

Para confirmar a instalação do SDK:

```bash
dotnet --list-sdks
```



---

# ▶️ Como Rodar o Projeto

Clone o repositório:

```bash
git clone https://github.com/MatheusSousaAlmeida/helpdesk-api
```

Entre na pasta:

```bash
cd helpdesk-api
```

Restaure os pacotes:

```bash
dotnet restore
```

Compile:

```bash
dotnet build
```

Execute a API:

```bash
dotnet run --project HelpDesk.Api
```

Ou abra:

```text
HelpDesk.sln
```

no Visual Studio 2022 e aperte F5.

# 🧪 Testes Automatizados

A solução possui dois projetos dedicados a testes, separando testes unitários dos testes funcionais e de integração.

## HelpDesk.Unit

Responsável pelos testes de unidade das regras de negócio e entidades, executados de forma isolada com mocks quando necessário.

Inclui:

- Testes dos UseCases
- Testes das regras de negócio
- Testes das entidades
- Validação de criação e alteração de chamados
- Validação de prioridade e fluxo de status
- Validação de paginação na camada de aplicação

Para executar somente os testes unitários:

```bash
dotnet test HelpDesk.Unit/HelpDesk.Unit.csproj
```

## HelpDesk.Integration

Responsável pelos testes funcionais e de integração, validando o ciclo de requisição HTTP da API através de `WebApplicationFactory`.

Inclui:

- Testes dos Controllers
- Testes de integração dos endpoints
- Testes de Repository com Entity Framework Core InMemory
- Testes de Health Check
- Testes de Rate Limit
- Testes de paginação via endpoint
- Validação de códigos HTTP como 201, 204, 404 e 429

Para executar somente os testes funcionais e de integração:

```bash
dotnet test HelpDesk.Integration/HelpDesk.Integration.csproj
```

Para executar os dois projetos de testes através da Solution:

```bash
dotnet test HelpDesk.sln
```

Ou:

```text
Execute pelo Gerenciador de Testes do Visual Studio.
```

O resultado esperado é que todos os testes sejam apresentados como:

```text
Passed
```

---

# 🌐 Endpoints

## Usuários

### Listar usuários

```http
GET /api/usuarios?pageNumber=1&pageSize=10
```

### Buscar usuário por ID

```http
GET /api/usuarios/1
```

### Criar usuário

```http
POST /api/usuarios
```

Exemplo:

```json
{
  "idUsuario": 1,
  "nome": "João da Silva",
  "email": "joao.silva@empresa.com",
  "departamento": "Financeiro",
  "ativo": true
}
```

### Atualizar usuário

```http
PUT /api/usuarios/1
```

### Excluir usuário

```http
DELETE /api/usuarios/1
```

---

# 👨‍💻 Técnicos

### Listar técnicos

```http
GET /api/tecnicos?pageNumber=1&pageSize=10
```

### Buscar técnico

```http
GET /api/tecnicos/1
```

### Criar técnico

```http
POST /api/tecnicos
```

Exemplo:

```json
{
  "idTecnico": 1,
  "nome": "Maria Souza",
  "email": "maria.souza@empresa.com",
  "especialidade": "Infraestrutura",
  "ativo": true
}
```

### Atualizar técnico

```http
PUT /api/tecnicos/1
```

### Excluir técnico

```http
DELETE /api/tecnicos/1
```

---

# 🎫 Chamados

### Listar chamados

```http
GET /api/chamados?pageNumber=1&pageSize=10
```

Exemplo:

```http
GET /api/chamados?pageNumber=2&pageSize=5
```

### Buscar chamado

```http
GET /api/chamados/1
```

### Listar chamados por usuário

```http
GET /api/chamados/usuario/{idUsuario}?pageNumber=1&pageSize=10
```

Exemplo:

```http
GET /api/chamados/usuario/1?pageNumber=1&pageSize=10
```

Retorna os chamados associados ao usuário informado, utilizando paginação.

### Listar chamados por técnico

```http
GET /api/chamados/tecnico/{idTecnico}?pageNumber=1&pageSize=10
```

Exemplo:

```http
GET /api/chamados/tecnico/1?pageNumber=1&pageSize=10
```

Retorna os chamados atribuídos ao técnico informado, utilizando paginação.

### Criar chamado

```http
POST /api/chamados
```

Exemplo:

```json
{
  "idChamado": 1,
  "idUsuario": 1,
  "idTecnico": null,
  "titulo": "Computador sem acesso à internet",
  "descricao": "Usuário não consegue acessar a rede corporativa.",
  "prioridade": "Alta",
  "status": "Aberto"
}
```

O status inicial é controlado pela aplicação.

### Atualizar chamado

```http
PUT /api/chamados/1
```

### Excluir chamado

```http
DELETE /api/chamados/1
```

---

# 💬 Comentários

### Listar comentários

```http
GET /api/comentarios?pageNumber=1&pageSize=10
```

### Buscar comentário

```http
GET /api/comentarios/1
```

### Listar comentários de um chamado

```http
GET /api/comentarios/chamado/{idChamado}?pageNumber=1&pageSize=10
```

Exemplo:

```http
GET /api/comentarios/chamado/1?pageNumber=1&pageSize=10
```

Retorna os comentários associados ao chamado informado, utilizando paginação.

### Criar comentário

```http
POST /api/comentarios
```

Exemplo:

```json
{
  "idComentario": 1,
  "idChamado": 1,
  "autor": "Maria Souza",
  "texto": "Foi identificado um problema na configuração de rede."
}
```

### Atualizar comentário

```http
PUT /api/comentarios/1
```

### Excluir comentário

```http
DELETE /api/comentarios/1
```

---

# ❤️ Health Check

### Verificar API

```http
GET /api/health/live
```

### Verificar banco

```http
GET /api/health/db
```

---

# 🚦 Exemplo de Rate Limit

A política permite:

```text
5 requisições em 20 segundos
```

Quando o limite é ultrapassado:

```http
HTTP/1.1 429 Too Many Requests
```

Exemplo de demonstração:

```text
Requisição 1 → 200
Requisição 2 → 200
Requisição 3 → 200
Requisição 4 → 200
Requisição 5 → 200
Requisição 6 → 429
```

---

# 📌 Principais Tecnologias

```text
.NET 8
ASP.NET Core Web API
Entity Framework Core
Oracle
Repository Pattern
DTOs
Mappers
Swagger / OpenAPI
Swagger Annotations
Swagger Filters
Response Compression
Rate Limiting
Serilog
ILogger
Health Checks
OpenTelemetry
Azure Monitor
Application Insights
xUnit
Moq
EF Core InMemory
WebApplicationFactory
```

---

## 👥 Integrantes do Grupo

- **Enzo Monteiro Maciel** - RM: 563734
- **Matheus de Almeida Sousa** - RM: 563557
- **Paulo Estalise** - RM: 563811
- **Gabriel Bebé Silva** - RM: 562012
- **Emanuel Italo** - RM: 561337

---
