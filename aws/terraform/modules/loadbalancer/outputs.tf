output "iam_role_arn" {
  description = "ARN of the IAM role for Load Balancer Controller"
  value       = aws_iam_role.lb_controller_role.arn
}

output "iam_role_name" {
  description = "Name of the IAM role for Load Balancer Controller"
  value       = aws_iam_role.lb_controller_role.name
}

output "helm_release_name" {
  description = "Name of the Helm release"
  value       = helm_release.aws_lb_controller.name
}
