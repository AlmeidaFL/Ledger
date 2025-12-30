output "helm_release_name" {
  description = "Name of the Helm release"
  value       = helm_release.kyverno.name
}

output "namespace" {
  description = "Namespace where Kyverno is deployed"
  value       = var.namespace
}

output "policy_name" {
  description = "Name of the ClusterPolicy"
  value       = var.policy_name
}
