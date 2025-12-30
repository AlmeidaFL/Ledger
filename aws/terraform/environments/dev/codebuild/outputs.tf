output "project_name" {
  description = "Name of the CodeBuild project"
  value       = module.codebuild.project_name
}

output "project_arn" {
  description = "ARN of the CodeBuild project"
  value       = module.codebuild.project_arn
}

output "iam_role_arn" {
  description = "ARN of the IAM role for CodeBuild"
  value       = module.codebuild.iam_role_arn
}

output "iam_role_name" {
  description = "Name of the IAM role for CodeBuild"
  value       = module.codebuild.iam_role_name
}
