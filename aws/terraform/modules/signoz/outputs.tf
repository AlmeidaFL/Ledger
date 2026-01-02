output "helm_release_name" {
  description = "Name of the Helm release"
  value       = helm_release.signoz.name
}

output "namespace" {
  description = "Namespace where SigNoz is deployed"
  value       = var.namespace
}
