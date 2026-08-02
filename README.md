# OrderFlow API

API REST para gerenciamento de clientes, produtos e pedidos, desenvolvida como projeto de portfólio e treinamento prático em ASP.NET Core.

O projeto aplica Clean Architecture, princípios SOLID, separação de responsabilidades, baixo acoplamento, testes automatizados e práticas comuns de APIs profissionais.

## Tecnologias atuais

- .NET 9 e ASP.NET Core Web API
- Entity Framework Core e PostgreSQL
- Swagger / OpenAPI com versionamento
- FluentValidation e AutoMapper
- JWT e Refresh Token
- Serilog
- xUnit e Moq

## Arquitetura

```text
src/
├── OrderFlow.Api            # Controllers, middlewares e configuração HTTP
├── OrderFlow.Application    # Casos de uso, serviços e DTOs
├── OrderFlow.Domain         # Entidades, regras, consultas e contratos
└── OrderFlow.Infrastructure # Entity Framework, repositórios e segurança

OrderFlow.Tests              # Testes automatizados
```

Fluxo principal:

```text
Controller → Application Service → Interface do repositório → Infrastructure → Banco
```

A API não expõe `IQueryable` nem detalhes do Entity Framework fora da Infrastructure. Filtros e ordenação são representados por contratos tipados do domínio.

## Funcionalidades

- Cadastro e consulta de clientes e produtos
- Criação e consulta de pedidos
- Paginação, filtros e ordenação de pedidos
- Autenticação JWT
- Rotação e revogação de Refresh Tokens
- Senhas protegidas com PBKDF2
- Refresh Tokens persistidos somente como hash
- Respostas de erro padronizadas com `ProblemDetails`
- Logging estruturado em console e arquivos diários
- API versionada por segmento de URL

## Endpoints e versionamento

A versão atual é a `v1`:

```text
/api/v1/Clientes
/api/v1/Produtos
/api/v1/Pedidos
/api/v1/auth/registrar
/api/v1/auth/login
/api/v1/auth/refresh
/api/v1/auth/revogar
```

Exemplo de consulta paginada, filtrada e ordenada:

```http
GET /api/v1/Pedidos?pagina=1&tamanhoPagina=10&valorMinimo=100&ordenarPor=valorTotal&direcao=desc
```

Filtros disponíveis para pedidos:

- `clienteId`
- `dataInicio` e `dataFim`
- `valorMinimo` e `valorMaximo`
- `ordenarPor`: `data`, `cliente` ou `valorTotal`
- `direcao`: `asc` ou `desc`

## Roadmap

### Fase 1 — Base e Clean Architecture

- [x] Separação em Api, Application, Domain e Infrastructure
- [x] Entidades e regras de domínio
- [x] Repository e injeção de dependência
- [x] Serviços de aplicação e DTOs
- [x] Entity Framework Core e PostgreSQL
- [x] Migrations
- [x] FluentValidation e AutoMapper
- [x] Testes unitários iniciais

### Fase 2 — API Profissional

- [x] Middleware de exceções
- [x] Paginação
- [x] Filtros
- [x] Ordenação
- [x] JWT
- [x] Refresh Token
- [x] Versionamento da API
- [x] Logging

### Fase 3 — DevOps

> Próximo ponto de retomada: criar o **Docker Compose** com API e PostgreSQL.

- [x] Docker
- [ ] Docker Compose
- [ ] GitHub Actions
- [ ] SonarQube
- [ ] Cobertura de testes

### Fase 4 — Arquitetura Distribuída

- [ ] RabbitMQ
- [ ] Worker Service
- [ ] Eventos
- [ ] Retry
- [ ] Dead Letter Queue
- [ ] Idempotência

### Fase 5 — Performance

- [ ] Redis
- [ ] Cache
- [ ] CQRS completo
- [ ] Queries otimizadas

### Fase 6 — Cloud

- [ ] Deploy AWS
- [ ] EC2
- [ ] RDS
- [ ] S3
- [ ] CloudWatch

### Fase 7 — Observabilidade

- [x] Serilog
- [ ] OpenTelemetry
- [ ] Prometheus
- [ ] Grafana
- [ ] Tracing

## Como executar

### Pré-requisitos

- .NET SDK 9
- PostgreSQL
- Certificado HTTPS de desenvolvimento configurado

Configure a conexão com o PostgreSQL em `src/OrderFlow.Api/appsettings.json` e aplique as migrations:

```powershell
dotnet ef database update --project src/OrderFlow.Infrastructure --startup-project src/OrderFlow.Api
```

Inicie a API:

```powershell
dotnet run --project src/OrderFlow.Api --launch-profile https
```

Endereços locais:

- Swagger: `https://localhost:7225/swagger`
- HTTP: `http://localhost:5186`
- HTTPS: `https://localhost:7225`

## Docker

Construa a imagem a partir da raiz do repositório:

```powershell
docker build -t orderflow-api .
```

O projeto utiliza build multi-stage, imagem final do ASP.NET Core, porta HTTP `8080` e execução como usuário sem privilégios administrativos. A API e o PostgreSQL serão executados juntos na próxima etapa, com Docker Compose.

## Autenticação

Registre um usuário ou faça login. Em seguida, envie o `accessToken` no cabeçalho:

```http
Authorization: Bearer ACCESS_TOKEN
```

O Refresh Token deve ser enviado somente no corpo dos endpoints de renovação ou revogação. Em produção, configure a chave JWT por variável de ambiente:

```powershell
$env:Jwt__Chave = "uma-chave-secreta-aleatoria-com-pelo-menos-32-caracteres"
```

## Build e testes

Pare qualquer instância da API antes de compilar para evitar bloqueio de arquivos `.dll`.

```powershell
dotnet build OrderFlow.sln
dotnet test OrderFlow.sln
```

Estado atual: **12 testes automatizados aprovados e nenhuma falha**.

## Próximo passo

Continuar a **Fase 3 — DevOps** criando o `docker-compose.yml` com API, PostgreSQL, volume persistente, rede interna e health checks.
