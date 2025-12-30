output "project_name" {
  description = "Name of the CodeBuild project"
  value       = aws_codebuild_project.ledger_codebuild.name
}

output "project_arn" {
  description = "ARN of the CodeBuild project"
  value       = aws_codebuild_project.ledger_codebuild.arn
}

output "iam_role_arn" {
  description = "ARN of the IAM role for CodeBuild"
  value       = aws_iam_role.codebuild_role.arn
}

output "iam_role_name" {
  description = "Name of the IAM role for CodeBuild"
  value       = aws_iam_role.codebuild_role.name
}
