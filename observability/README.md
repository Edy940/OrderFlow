# Observabilidade (Prometheus + Grafana + Loki)

Stack de observabilidade gratuita: métricas (Prometheus), logs (Loki/Promtail) e dashboards
(Grafana). Roda junto com a API no mesmo `docker-compose.prod.yml`, mas **Grafana e Prometheus
não ficam expostos publicamente** — só acessíveis via túnel SSH (ver passo 4).

Esses arquivos de configuração (Prometheus, Promtail, provisioning do Grafana) **não são
copiados automaticamente pelo pipeline de CD** — só o `docker-compose.prod.yml` é. Isso é
proposital, pra não alterar a automação de deploy existente. Eles mudam raramente (dashboards,
scrape config), então basta copiar de novo manualmente se algo aqui mudar.

## 1. Cadastrar o secret que falta

Repo → Settings → Secrets and variables → Actions:
- `GRAFANA_ADMIN_PASSWORD` — senha do usuário `admin` do Grafana.

(Isso é lido automaticamente pelo workflow existente, que já escreve o `.env` na EC2 a cada
deploy — só adicionei uma linha nele pra incluir essa variável.)

## 2. Copiar esta pasta pra EC2 (uma vez)

```bash
scp -r -i /caminho/da/chave observability ec2-user@<EC2_HOST>:~/orderflow-deploy/
```

Resultado esperado: `~/orderflow-deploy/observability/...` com a mesma estrutura daqui.

## 3. Deixar o próximo deploy automático subir os novos serviços

Um merge em `master` (ou um `workflow_dispatch`) já vai rodar `docker compose pull && up -d`
com o `docker-compose.prod.yml` atualizado — isso sobe `prometheus`, `loki`, `promtail` e
`grafana` junto com a `api`, sem precisar mexer em mais nada manualmente.

Se quiser testar antes, sem esperar o pipeline, dá pra rodar direto na EC2:
```bash
cd ~/orderflow-deploy
docker compose -f docker-compose.prod.yml up -d prometheus loki promtail grafana
```

## 4. Acessar o Grafana via túnel SSH

```bash
ssh -i /caminho/da/chave -L 3000:localhost:3000 ec2-user@<EC2_HOST>
```
Deixe essa sessão aberta e acesse `http://localhost:3000` no seu navegador. Login: `admin` /
a senha do secret `GRAFANA_ADMIN_PASSWORD`.

O dashboard **OrderFlow API** já vem provisionado (pasta "OrderFlow" no menu Dashboards) com:
status da API (up/down), requisições por segundo, taxa de erro 5xx e tempo de resposta p95.
Há também um painel de logs (via Loki) com a saída dos containers.

Prometheus (`http://localhost:9090`, mesmo túnel, trocando a porta) também fica acessível só
localmente, caso queira consultar métricas cruas.

## Como funciona por baixo dos panos

- A API expõe métricas em `/metrics` (pacote `prometheus-net.AspNetCore`); o Prometheus faz
  scrape a cada 15s via rede interna do compose (`api:8080/metrics` — não depende da porta
  8080 estar publicada, é comunicação container-a-container).
- O Promtail lê os arquivos de log que o Docker já grava em
  `/var/lib/docker/containers/*/*-json.log` (mesma máquina, montado read-only) e envia pro
  Loki. Não precisa de nenhum plugin de log driver extra instalado na instância.
- O Grafana já vem com Prometheus e Loki cadastrados como datasource (provisionados via
  arquivo, `provisioning/datasources/datasources.yml`) e o dashboard carregado automaticamente
  (`provisioning/dashboards/` + `dashboards/orderflow-api.json`) — nada disso é feito na mão
  pela UI.

## Nota de segurança

`/metrics` na API fica no mesmo endpoint público 8080 que o resto da API (não tem proxy/auth
na frente). Não expõe dado sensível de negócio, só contadores de requisição — mas se isso for
um problema pra você, dá pra restringir depois (ex.: middleware que só libera `/metrics` para
IPs internos, ou mover pra uma porta separada não publicada).
