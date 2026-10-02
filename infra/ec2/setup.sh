#!/usr/bin/env bash
# Setup de uma instancia EC2 Amazon Linux 2023 para rodar o OrderFlow via Docker Compose.
# Uso: como user-data no lancamento da instancia, ou copiado e executado manualmente via SSH.
set -euo pipefail

DEPLOY_USER="${DEPLOY_USER:-ec2-user}"
DEPLOY_DIR="/home/${DEPLOY_USER}/orderflow-deploy"

echo "== Atualizando pacotes do sistema =="
sudo dnf update -y

echo "== Instalando Docker =="
sudo dnf install -y docker

echo "== Habilitando e iniciando o servico do Docker no boot =="
sudo systemctl enable --now docker

echo "== Adicionando '${DEPLOY_USER}' ao grupo docker =="
sudo usermod -aG docker "${DEPLOY_USER}"

echo "== Instalando o plugin Docker Compose =="
# AL2023 nao tem "docker-compose-plugin" nos repositorios dnf padrao;
# o caminho suportado e baixar o binario oficial do GitHub Releases.
ARCH="$(uname -m)"
case "$ARCH" in
  x86_64)  COMPOSE_ARCH="x86_64" ;;
  aarch64) COMPOSE_ARCH="aarch64" ;;
  *) echo "Arquitetura nao suportada: $ARCH" >&2; exit 1 ;;
esac

sudo mkdir -p /usr/local/lib/docker/cli-plugins
sudo curl -fsSL \
  "https://github.com/docker/compose/releases/latest/download/docker-compose-linux-${COMPOSE_ARCH}" \
  -o /usr/local/lib/docker/cli-plugins/docker-compose
sudo chmod +x /usr/local/lib/docker/cli-plugins/docker-compose

echo "== Criando diretorio de deploy (${DEPLOY_DIR}) =="
sudo -u "${DEPLOY_USER}" mkdir -p "${DEPLOY_DIR}"

echo "== Verificacao =="
docker --version
docker compose version

echo
echo "Setup concluido."
echo "Se voce rodou isso via SSH (nao como user-data), faca logout/login como '${DEPLOY_USER}' antes"
echo "de usar 'docker' sem sudo -- a associacao ao grupo 'docker' so vale pra novas sessoes."
