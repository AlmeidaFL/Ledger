resource "helm_release" "argocd" {
  name             = "argocd"
  repository       = "https://argoproj.github.io/argo-helm"
  chart            = "argo-cd"
  namespace        = "argocd"
  create_namespace = true
  version          = "9.1.10"
}

resource "kubectl_manifest" "bootstrap_ledger" {
  yaml_body = <<-YAML
    apiVersion: argoproj.io/v1alpha1
    kind: Application
    metadata:
      name: bootstrap-ledger
      namespace: argocd
      finalizers:
        - resources-finalizer.argocd.argoproj.io
    spec:
      project: default
      source:
        repoURL: https://github.com/AlmeidaFL/Ledger.git
        targetRevision: main
        path: k8s/argocd
        directory:
          recurse: false
      destination:
        server: https://kubernetes.default.svc
        namespace: argocd
      syncPolicy:
        automated:
          prune: true
          selfHeal: true
  YAML

  depends_on = [helm_release.argocd]
}
# 
# resource "kubernetes_manifest" "bootstrap_ledger" {
#   manifest = {
#     apiVersion = "argoproj.io/v1alpha1"
#     kind       = "Application"
#     metadata = {
#       name       = "bootstrap-ledger"
#       namespace  = "argocd"
#       finalizers = ["resources-finalizer.argocd.argoproj.io"]
#     }
#     spec = {
#       project = "default"
#       source = {
#         repoURL        = "https://github.com/AlmeidaFL/Ledger.git"
#         targetRevision = "main"
#         path           = "k8s/argocd"
#         directory = {
#           recurse = false
#         }
#       }
#       destination = {
#         server    = "https://kubernetes.default.svc"
#         namespace = "argocd"
#       }
#       syncPolicy = {
#         automated = {
#           prune    = true
#           selfHeal = true
#         }
#       }
#     }
#   }
# 
#   depends_on = [helm_release.argocd]
# }