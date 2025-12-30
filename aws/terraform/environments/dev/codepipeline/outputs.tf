output "pipeline_name" {
  description = "Name of the CodePipeline"
  value       = module.codepipeline.pipeline_name
}

output "pipeline_arn" {
  description = "ARN of the CodePipeline"
  value       = module.codepipeline.pipeline_arn
}

output "pipeline_id" {
  description = "ID of the CodePipeline"
  value       = module.codepipeline.pipeline_id
}

output "iam_role_arn" {
  description = "ARN of the IAM role for CodePipeline"
  value       = module.codepipeline.iam_role_arn
}

output "s3_bucket_name" {
  description = "Name of the S3 bucket for artifacts"
  value       = module.codepipeline.s3_bucket_name
}

output "codestar_connection_arn" {
  description = "ARN of the CodeStar connection"
  value       = module.codepipeline.codestar_connection_arn
}

output "codestar_connection_status" {
  description = "Status of the CodeStar connection (needs manual activation)"
  value       = module.codepipeline.codestar_connection_status
}
