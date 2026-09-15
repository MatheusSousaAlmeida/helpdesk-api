# Guia de commits sugeridos

O projeto esta entregue completo, mas esta divisao permite aplicar os arquivos por blocos no repositorio do grupo e manter um historico de commits coerente.

## Commit 01 - estrutura e dominio

Arquivos principais:
- Solution e projetos
- `Domain/Entities` (incluindo os atributos `[Index]` apresentados em aula)

Mensagem:

```text
feat: cria estrutura inicial e entidades do HelpDesk
```

## Commit 02 - DTOs e mappers

Arquivos:
- `Application/Dtos`
- `Application/Mappers`

Mensagem:

```text
feat: adiciona DTOs e mapeamentos das entidades
```

## Commit 03 - interfaces

Arquivos:
- `Domain/Interfaces`
- `Application/Interfaces`

Mensagem:

```text
feat: adiciona interfaces de repositories e use cases
```

## Commit 04 - persistencia e indices

Arquivos:
- `Infrastructure/Data/ApplicationContext.cs`
- `Infrastructure/Data/Migrations`

Mensagem:

```text
feat: configura contexto Oracle relacionamentos e indices
```

## Commit 05 - repositories e paginacao

Arquivos:
- `Infrastructure/Data/Repositories`

Mensagem:

```text
feat: implementa repositories com paginacao
```

## Commit 06 - use cases e regras de negocio

Arquivos:
- `Application/UseCases`
- `Infrastructure/IoC/Bootstrap.cs`

Mensagem:

```text
feat: implementa casos de uso e regras dos chamados
```

## Commit 07 - controllers e swagger

Arquivos:
- `Presentation/Controllers`, exceto Health se quiser separar
- `Doc/Samples` com exemplos de request/response do Swagger
- pacote `Swashbuckle.AspNetCore.Filters`

Mensagem:

```text
feat: adiciona controllers e documentacao swagger
```

## Commit 08 - logging e health checks

Arquivos/trechos:
- Serilog e Correlation ID no `Program.cs`
- `HealthController.cs`
- Health Checks no `Program.cs`

Mensagem:

```text
feat: adiciona logging estruturado e health checks
```

## Commit 09 - performance e protecao

Arquivos/trechos:
- Response Compression
- Rate Limiting com politica nomeada `politica_5_tentativas` e `[EnableRateLimiting]`
- particionamento da politica por IP
- configuracoes no `appsettings.json`

Mensagem:

```text
feat: adiciona compressao e rate limiting por IP
```

## Commit 10 - observabilidade

Arquivos:
- `Infrastructure/Observability/ApiMetrics.cs`
- configuracao OpenTelemetry/Application Insights no `Program.cs`

Mensagem:

```text
feat: integra metricas tracing e application insights
```

## Commit 11 - testes automatizados

Arquivos:
- `HelpDesk.Tests`

Mensagem:

```text
test: adiciona testes unitarios e funcionais da API
```

## Commit 12 - documentacao e CI

Arquivos:
- `README.md`
- `.github/workflows/build-and-test.yml`
- `GUIA_COMMITS.md`

Mensagem:

```text
docs: adiciona documentacao de execucao e entrega do CP4
```
