# HelpDesk API - CP4 Advanced Business Development with .NET

API RESTful para gerenciamento de chamados de suporte de TI, desenvolvida em .NET 8 com Controllers e organizada em camadas seguindo o mesmo padrao estrutural utilizado no projeto de referencia da disciplina.

## Estrutura da solution

```text
HelpDesk.sln
|-- HelpDesk.Api
|   |-- Application
|   |   |-- Dtos
|   |   |-- Interfaces
|   |   |-- Mappers
|   |   `-- UseCases
|   |-- Domain
|   |   |-- Entities
|   |   `-- Interfaces
|   |-- Doc
|   |   `-- Samples
|   |-- Infrastructure
|   |   |-- Data
|   |   |   |-- Migrations
|   |   |   `-- Repositories
|   |   |-- IoC
|   |   `-- Observability
|   |-- Presentation
|   |   `-- Controllers
|   `-- Program.cs
`-- HelpDesk.Tests
    |-- Application
    |-- Controllers
    |-- Domain
    |-- Fixtures
    |-- HealthChecks
    |-- Infrastructure
    `-- RateLimit
```

## Entidades

- **Usuario**: solicitante que abre chamados.
- **Tecnico**: profissional responsavel pelo atendimento.
- **Chamado**: registro principal do atendimento.
- **Comentario**: interacao registrada no chamado.

### Relacionamentos

- Usuario 1:N Chamado
- Tecnico 1:N Chamado
- Chamado 1:N Comentario

## Regras de negocio implementadas

- Somente usuario ativo pode abrir chamado.
- Tecnico informado deve existir e estar ativo.
- Todo novo chamado inicia com status `Aberto`.
- Prioridades aceitas: `Baixa`, `Media`, `Alta`, `Critica`.
- Fluxo de status: `Aberto -> Em Atendimento -> Resolvido -> Fechado`.
- Chamado fechado nao pode ser alterado.
- Ao fechar um chamado, `DataFechamento` e preenchida automaticamente.
- Chamado fechado nao pode receber novos comentarios.

## Requisitos do CP4 atendidos

### Repository Pattern, DTOs e Mappers

As interfaces de repository ficam em `Domain/Interfaces`, implementacoes em `Infrastructure/Data/Repositories`, DTOs e mapeamentos em `Application`.

### Paginacao

Endpoints de listagem recebem:

```http
GET /api/chamados?pageNumber=1&pageSize=10
```

`PageSize` e limitado a 100 na camada de Application e os repositories aplicam `Skip` e `Take`.

### Indices de banco

Os indices sao declarados diretamente nas entidades com o atributo `[Index]`, seguindo o exemplo apresentado em aula, e sao materializados no banco pela migration inicial. Entre eles:

- `UX_USUARIO_EMAIL` (unique)
- `UX_TECNICO_EMAIL` (unique)
- `IX_CHAMADO_STATUS`
- `IX_CHAMADO_PRIORIDADE`
- `IX_CHAMADO_STATUS_PRIORIDADE` (composto)
- `IX_CHAMADO_USUARIO`
- `IX_CHAMADO_TECNICO`
- `IX_CHAMADO_DATA_ABERTURA`
- `IX_COMENTARIO_CHAMADO`

### Response Compression

Configurada no `Program.cs` com Brotli e Gzip, inclusive para HTTPS.

### Rate Limiting

Fixed Window com a politica nomeada `politica_5_tentativas`, aplicada nos endpoints de listagem por `[EnableRateLimiting]` e particionada pelo endereco IP do cliente.

A politica e configurada diretamente no `Program.cs`, seguindo o exemplo utilizado em aula:

- `PermitLimit = 5`
- `Window = 20 segundos`
- `QueueLimit = 0`
- retorno `429 Too Many Requests` ao exceder o limite

Na sexta requisicao do mesmo IP dentro da mesma janela, a API retorna:

```http
429 Too Many Requests
```

### Swagger

Swagger/OpenAPI habilitado com `SwaggerOperation`, `SwaggerResponse`, `SwaggerParameter`, `SwaggerRequestExample` e `SwaggerResponseExample`. O projeto usa `Swashbuckle.AspNetCore.Filters` e providers em `Doc/Samples`, seguindo o padrao do exemplo do professor.

As listagens tambem documentam `204 No Content` quando nao ha registros.

Em desenvolvimento, execute a API e abra `/swagger`.

### Logging e Serilog

- `ILogger<T>` nos Controllers e UseCases.
- `Information`, `Warning` e `Error` nos fluxos principais.
- Serilog no console.
- Arquivo diario em `logs/api-AAAA-MM-DD.log`.
- Retencao de 7 arquivos.
- Correlation ID no header `X-Correlation-ID`.

### Health Checks

Endpoints de middleware:

```http
GET /health
GET /health/live
GET /health/db
```

Endpoints detalhados via controller:

```http
GET /api/health/live
GET /api/health/db
```

`live` valida a propria API e `db` valida o Oracle.

### OpenTelemetry e Application Insights

A API utiliza OpenTelemetry para traces e metricas e integra o Azure Monitor/Application Insights atraves de `UseAzureMonitor`.

Configure em `appsettings.json` ou por configuracao externa:

```json
"ApplicationInsights": {
  "ConnectionString": "SUA_CONNECTION_STRING"
}
```

Nao publique uma connection string real em repositorio publico.

Metricas customizadas:

- `helpdesk.api.requests`
- `helpdesk.api.errors`
- `helpdesk.api.response_time`
- `helpdesk.chamados.criados`
- `helpdesk.api.error_rate`
- `helpdesk.api.average_response_time`

Snapshot local:

```http
GET /metrics
```

## Banco de dados

O projeto utiliza Oracle com `Oracle.EntityFrameworkCore`, seguindo o projeto de referencia.

Configure:

```json
"ConnectionStrings": {
  "OracleDbConnection": "User Id=USUARIO;Password=SENHA;Data Source=HOST:1521/SERVICE_NAME"
}
```

A aplicacao tenta aplicar as migrations ao iniciar quando a connection string estiver preenchida.

Para trabalhar manualmente com migrations:

```bash
dotnet ef database update --project HelpDesk.Api
```

## Principais endpoints

| Metodo | Endpoint | Descricao |
|---|---|---|
| GET | `/api/usuarios?pageNumber=1&pageSize=10` | Lista usuarios |
| GET | `/api/usuarios/{id}` | Busca usuario |
| POST | `/api/usuarios` | Cadastra usuario |
| PUT | `/api/usuarios/{id}` | Atualiza usuario |
| DELETE | `/api/usuarios/{id}` | Exclui usuario |
| GET | `/api/tecnicos?pageNumber=1&pageSize=10` | Lista tecnicos |
| POST | `/api/tecnicos` | Cadastra tecnico |
| GET | `/api/chamados?pageNumber=1&pageSize=10` | Lista chamados |
| GET | `/api/chamados/{id}` | Busca chamado |
| GET | `/api/chamados/usuario/{id}` | Chamados do usuario |
| GET | `/api/chamados/tecnico/{id}` | Chamados do tecnico |
| POST | `/api/chamados` | Abre chamado |
| PUT | `/api/chamados/{id}` | Atualiza chamado/status |
| GET | `/api/comentarios/chamado/{id}` | Comentarios do chamado |
| POST | `/api/comentarios` | Adiciona comentario |

## Exemplo de requisicoes

### Usuario

```json
{
  "idUsuario": 1,
  "nome": "Joao Silva",
  "email": "joao@empresa.com",
  "departamento": "Financeiro",
  "ativo": true
}
```

### Tecnico

```json
{
  "idTecnico": 1,
  "nome": "Ana Tecnica",
  "email": "ana@empresa.com",
  "especialidade": "Sistemas",
  "ativo": true
}
```

### Chamado

```json
{
  "idChamado": 1,
  "idUsuario": 1,
  "idTecnico": 1,
  "titulo": "Erro de acesso ao ERP",
  "descricao": "Usuario nao consegue autenticar no sistema.",
  "prioridade": "Alta"
}
```

Para avancar o chamado para o proximo status, envie `PUT /api/chamados/{id}` com os dados do chamado e `status`, respeitando o fluxo definido.

## Executando o projeto

Pre-requisitos:

- .NET SDK 8
- Oracle acessivel para a execucao completa dos endpoints persistentes

Restaurar e compilar:

```bash
dotnet restore
dotnet build
```

Executar:

```bash
dotnet run --project HelpDesk.Api
```

## Testes

Todos os testes estao concentrados em `HelpDesk.Tests`, mas organizados por categoria.

Executar:

```bash
dotnet test
```

A suite cobre, entre outros cenarios:

- regras de criacao e status do chamado;
- bloqueio de usuario inativo;
- prioridade invalida;
- fechamento permitido somente apos resolucao;
- comentario em chamado fechado;
- paginacao no repository;
- ciclo HTTP basico da API;
- Health Check;
- Rate Limit por politica nomeada/IP e retorno 429.
- retorno 204 em listagem vazia.

Os testes de repository utilizam EF Core InMemory. Os testes funcionais de Controllers usam `WebApplicationFactory` e mocks dos UseCases, seguindo o padrao do projeto de referencia, para validar o ciclo HTTP sem depender do Oracle.

## Demonstracao sugerida

1. Abrir Swagger.
2. Cadastrar usuario e tecnico.
3. Abrir chamado.
4. Demonstrar paginacao.
5. Demonstrar uma transicao invalida de status.
6. Fazer requisicoes repetidas e mostrar `429 Too Many Requests`.
7. Mostrar `/health/live` e `/health/db`.
8. Executar `dotnet test`.
9. Mostrar logs no console/arquivo.
10. Mostrar requisicoes, traces e metricas no Application Insights.

## Seguranca de configuracao

Nao versionar credenciais reais de Oracle ou Application Insights. Utilize configuracao local, variaveis de ambiente ou User Secrets.
