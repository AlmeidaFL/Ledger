output "iam_role_arn" {
  description = "ARN of the IAM role for Load Balancer Controller"
  value       = module.loadbalancer.iam_role_arn
}

output "iam_role_name" {
  description = "Name of the IAM role for Load Balancer Controller"
  value       = module.loadbalancer.iam_role_name
}

output "helm_release_name" {
  description = "Name of the Helm release"
  value       = module.loadbalancer.helm_release_name
}
