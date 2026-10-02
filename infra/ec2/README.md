# Setup de nova instância EC2 (Amazon Linux 2023)

Substitui a instância antiga (Amazon Linux 2, fim de suporte) sem alterar o workflow de CD
(`.github/workflows/ci.yml`, job `deploy`) — ele continua apontando para o mesmo `EC2_HOST`
de sempre, contanto que esse secret seja um **Elastic IP** (ver passo 6).

## 1. Lançar a instância

- AMI: Amazon Linux 2023 (`al2023-ami-*-x86_64` ou `-arm64`, conforme o tipo de instância).
- **Key pair**: use o **mesmo par de chaves cujo conteúdo privado está no secret `EC2_SSH_KEY`**
  do GitHub. Se não tiver mais a chave original, você pode lançar com uma key pair nova e, no
  primeiro boot, adicionar a chave pública correspondente ao `EC2_SSH_KEY` atual em
  `~ec2-user/.ssh/authorized_keys` — mas é mais simples reaproveitar o par existente.
- **Security Group**: replique (ou reutilize) o da instância antiga — precisa liberar porta 22
  (SSH, de onde os runners do GitHub Actions e você mesmo conectam) e a porta 8080 (API), se
  ela for acessada de fora.

## 2. Rodar o `setup.sh`

**Opção A — via user-data (recomendado, zero passo manual depois do boot):**
Cole o conteúdo de [`setup.sh`](./setup.sh) no campo "User data" ao lançar a instância.

**Opção B — manualmente, depois do boot:**
```bash
scp -i /caminho/da/chave infra/ec2/setup.sh ec2-user@<IP_DA_NOVA_INSTANCIA>:~/setup.sh
ssh -i /caminho/da/chave ec2-user@<IP_DA_NOVA_INSTANCIA> "chmod +x ~/setup.sh && ~/setup.sh"
```

Critério de pronto: `docker --version` e `docker compose version` respondem sem erro, e o
diretório `~/orderflow-deploy` existe.

## 3. Testar manualmente antes de confiar no deploy automático

```bash
scp -i /caminho/da/chave docker-compose.prod.yml ec2-user@<IP_DA_NOVA_INSTANCIA>:~/orderflow-deploy/
ssh -i /caminho/da/chave ec2-user@<IP_DA_NOVA_INSTANCIA>
cd ~/orderflow-deploy

cat > .env <<'EOF'
IMAGE_NAME=ghcr.io/edy940/orderflow
IMAGE_TAG=latest
DB_CONNECTION_STRING=<connection string real>
JWT_CHAVE=<chave jwt real>
EOF

echo "<GITHUB_TOKEN ou PAT com read:packages>" | docker login ghcr.io -u <seu-usuario-github> --password-stdin
docker compose -f docker-compose.prod.yml pull
docker compose -f docker-compose.prod.yml up -d
curl http://localhost:8080/health   # espera 200
```

Se isso funcionar, a instância está pronta pro pipeline de CD assumir sozinho.

## 4. (Opcional) Migrar dados/estado

Se a instância antiga guarda algo com estado além do container da API (ex.: volumes, configs
manuais fora do compose), copie antes de desligá-la. O Postgres não é afetado por essa troca —
ele continua rodando de onde está (fora do `docker-compose.prod.yml`).

## 5. Trocar o Elastic IP

Isso é o que faz a troca ser invisível pro pipeline — **não mexe em nenhum secret nem no
workflow**, porque `EC2_HOST` já aponta para o Elastic IP, não para o IP dinâmico da instância.

Via console: EC2 → Elastic IPs → selecione o IP em uso → **Actions → Disassociate address**
→ depois **Actions → Associate address** apontando para a instância nova.

Via AWS CLI:
```bash
aws ec2 disassociate-address --association-id <association-id-da-instancia-antiga>
aws ec2 associate-address --instance-id <id-da-instancia-nova> --allocation-id <allocation-id-do-eip>
```

Depois de associar, rode de novo o passo 3 (curl no `/health`) usando o **Elastic IP** (não o
IP privado/público da instância) pra confirmar que é isso mesmo que o pipeline vai enxergar.

## 6. Desligar a instância antiga

Só depois de confirmar que a nova responde no Elastic IP e que um deploy automático (merge em
`master`, ou `workflow_dispatch` de rollback) funcionou de ponta a ponta nela. Pare (não termine
de cara) a instância antiga por um dia ou dois antes de terminar de vez, como rede de segurança.
