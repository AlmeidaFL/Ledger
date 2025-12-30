output "pipeline_name" {
  description = "Name of the CodePipeline"
  value       = aws_codepipeline.pipeline.name
}

output "pipeline_arn" {
  description = "ARN of the CodePipeline"
  value       = aws_codepipeline.pipeline.arn
}

output "pipeline_id" {
  description = "ID of the CodePipeline"
  value       = aws_codepipeline.pipeline.id
}

output "iam_role_arn" {
  description = "ARN of the IAM role for CodePipeline"
  value       = aws_iam_role.codepipeline_role.arn
}

output "iam_role_name" {
  description = "Name of the IAM role for CodePipeline"
  value       = aws_iam_role.codepipeline_role.name
}

output "s3_bucket_name" {
  description = "Name of the S3 bucket for artifacts"
  value       = aws_s3_bucket.codepipeline_artifacts.bucket
}

output "s3_bucket_arn" {
  description = "ARN of the S3 bucket for artifacts"
  value       = aws_s3_bucket.codepipeline_artifacts.arn
}

output "codestar_connection_arn" {
  description = "ARN of the CodeStar connection"
  value       = aws_codestarconnections_connection.github.arn
}

output "codestar_connection_status" {
  description = "Status of the CodeStar connection"
  value       = aws_codestarconnections_connection.github.connection_status
}
