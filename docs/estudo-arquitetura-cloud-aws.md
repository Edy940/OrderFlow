# Guia de estudo: Arquitetura Cloud para entrevista bancária (aplicado ao OrderFlow)

Objetivo: pegar os temas que caem em entrevista (balanceamento de carga, escalonamento, saturação,
latência, rate limiting, DDoS, polling/filas) e implementar cada um, de verdade, neste projeto — para
você conseguir falar "eu implementei isso" em vez de "eu li sobre isso".

Dividido em duas partes:
- **Parte 1** — o que dá pra codar agora, dentro do OrderFlow.Api (não depende de AWS, roda local/Docker).
- **Parte 2** — como subir isso na AWS e demonstrar os conceitos de infraestrutura de verdade.

---

## Parte 1 — Implementações no código (`src/OrderFlow.Api`)

### 1.1 Rate limiting / proteção contra abuso de requisições

O ASP.NET Core já tem rate limiting nativo (`Microsoft.AspNetCore.RateLimiting`, desde o .NET 7).
Vocês não têm isso hoje em [Program.cs](../src/OrderFlow.Api/Program.cs).

Adicionar antes de `var app = builder.Build();`:

```csharp
using System.Threading.RateLimiting;

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // política global: fixed window por IP
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    // política mais restritiva especificamente pro login (alvo clássico de brute force / DDoS)
    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});
```

E no pipeline (`app.UseAuthentication()` / antes de `MapControllers`):

```csharp
app.UseRateLimiter();
```

No controller de login, decorar a action com `[EnableRateLimiting("login")]`.

**O que isso te dá pra falar na entrevista:** throttling por IP, fixed window vs sliding window vs
token bucket (você pode citar os três algoritmos disponíveis na lib), e por que login/endpoints de
auth merecem limite mais agressivo (mitigação de credential stuffing, que é DDoS de aplicação).

### 1.2 Health checks (pré-requisito pro load balancer saber se a instância está saudável)

```csharp
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "postgres");
```

```csharp
app.MapHealthChecks("/health");
```

Pacote: `AspNetCore.HealthChecks.NpgSql`.

**Por quê:** todo load balancer (ALB da AWS incluso) precisa de um endpoint de health check pra saber
se deve tirar uma instância do pool. Isso conecta direto com "saturação" — se o Postgres está lento,
o health check pode começar a falhar antes do usuário sentir.

### 1.3 Resiliência (timeout, retry, circuit breaker) — usando Polly

Vocês consomem o próprio banco via EF Core; se algum dia chamarem uma API externa (ex.: serviço de
pagamento), isso é essencial. Adicionar `Microsoft.Extensions.Http.Resilience`:

```csharp
builder.Services.AddHttpClient("pagamentos")
    .AddStandardResilienceHandler(options =>
    {
        options.Retry.MaxRetryAttempts = 3;
        options.CircuitBreaker.FailureRatio = 0.5;
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);
    });
```

**O que isso te dá pra falar:** circuit breaker (evita que uma instância sobrecarregada derrube o
sistema inteiro — "saturação em cascata"), retry com backoff, timeout de requisição (conecta com
"tempo de requisição" que os recrutadores perguntam).

### 1.4 Connection pooling (você já tem, mas saiba explicar)

O Npgsql (driver do Postgres usado via `UseNpgsql` em
[OrderFlow.Infrastructure/Data](../src/OrderFlow.Infrastructure/Data)) já faz pooling de conexões por
padrão. Configuração explícita na connection string:

```
Maximum Pool Size=100;Minimum Pool Size=5;Connection Idle Lifetime=300
```

**Por quê:** é o "pool" que você mencionou — um conjunto de conexões TCP com o banco, reaproveitadas
em vez de abrir/fechar a cada requisição. Em bancos, esgotar o pool sob carga é uma causa clássica de
incidente ("saturação de conexões").

### 1.5 Logging estruturado com correlação (já existe, é seu diferencial)

Vocês já usam Serilog com `CorrelationId` no template
([Program.cs](../src/OrderFlow.Api/Program.cs)) e `UseSerilogRequestLogging()`. Isso já é observabilidade
básica — vale citar em entrevista como base pra medir p95/p99 de latência.

---

## Parte 2 — Deploy na AWS (arquitetura + passo a passo)

Arquitetura alvo:

```
Internet
   │
   ▼
Route 53 (DNS)
   │
   ▼
CloudFront (CDN) + AWS WAF/Shield  ← proteção DDoS / regras de firewall
   │
   ▼
Application Load Balancer (ALB)    ← balanceamento de carga + health checks
   │
   ├── ECS Fargate task (OrderFlow.Api) ── réplica 1
   ├── ECS Fargate task (OrderFlow.Api) ── réplica 2   ← pool de instâncias
   └── Auto Scaling (baseado em CPU/latência)           ← escalonamento
   │
   ▼
RDS PostgreSQL (Multi-AZ)
   │
CloudWatch (métricas, alarmes, logs)  ← monitoramento de saturação/latência
```

### Passo 1 — Containerizar (já feito)

Vocês já têm o [Dockerfile](../Dockerfile) funcional. Testar local:

```bash
docker build -t orderflow-api .
docker run -p 8080:8080 orderflow-api
```

### Passo 2 — Subir a imagem no ECR (Elastic Container Registry)

```bash
aws ecr create-repository --repository-name orderflow-api
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin <account-id>.dkr.ecr.us-east-1.amazonaws.com

docker tag orderflow-api:latest <account-id>.dkr.ecr.us-east-1.amazonaws.com/orderflow-api:latest
docker push <account-id>.dkr.ecr.us-east-1.amazonaws.com/orderflow-api:latest
```

### Passo 3 — Banco gerenciado (RDS PostgreSQL)

- Console AWS → RDS → Create database → PostgreSQL.
- Marcar **Multi-AZ** (alta disponibilidade — failover automático se a instância primária cair).
- Anotar o endpoint e configurar a connection string via **Secrets Manager** (nunca hardcoded).
- Ajustar `Maximum Pool Size` da seção 1.4 conforme o número de réplicas × conexões esperadas —
  isso é literalmente uma pergunta de entrevista: "como você evita esgotar conexões do banco quando
  escala horizontalmente?".

### Passo 4 — ECS Fargate (rodar os containers sem gerenciar servidor)

1. Criar um **cluster ECS** (Fargate).
2. Criar uma **Task Definition** apontando pra imagem do ECR, expondo a porta 8080 (igual ao Dockerfile).
3. Criar um **Service** com `Desired count = 2` (duas réplicas = seu "pool" de instâncias).
4. Vincular o service ao Target Group do ALB (passo 5), usando `/health` (seção 1.2) como health check path.

### Passo 5 — Application Load Balancer

1. Criar um **ALB** público, com um **Target Group** do tipo IP (Fargate).
2. Health check: path `/health`, intervalo 30s, threshold de falha 2-3 tentativas.
3. Isso é o "balanceamento de carga" da entrevista: o ALB distribui requisições entre as tasks
   saudáveis (algoritmo round-robin/least-outstanding-requests) e remove do pool qualquer task que
   falhe o health check.

### Passo 6 — Auto Scaling (escalonamento)

No ECS Service, configurar **Service Auto Scaling**:

- Métrica: `ECSServiceAverageCPUUtilization` (target ex.: 60%) ou `ALBRequestCountPerTarget`.
- Min tasks: 2, Max tasks: 6 (por exemplo).

Isso é a resposta pronta pra "o que acontece quando o sistema satura": o CloudWatch dispara o alarme,
o Auto Scaling sobe novas tasks, o ALB começa a rotear tráfego pra elas assim que passam no health
check.

### Passo 7 — Proteção contra DDoS e abuso

- **AWS Shield Standard**: já vem ativado de graça em tudo que usa ALB/CloudFront (mitigação de
  DDoS de rede/transporte — camada 3/4).
- **AWS WAF**: anexar ao ALB ou CloudFront com regras gerenciadas (`AWSManagedRulesCommonRuleSet`,
  `AWSManagedRulesAmazonIpReputationList`) + uma **rate-based rule** (ex.: bloquear IP que passar de
  2000 requisições em 5 minutos) — isso complementa o rate limiting de aplicação da seção 1.1, só que
  na borda da rede, antes de chegar no seu container.
- **CloudFront** na frente do ALB: cacheia respostas estáticas e absorve picos de tráfego antes de
  chegar na sua infraestrutura.

### Passo 8 — Monitoramento (latência, saturação)

CloudWatch, criar alarmes para:

- `TargetResponseTime` (latência do ALB até o container) — conecta com p95/p99.
- `HTTPCode_Target_5XX_Count` — erros de aplicação sob carga.
- `CPUUtilization` / `MemoryUtilization` das tasks — saturação de recursos.
- `RDS: DatabaseConnections` e `FreeableMemory` — saturação do banco.

Dashboard único no CloudWatch juntando os quatro é um ótimo artefato pra mostrar em entrevista
(print de tela ou descrever verbalmente a arquitetura).

### Passo 9 (opcional, avançado) — Polling assíncrono com SQS

Pra demonstrar o conceito de "polling em filas" que você perguntou:

1. Criar uma fila **SQS** (ex.: `pedidos-processamento`).
2. Ao criar um pedido, `OrderFlow.Application/Services` publica uma mensagem na fila em vez de
   processar tudo síncrono.
3. Um worker (pode ser o próprio `OrderFlow.Console` do projeto, rodando como ECS task separada ou
   Lambda) faz *polling* na fila (`ReceiveMessage` com `WaitTimeSeconds` — isso é **long polling**,
   evita chamadas vazias repetidas) e processa.
4. Isso desacopla a API do processamento pesado — se o processamento saturar, a fila absorve o pico
   em vez de derrubar a API (backpressure).

---

## Parte 3 — Como responder na entrevista (roteiro rápido)

Pra cada tema, uma frase de 15-20s conectando conceito → o que você implementou:

| Tema | Resposta modelo |
|---|---|
| Balanceamento de carga | "Uso ALB na frente de múltiplas tasks Fargate, com health check em `/health` que valida inclusive a conexão com o Postgres." |
| Escalonamento | "Auto Scaling do ECS baseado em CPU e request count, min 2 / max 6 tasks." |
| Saturação | "Monitoro connection pool do Npgsql e uso circuit breaker via Polly pra não deixar uma dependência lenta derrubar a API inteira." |
| Tempo de requisição | "Serilog com correlation ID em toda requisição, e CloudWatch alarme em `TargetResponseTime` p95." |
| Proteção de requisições | "Rate limiting nativo do ASP.NET Core por IP, política mais restritiva no endpoint de login." |
| DDoS | "Shield Standard automático + WAF com rate-based rule na borda, antes do tráfego chegar na aplicação." |
| Polling / filas | "SQS com long polling desacoplando criação de pedido do processamento, pra absorver picos." |

---

## Ordem sugerida de execução

1. Seção 1.1 (rate limiting) e 1.2 (health checks) — rápido, alto impacto, dá pra fazer hoje.
2. Commit + push.
3. Deploy manual na AWS seguindo Parte 2 (ou peça ajuda pra escrever Terraform/CDK em vez de clicar
   no console, se quiser algo versionado pra mostrar no GitHub).
4. Seção 1.3 (Polly) se sobrar tempo — é o item que mais impressiona em banco, porque resiliência é
   assunto sério pra sistemas de pagamento.
