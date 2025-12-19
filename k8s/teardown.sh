#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "==> Tearing down Ledger cluster"

echo "==> Removing service infrastructure"

SERVICES=(
  "Client"
  "LedgerGateway"
  "FinancialService"
  "EventRelayWorker"
  "UserApi"
  "SimpleAuth"
)

for svc in "${SERVICES[@]}"; do
  INFRA_FILE="$SCRIPT_DIR/../$svc/k8s/infra.yaml"

  if [[ -f "$INFRA_FILE" ]]; then
    echo "----> $svc infra"
    kubectl delete -f "$INFRA_FILE" --ignore-not-found
  else
    echo "----> $svc infra not found, skipping"
  fi
done

echo "==> Removing global infrastructure"

kubectl delete -f "$SCRIPT_DIR/kafka.yaml" --ignore-not-found
kubectl delete -f "$SCRIPT_DIR/postgres.yaml" --ignore-not-found
kubectl delete -f "$SCRIPT_DIR/svc-account.yaml" --ignore-not-found

echo "==> Ledger cluster torn down"
