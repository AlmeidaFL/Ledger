#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "==> Applying global infrastructure"

kubectl apply -f "$SCRIPT_DIR/svc-account.yaml"
kubectl apply -f "$SCRIPT_DIR/postgres.yaml"
kubectl apply -f "$SCRIPT_DIR/kafka.yaml"

echo "==> Applying service infrastructure"

kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.12.0/deploy/static/provider/cloud/deploy.yaml

SERVICES=(
  "Client"
  "SimpleAuth"
  "UserApi"
  "EventRelayWorker"
  "FinancialService"
  "LedgerGateway"
)

for svc in "${SERVICES[@]}"; do
  INFRA_FILE="$SCRIPT_DIR/../$svc/k8s/infra.yaml"

  if [[ -f "$INFRA_FILE" ]]; then
    echo "----> $svc infra"
    kubectl apply -f "$INFRA_FILE"
  else
    echo "----> $svc infra not found, skipping"
  fi
done

echo "==> Ledger cluster started successfully"
