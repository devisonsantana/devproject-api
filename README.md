# DevProject API

[![CI](https://github.com/devisonsantana/devproject-api/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/devisonsantana/devproject-api/actions/workflows/dotnet.yml?query=branch%3Amain)

> **English** | [Português](#portugues)

<a id="english"></a>

## English

### Project presentation

DevProject API is an ASP.NET Core Web API starter for organizing projects,
features, and tasks. It also contains the first building blocks for user
registration and authentication-related domain data. The solution uses a
layered Clean Architecture style: HTTP controllers call Application services,
Application maps requests and responses and defines contracts, and
Infrastructure provides Entity Framework Core persistence backed by
PostgreSQL.

The current API is an initial implementation, not a complete project
management or authentication product. The implemented HTTP operations,
available filters, persistence behavior, and outstanding gaps are described
below.

### Table of contents

- [Project presentation](#project-presentation)
- [Capabilities](#capabilities)
- [Architecture and projects](#architecture-and-projects)
- [Domain model](#domain-model)
- [HTTP API](#http-api)
- [Filtered searches](#filtered-searches)
- [Persistence and migrations](#persistence-and-migrations)
- [Run locally](#run-locally)
- [Swagger UI](#swagger-ui)
- [Tests and CI](#tests-and-ci)
- [Known issues, inconsistencies, and missing features](#known-issues-inconsistencies-and-missing-features)
- [Apresentação em português](#portugues)
- [Problemas e funcionalidades pendentes](#problemas-e-funcionalidades-pendentes)

### Capabilities

- Create, retrieve, update, and delete projects.
- Create features under projects and retrieve or search features.
- Create tasks under features and retrieve or search tasks.
- Register users; passwords are hashed with BCrypt before persistence.
- Search projects, features, and tasks using optional query-string filters.
- Store projects, features, tasks, and users in PostgreSQL through EF Core.
- Explore the API contract with the Swagger UI in Development.
- Run domain, Application, Infrastructure/PostgreSQL, and API test projects.

### Architecture and projects

| Project | Responsibility |
|---|---|
| [`DevProject.Domain`](./src/DevProject.Domain) | Entities, status/priority enums, and generic repository/Unit of Work contracts. |
| [`DevProject.Application`](./src/DevProject.Application) | DTOs, search-query records, services, mapper contracts, mapping, and dependency registration. |
| [`DevProject.Infrastructure`](./src/DevProject.Infrastructure) | EF Core `AppDbContext`, entity mappings, PostgreSQL repositories, Unit of Work, and migrations. |
| [`DevProject.Api`](./src/DevProject.Api) | ASP.NET Core controllers, API composition, JSON enum configuration, OpenAPI, and Swagger UI. |
| [`DevProject.Domain.Tests`](./tests/DevProject.Domain.Tests) | Unit tests for domain entities and their behaviors. |
| [`DevProject.Application.Tests`](./tests/DevProject.Application.Tests) | Tests for DTOs, mappers, and Application services. |
| [`DevProject.Infrastructure.Tests`](./tests/DevProject.Infrastructure.Tests) | PostgreSQL integration tests for persistence and repositories. |
| [`DevProject.Api.Tests`](./tests/DevProject.Api.Tests) | HTTP integration tests for controllers, responses, and Swagger. |

The solution is [`DevProject.slnx`](./DevProject.slnx). The primary dependency
direction is API → Application → Domain, with Infrastructure depending on
Application and Domain to implement persistence contracts. Controllers return
Application DTOs rather than Domain entities.

### Domain model

- **Project** contains a name, optional description, status, and features.
- **Feature** belongs to a project, has a name, optional description, status,
  order, and tasks.
- **ProjectTask** belongs to a feature and stores a title, optional
  description, status, priority, optional due date, completion time, and
  order. Marking a task `Done` sets `CompletedAt`; changing it to another status
  clears that timestamp.
- **User** stores a profile, normalized email, password hash, and active state.
- **RefreshToken** exists as a Domain entity with hash, expiry and revocation
  properties, but is not currently included in EF persistence or the API.
- The base `Entity` assigns a GUID and UTC creation timestamp and tracks
  `UpdatedAt`.

Status and priority values are:

| Enum | Values |
|---|---|
| `ProjectStatus` | `Planned`, `Active`, `OnHold`, `Completed`, `Archived` |
| `FeatureStatus` | `Planned`, `InProgress`, `Completed`, `OnHold` |
| `ProjectTaskStatus` | `Todo`, `InProgress`, `Blocked`, `Done` |
| `TaskPriority` | `Low`, `Medium`, `High`, `Critical` |

Enum properties are stored as strings in PostgreSQL and serialized as strings
in API JSON.

### HTTP API

All routes are rooted at `/api`. GUID route parameters are shown as `{id}`,
`{projectId}`, or `{featureId}`.

| Method | Route | Behavior |
|---|---|---|
| `GET` | `/projects` | List projects; accepts `name` and `status` filters. |
| `GET` | `/projects/{id}` | Retrieve a project. |
| `POST` | `/projects` | Create a project; returns `201 Created` and a resource location. |
| `PUT` | `/projects/{id}` | Update project name and description. |
| `DELETE` | `/projects/{id}` | Delete a project; returns `204 No Content` when found. |
| `GET` | `/features` | List features; accepts `projectId`, `name`, and `status`. |
| `GET` | `/features/{id}` | Retrieve a feature. |
| `POST` | `/projects/{projectId}/features` | Create a feature for a project. |
| `GET` | `/tasks` | List tasks; accepts `projectId`, `featureId`, `search`, `status`, and `priority`. |
| `GET` | `/tasks/{id}` | Retrieve a task. |
| `POST` | `/features/{featureId}/tasks` | Create a task for a feature. |
| `POST` | `/users` | Register a user; returns a public user response, not the password/hash. |

Successful create operations return `201 Created`. Missing project/feature/task
lookups return `404 Not Found`; deleting a missing project also returns `404`.
Invalid values detected by the controllers/services/domain return `400 Bad
Request` Problem Details responses. MVC model binding also rejects malformed
GUIDs and invalid enum query values with `400`.

Example request bodies:

```json
{
  "name": "DevProject",
  "description": "Project tracking"
}
```

```json
{
  "title": "Implement sign-in",
  "description": "Add the authentication flow",
  "priority": "High",
  "order": 1
}
```

See [`DevProject.Api.http`](./src/DevProject.Api/DevProject.Api.http) for
sample requests for the implemented routes.

### Filtered searches

Search-query records live in Application under
[`Queries`](./src/DevProject.Application/Queries):

- `ProjectSearchQuery`: optional partial project name and project status.
- `FeatureSearchQuery`: optional project ID, partial feature name, and feature
  status.
- `ProjectTaskSearchQuery`: optional project ID, feature ID, partial title or
  description search, task status, and priority.

Name/text matching uses PostgreSQL `ILIKE` (case-insensitive substring
matching). Repository queries apply the filters and deterministic ordering in
the database and use `AsNoTracking` for these read-only searches. The API
returns the resulting collections without pagination.

### Persistence and migrations

The Infrastructure context is
[`AppDbContext`](./src/DevProject.Infrastructure/Context/AppDbContext.cs).
Entity configurations are discovered from the Infrastructure assembly.
Current persisted tables are `Projects`, `Features`, `Tasks`, and `Users`.
Indexes cover project status/creation, feature project/order/status, task
feature/order/status/due date, and unique normalized user email.

The initial migration is in
[`Persistence/Migrations`](./src/DevProject.Infrastructure/Persistence/Migrations).
The API does not automatically apply migrations at startup; apply migrations
to the configured database as a deployment/setup step before using the
endpoints. With the EF Core CLI available, apply it using:

```bash
dotnet ef database update \
  --project src/DevProject.Infrastructure/DevProject.Infrastructure.csproj \
  --startup-project src/DevProject.Api/DevProject.Api.csproj
```

The current model and migration configure cascading deletes from Project to
Feature and Feature to Task. Deleting a project can therefore remove its
features and tasks. User email uniqueness is enforced by a unique index on
`NormalizedEmail`.

### Run locally

Requirements:

- .NET 10 SDK.
- PostgreSQL compatible with the configured Npgsql provider.
- A database created for the application.

Configure the connection string through the standard ASP.NET Core environment
variable. Replace the placeholder with a local secret; do not commit real
credentials:

```bash
export ConnectionStrings__DefaultConnection='Host=localhost;Port=5432;Database=devproject;Username=postgres;Password=<your-password>'
dotnet run --project src/DevProject.Api/DevProject.Api.csproj
```

The launch profiles use `http://localhost:5001` and
`https://localhost:7000` (with HTTP also on port 5001). The API requires
`ConnectionStrings:DefaultConnection` outside the `Testing` environment.

Build and test:

```bash
dotnet restore
dotnet build DevProject.slnx
dotnet test DevProject.slnx
```

Infrastructure tests require PostgreSQL. They read
`ConnectionStrings__TestDatabase`; absent that variable, the fixture expects a
local test database on port `5433`. The CI workflow supplies its own
PostgreSQL service and connection string.

### Swagger UI

In the Development environment, open [`http://localhost:5001/swagger`](http://localhost:5001/swagger)
for the interactive Swagger UI. The generated document is available at
[`/swagger/v1/swagger.json`](http://localhost:5001/swagger/v1/swagger.json).
Swagger/OpenAPI middleware is disabled in environments other than Development
and Testing.

### Tests and CI

The GitHub Actions workflow
[`dotnet.yml`](./.github/workflows/dotnet.yml) runs on pushes and pull requests
targeting `main`. It provisions PostgreSQL, restores dependencies, builds the
solution, and runs all tests. The badge at the top links to the workflow runs
for `main`.

The four test projects cover Domain behavior, Application DTOs/mappers/services,
Infrastructure persistence against PostgreSQL, and API routes/HTTP behavior
including Swagger availability.

<a id="portugues"></a>

## Português

<a id="apresentacao-do-projeto"></a>

### Apresentação do projeto

A DevProject API é uma API inicial em ASP.NET Core para organizar projetos,
funcionalidades (*features*) e tarefas. Também contém os primeiros elementos
de domínio relacionados a usuários e autenticação. A solution segue um estilo
de Clean Architecture em camadas: os controllers HTTP chamam os serviços da
Application; a Application mapeia requisições e respostas e define contratos;
e a Infrastructure implementa a persistência com Entity Framework Core e
PostgreSQL.

A API atual é uma implementação inicial, não um produto completo de gestão de
projetos ou autenticação. As operações HTTP implementadas, os filtros
disponíveis, o comportamento da persistência e as lacunas conhecidas estão
descritos a seguir.

### Índice

- [Apresentação do projeto](#apresentacao-do-projeto)
- [Funcionalidades atuais](#funcionalidades-atuais)
- [Arquitetura e projetos](#arquitetura-e-projetos)
- [Modelo de domínio](#modelo-de-dominio)
- [API HTTP](#api-http)
- [Consultas filtradas](#consultas-filtradas)
- [Persistência e migrations](#persistencia-e-migrations)
- [Execução local](#execucao-local)
- [Swagger UI](#swagger-ui-1)
- [Testes e CI](#testes-e-ci-1)
- [Falhas, inconsistências e funcionalidades pendentes](#problemas-e-funcionalidades-pendentes)
- [English](#english)

<a id="funcionalidades-atuais"></a>

### Funcionalidades atuais

- Criar, consultar, atualizar e excluir projetos.
- Criar features associadas a projetos e consultar ou filtrar features.
- Criar tarefas associadas a features e consultar ou filtrar tarefas.
- Cadastrar usuários; as senhas são protegidas com hash BCrypt antes da
  persistência.
- Pesquisar projetos, features e tarefas usando filtros opcionais na query
  string.
- Persistir projetos, features, tarefas e usuários em PostgreSQL via EF Core.
- Explorar o contrato da API pelo Swagger UI no ambiente Development.
- Executar testes de domínio, Application, Infrastructure/PostgreSQL e API.

<a id="arquitetura-e-projetos"></a>

### Arquitetura e projetos

| Projeto | Responsabilidade |
|---|---|
| [`DevProject.Domain`](./src/DevProject.Domain) | Entidades, enums de status/prioridade e contratos genéricos de repositório/Unit of Work. |
| [`DevProject.Application`](./src/DevProject.Application) | DTOs, consultas de busca, serviços, contratos dos mappers, mapeamentos e registro de dependências. |
| [`DevProject.Infrastructure`](./src/DevProject.Infrastructure) | `AppDbContext` do EF Core, mapeamentos, repositórios PostgreSQL, Unit of Work e migrations. |
| [`DevProject.Api`](./src/DevProject.Api) | Controllers ASP.NET Core, composição da API, configuração de enums JSON, OpenAPI e Swagger UI. |
| [`DevProject.Domain.Tests`](./tests/DevProject.Domain.Tests) | Testes unitários de entidades e comportamentos do domínio. |
| [`DevProject.Application.Tests`](./tests/DevProject.Application.Tests) | Testes de DTOs, mappers e serviços da Application. |
| [`DevProject.Infrastructure.Tests`](./tests/DevProject.Infrastructure.Tests) | Testes de integração PostgreSQL para persistência e repositórios. |
| [`DevProject.Api.Tests`](./tests/DevProject.Api.Tests) | Testes HTTP de integração para controllers, respostas e Swagger. |

A solution está em [`DevProject.slnx`](./DevProject.slnx). A direção principal
das dependências é API → Application → Domain; a Infrastructure depende de
Application e Domain para implementar os contratos de persistência. Os
controllers retornam DTOs da Application, não entidades do Domain.

<a id="modelo-de-dominio"></a>

### Modelo de domínio

- **Project** possui nome, descrição opcional, status e features.
- **Feature** pertence a um projeto e possui nome, descrição opcional, status,
  ordem e tarefas.
- **ProjectTask** pertence a uma feature e guarda título, descrição opcional,
  status, prioridade, prazo opcional, data de conclusão e ordem. Ao passar para
  `Done`, define `CompletedAt`; ao mudar para outro status, limpa esse campo.
- **User** armazena perfil, e-mail normalizado, hash de senha e estado ativo.
- **RefreshToken** existe como entidade do Domain com hash, expiração e
  revogação, mas ainda não faz parte da persistência EF nem da API.
- A classe base `Entity` gera GUID e timestamp de criação UTC, além de rastrear
  `UpdatedAt`.

Os valores de status e prioridade são:

| Enum | Valores |
|---|---|
| `ProjectStatus` | `Planned`, `Active`, `OnHold`, `Completed`, `Archived` |
| `FeatureStatus` | `Planned`, `InProgress`, `Completed`, `OnHold` |
| `ProjectTaskStatus` | `Todo`, `InProgress`, `Blocked`, `Done` |
| `TaskPriority` | `Low`, `Medium`, `High`, `Critical` |

Enums são armazenados como strings no PostgreSQL e serializados como strings
no JSON da API.

<a id="api-http"></a>

### API HTTP

Todas as rotas começam com `/api`. Os parâmetros GUID são representados por
`{id}`, `{projectId}` ou `{featureId}`.

| Método | Rota | Comportamento |
|---|---|---|
| `GET` | `/projects` | Lista projetos; aceita filtros `name` e `status`. |
| `GET` | `/projects/{id}` | Consulta um projeto. |
| `POST` | `/projects` | Cria projeto; retorna `201 Created` e localização do recurso. |
| `PUT` | `/projects/{id}` | Atualiza nome e descrição do projeto. |
| `DELETE` | `/projects/{id}` | Exclui projeto; retorna `204 No Content` quando encontrado. |
| `GET` | `/features` | Lista features; aceita `projectId`, `name` e `status`. |
| `GET` | `/features/{id}` | Consulta uma feature. |
| `POST` | `/projects/{projectId}/features` | Cria feature para um projeto. |
| `GET` | `/tasks` | Lista tarefas; aceita `projectId`, `featureId`, `search`, `status` e `priority`. |
| `GET` | `/tasks/{id}` | Consulta uma tarefa. |
| `POST` | `/features/{featureId}/tasks` | Cria tarefa para uma feature. |
| `POST` | `/users` | Cadastra usuário; retorna dados públicos, sem senha/hash. |

Criações bem-sucedidas retornam `201 Created`. Consultas de projeto, feature ou
tarefa inexistentes retornam `404 Not Found`; a exclusão de projeto inexistente
também retorna `404`. Valores inválidos identificados por controller/serviço/
domínio retornam `400 Bad Request` no formato Problem Details. O binding do MVC
também rejeita GUIDs malformados e enums inválidos na query com `400`.

Exemplos de corpo:

```json
{
  "name": "DevProject",
  "description": "Acompanhamento de projetos"
}
```

```json
{
  "title": "Implementar autenticação",
  "description": "Adicionar fluxo de login",
  "priority": "High",
  "order": 1
}
```

Consulte [`DevProject.Api.http`](./src/DevProject.Api/DevProject.Api.http) para
exemplos das rotas implementadas.

<a id="consultas-filtradas"></a>

### Consultas filtradas

Os tipos de consulta ficam na camada Application, em
[`Queries`](./src/DevProject.Application/Queries):

- `ProjectSearchQuery`: nome parcial opcional e status do projeto.
- `FeatureSearchQuery`: ID do projeto, nome parcial da feature e status
  opcionais.
- `ProjectTaskSearchQuery`: IDs de projeto/feature, busca parcial por título ou
  descrição, status da tarefa e prioridade opcionais.

A busca textual usa `ILIKE` do PostgreSQL (correspondência parcial sem
diferenciar maiúsculas/minúsculas). Os repositórios aplicam filtros e ordenação
determinística no banco e usam `AsNoTracking` nessas consultas somente de
leitura. A API ainda retorna as coleções sem paginação.

<a id="persistencia-e-migrations"></a>

### Persistência e migrations

O contexto da Infrastructure é
[`AppDbContext`](./src/DevProject.Infrastructure/Context/AppDbContext.cs).
As configurações das entidades são descobertas no assembly da Infrastructure.
As tabelas atualmente persistidas são `Projects`, `Features`, `Tasks` e
`Users`. Há índices para status/criação de projetos, projeto/ordem/status de
features, feature/ordem/status/prazo de tarefas e unicidade do e-mail
normalizado.

A migration inicial está em
[`Persistence/Migrations`](./src/DevProject.Infrastructure/Persistence/Migrations).
A API não aplica migrations automaticamente ao iniciar; aplique-as ao banco
configurado antes de usar os endpoints. Com a ferramenta de linha de comando do
EF Core disponível, aplique-a com:

```bash
dotnet ef database update \
  --project src/DevProject.Infrastructure/DevProject.Infrastructure.csproj \
  --startup-project src/DevProject.Api/DevProject.Api.csproj
```

O modelo e a migration atuais configuram exclusão em cascata de Project para
Feature e de Feature para Task. Portanto, excluir um projeto pode excluir suas
features e tarefas. A unicidade do e-mail do usuário é garantida por índice
único em `NormalizedEmail`.

<a id="execucao-local"></a>

### Execução local

Requisitos:

- .NET 10 SDK.
- PostgreSQL compatível com o provedor Npgsql configurado.
- Um banco criado para a aplicação.

Configure a connection string pela variável de ambiente padrão do ASP.NET
Core. Substitua o valor de exemplo por um segredo local; não versione
credenciais reais:

```bash
export ConnectionStrings__DefaultConnection='Host=localhost;Port=5432;Database=devproject;Username=postgres;Password=<sua-senha>'
dotnet run --project src/DevProject.Api/DevProject.Api.csproj
```

Os perfis de execução usam `http://localhost:5001` e
`https://localhost:7000` (com HTTP também na porta 5001). A API exige
`ConnectionStrings:DefaultConnection` fora do ambiente `Testing`.

Build e testes:

```bash
dotnet restore
dotnet build DevProject.slnx
dotnet test DevProject.slnx
```

Os testes de Infrastructure exigem PostgreSQL. Eles leem
`ConnectionStrings__TestDatabase`; sem essa variável, o fixture espera um banco
de teste local na porta `5433`. O workflow de CI fornece seu próprio serviço
PostgreSQL e connection string.

<a id="swagger-ui-1"></a>

### Swagger UI

No ambiente Development, acesse
[`http://localhost:5001/swagger`](http://localhost:5001/swagger) para abrir o
Swagger UI interativo. O documento gerado está em
[`/swagger/v1/swagger.json`](http://localhost:5001/swagger/v1/swagger.json).
O middleware do Swagger/OpenAPI fica desativado fora dos ambientes Development
e Testing.

<a id="testes-e-ci-1"></a>

### Testes e CI

O workflow do GitHub Actions
[`dotnet.yml`](./.github/workflows/dotnet.yml) é executado em pushes e pull
requests direcionados a `main`. Ele provisiona PostgreSQL, restaura
dependências, compila a solution e executa todos os testes. A badge no início
do README leva às execuções do workflow para `main`.

Os quatro projetos de testes cobrem comportamentos do Domain, DTOs/mappers/
serviços da Application, persistência da Infrastructure no PostgreSQL e
comportamento HTTP da API, incluindo disponibilidade do Swagger.

<a id="known-issues-inconsistencies-and-missing-features"></a>
<a id="problemas-e-funcionalidades-pendentes"></a>

## Known issues, inconsistencies, and missing features / Problemas, inconsistências e funcionalidades pendentes

Esta seção registra limitações observadas na implementação atual; não significa
que esses itens já tenham sido implementados ou corrigidos.

### 1. Autenticação e autorização não implementadas

Os DTOs `LoginRequest`, `RefreshTokenRequest`, `RevokeTokenRequest` e
`AuthResponse` existem, mas não há endpoints ou serviços de login, emissão/
renovação/revogação de tokens, JWT, autenticação ou autorização. As rotas
existentes não exigem identidade/autorização. O registro de usuário é público.

**English:** Authentication and authorization are not implemented. Login,
refresh-token, revoke-token, and `AuthResponse` DTOs exist, but there are no
login/token endpoints or services, JWT setup, authentication middleware, or
authorization policies. Existing routes do not require an authenticated
identity, and registration is public.

### 2. RefreshToken não é persistido

`RefreshToken` existe no Domain, mas `AppDbContext`, mapeamentos, migration,
repositório e Application/API services não o incluem. A entidade não pode ser
armazenada nem usada por um fluxo de refresh token atualmente.

**English:** `RefreshToken` exists in the Domain but is absent from
`AppDbContext`, EF configurations, migrations, repositories, and
Application/API services. It cannot currently be persisted or used in a
refresh-token flow.

### 3. Operações incompletas para features e tarefas

Há DTOs de atualização de Feature e Task e um mapper de Task para status/ordem,
mas não há endpoints nem operações de serviço para atualizar ou excluir esses
recursos. Também não há endpoints de listagem/consulta de usuários, apesar de
existirem métodos de leitura em `IUserService`.

**English:** Feature and task update DTOs exist, and the task mapper can map
status/order changes, but the services and API have no update/delete
operations for these resources. User read methods exist in `IUserService`, but
the API exposes no user list or profile-read route.

### 4. Coleções sem paginação

Listagens e buscas filtradas materializam e retornam todas as linhas
correspondentes. Não há parâmetros de página, limites ou metadados de
paginação nos endpoints atuais; conjuntos grandes podem aumentar latência e
uso de memória.

**English:** List and filtered-search operations materialize and return every
matching row. The current endpoints have no page parameters, result limits, or
pagination metadata, which may increase latency and memory use for large
datasets.

### 5. Exclusão em cascata pode remover dados relacionados

As relações Project → Feature e Feature → Task usam `DeleteBehavior.Cascade` e
a migration inicial contém `ON DELETE CASCADE`. A rota de exclusão de projeto
pode, portanto, apagar todas as features e tarefas relacionadas. Isso
contradiz uma decisão anterior documentada de usar `Restrict`; a documentação
precisa acompanhar o comportamento configurado e a estratégia de exclusão deve
ser confirmada antes de produção.

**English:** Project → Feature and Feature → Task relationships use
`DeleteBehavior.Cascade`, and the initial migration contains `ON DELETE
CASCADE`. Deleting a project can therefore delete all its features and tasks.
This conflicts with an earlier documented `Restrict` decision; documentation
now reflects the code, but the deletion policy should be explicitly confirmed
before production.

### 6. Versões do EF Core/Npgsql não estão alinhadas ao .NET alvo

Os projetos miram `net10.0`, enquanto Infrastructure referencia
`Npgsql.EntityFrameworkCore.PostgreSQL` 9.0.2 e
`Microsoft.EntityFrameworkCore.Design` 9.0.21. O conjunto está na geração 9.x,
não alinhado à versão 10 do runtime; recomenda-se avaliar e alinhar as versões
do EF Core, provedor e ferramentas antes de evoluir migrations ou implantar.

**English:** Projects target `net10.0`, while Infrastructure references
`Npgsql.EntityFrameworkCore.PostgreSQL` 9.0.2 and
`Microsoft.EntityFrameworkCore.Design` 9.0.21. These are 9.x packages rather
than the .NET 10 generation; review and align EF Core, provider, and tooling
versions before expanding migrations or deploying.

### 7. Validação de entrada e tratamento de erros são parciais

Os DTOs não declaram validação por atributos de tamanho/formato. As entidades
validam alguns campos obrigatórios e enums; algumas ações traduzem
`ArgumentException` em Problem Details. Não há handler global para erros de
banco. Por exemplo, e-mail com formato inválido não é rejeitado pelo domínio,
duplicidade de e-mail viola o índice único, e textos maiores que os limites do
banco podem gerar erro de persistência não convertido em resposta de domínio
ou conflito (`409`).

**English:** DTOs do not declare length/format validation attributes. Entities
validate some required values and enums, and selected actions convert
`ArgumentException` to Problem Details, but there is no global database-error
handler. For example, malformed email addresses are not rejected by the
Domain, duplicate emails violate the unique index, and strings exceeding
database limits may produce persistence errors that are not translated to a
domain response or `409 Conflict`.

### 8. `Location` do cadastro aponta para uma rota inexistente

`POST /api/users` retorna um header `Location` em `/api/users/{id}`, mas não
existe `GET /api/users/{id}`. Uma rota de perfil deve ser adicionada junto com
uma política de autenticação/autorização adequada, em vez de expor dados de
conta sem proteção.

**English:** `POST /api/users` returns a `Location` header for
`/api/users/{id}`, but there is no `GET /api/users/{id}` endpoint. Add a
profile route only alongside an appropriate authentication/authorization
policy rather than exposing account data without protection.

### 9. Limitações da busca textual

Os filtros de texto usam `ILIKE` com `%` ao redor do valor fornecido. Caracteres
`%` e `_` enviados pelo usuário são curingas SQL e podem ampliar a busca. Se a
API precisar de busca literal, os curingas devem ser escapados; para busca
avançada, é necessário definir semântica, limites e estratégia de índice.

**English:** Text filters use `ILIKE` with `%` around the supplied value.
User-supplied `%` and `_` characters remain SQL wildcards and can broaden the
search. Escape them if literal matching is required; advanced search needs
explicit semantics, limits, and an indexing strategy.