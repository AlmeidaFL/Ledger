output "helm_release_name" {
  description = "Name of the Helm release"
  value       = helm_release.argocd.name
}

output "namespace" {
  description = "Namespace where ArgoCD is deployed"
  value       = var.namespace
}

output "bootstrap_app_name" {
  description = "Name of the bootstrap application"
  value       = var.bootstrap_app_name
}
