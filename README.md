# OrderFlow API

API REST para gerenciamento de clientes, produtos e pedidos, desenvolvida como projeto de portfólio e treinamento prático em ASP.NET Core.

O objetivo do projeto é aplicar Clean Architecture, princípios SOLID, separação de responsabilidades, baixo acoplamento, testes e práticas comuns de APIs utilizadas em ambientes profissionais.

## Tecnologias

- .NET 9 e ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Swagger / OpenAPI
- FluentValidation
- AutoMapper
- Serilog
- xUnit e Moq

## Arquitetura

```text
src/
├── OrderFlow.Api            # Controllers, middlewares e configuração HTTP
├── OrderFlow.Application    # Casos de uso, serviços, DTOs e paginação
├── OrderFlow.Domain         # Entidades, regras e contratos do domínio
└── OrderFlow.Infrastructure # Entity Framework e repositórios

OrderFlow.Tests              # Testes automatizados
```

O fluxo esperado de uma requisição é:

```text
Controller -> Serviço de aplicação -> Interface do repositório -> Infrastructure -> Banco
```

## Progresso do projeto

### Base da aplicação

- [x] Separação em Api, Application, Domain e Infrastructure
- [x] Entidades de Cliente, Produto, Pedido e ItemPedido
- [x] Padrão Repository com injeção de dependência
- [x] Serviços de aplicação para pedidos
- [x] DTOs para entrada e saída de dados
- [x] Mapeamento com AutoMapper
- [x] Persistência com Entity Framework Core e PostgreSQL
- [x] Migrations do banco de dados
- [x] Documentação e testes manuais com Swagger
- [x] Validação de criação de pedidos com FluentValidation
- [x] Testes unitários iniciais com xUnit e Moq

### Observabilidade e tratamento de erros

- [x] Logging estruturado com Serilog no console
- [x] Logging em arquivos diários na pasta `Logs`
- [x] Logging automático das requisições HTTP
- [x] Logs de operações nos controllers
- [ ] Registrar e validar o middleware global de exceções no pipeline
- [ ] Integrar e validar o Correlation ID em todas as requisições e logs
- [ ] Padronizar as respostas de erro, por exemplo com `ProblemDetails`

### Evolução da API

- [ ] Concluir paginação de clientes
  - [x] Criar `ParametrosPaginacao`
  - [x] Criar `ResultadoPaginado<T>`
  - [ ] Adicionar consulta paginada ao repositório
  - [ ] Criar serviço de aplicação para clientes
  - [ ] Retornar DTOs paginados no controller
  - [ ] Testar páginas e limites pelo Swagger
- [ ] Implementar filtros
- [ ] Implementar ordenação
- [ ] Implementar autenticação com JWT
- [ ] Implementar Refresh Token
- [ ] Implementar autorização por perfil ou política
- [ ] Implementar versionamento da API
- [ ] Ampliar a cobertura de testes unitários
- [ ] Adicionar testes de integração da API
- [ ] Adicionar Docker e Docker Compose
- [ ] Configurar integração contínua (CI)

## Como executar

### Pré-requisitos

- .NET SDK 9
- PostgreSQL
- Certificado HTTPS de desenvolvimento configurado

Configure a conexão com o PostgreSQL em `src/OrderFlow.Api/appsettings.json` e execute as migrations:

```powershell
dotnet ef database update --project src/OrderFlow.Infrastructure --startup-project src/OrderFlow.Api
```

Inicie a API a partir da raiz do repositório:

```powershell
dotnet run --project src/OrderFlow.Api
```

A API utiliza atualmente:

- Swagger: `https://localhost:5001/swagger`
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

## Build e testes

Pare qualquer instância da API antes de compilar, para evitar que arquivos `.dll` fiquem bloqueados.

```powershell
dotnet build OrderFlow.sln
dotnet test OrderFlow.sln
```

## Próximo passo

Concluir a paginação de clientes mantendo o controller enxuto: a camada Application deve coordenar o caso de uso e a Infrastructure deve executar `OrderBy`, `Skip` e `Take` no banco de dados.
