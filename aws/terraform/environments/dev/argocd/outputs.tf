output "helm_release_name" {
  description = "Name of the Helm release"
  value       = module.argocd.helm_release_name
}

output "namespace" {
  description = "Namespace where ArgoCD is deployed"
  value       = module.argocd.namespace
}

output "bootstrap_app_name" {
  description = "Name of the bootstrap application"
  value       = module.argocd.bootstrap_app_name
}
