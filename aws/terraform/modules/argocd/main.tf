resource "helm_release" "argocd" {
  name             = var.helm_release_name
  repository       = var.helm_repository
  chart            = var.helm_chart
  namespace        = var.namespace
  create_namespace = var.create_namespace
  version          = var.helm_version
}

resource "kubectl_manifest" "bootstrap_ledger" {
  yaml_body = <<-YAML
    apiVersion: argoproj.io/v1alpha1
    kind: Application
    metadata:
      name: ${var.bootstrap_app_name}
      namespace: ${var.namespace}
      finalizers:
        - resources-finalizer.argocd.argoproj.io
    spec:
      project: default
      source:
        repoURL: ${var.ledger_repo_url}
        targetRevision: ${var.ledger_repo_branch}
        path: ${var.ledger_repo_path}
        directory:
          recurse: false
      destination:
        server: https://kubernetes.default.svc
        namespace: ${var.namespace}
      syncPolicy:
        automated:
          prune: ${var.auto_prune}
          selfHeal: ${var.auto_self_heal}
  YAML

  depends_on = [helm_release.argocd]
}
