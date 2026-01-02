resource "helm_release" "signoz" {
  name             = var.helm_release_name
  repository       = var.helm_repository
  chart            = var.helm_chart
  namespace        = var.namespace
  create_namespace = var.create_namespace
  version          = var.helm_version
}