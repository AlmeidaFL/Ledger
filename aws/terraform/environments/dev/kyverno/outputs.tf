output "helm_release_name" {
  description = "Name of the Helm release"
  value       = module.kyverno.helm_release_name
}

output "namespace" {
  description = "Namespace where Kyverno is deployed"
  value       = module.kyverno.namespace
}

output "policy_name" {
  description = "Name of the ClusterPolicy"
  value       = module.kyverno.policy_name
}
