variable "pipeline_name" {
  description = "Name of the CodePipeline"
  type        = string
  default     = "ledger-codepipeline"
}

variable "pipeline_type" {
  description = "Type of pipeline (V1 or V2)"
  type        = string
  default     = "V2"
}

variable "iam_role_name" {
  description = "Name of the IAM role for CodePipeline"
  type        = string
  default     = "codepipeline-role"
}

variable "iam_policy_name" {
  description = "Name of the IAM policy for CodePipeline"
  type        = string
  default     = "codepipeline-policy"
}

variable "s3_bucket_name" {
  description = "Name of the S3 bucket for pipeline artifacts"
  type        = string
  default     = "ledger-codepipeline"
}

variable "codestar_connection_name" {
  description = "Name of the CodeStar connection"
  type        = string
  default     = "ledger-connection"
}

variable "github_repository" {
  description = "Full GitHub repository ID (owner/repo)"
  type        = string
  default     = "AlmeidaFL/Ledger"
}

variable "github_branch" {
  description = "GitHub branch to monitor"
  type        = string
  default     = "main"
}

variable "detect_changes" {
  description = "Automatically detect changes"
  type        = bool
  default     = true
}

variable "trigger_branches" {
  description = "Branches that trigger the pipeline"
  type        = list(string)
  default     = ["main"]
}

variable "trigger_exclude_paths" {
  description = "File paths to exclude from triggering"
  type        = list(string)
  default     = ["**/k8s/*"]
}

variable "source_stage_name" {
  description = "Name of the source stage"
  type        = string
  default     = "LedgerSource"
}

variable "source_action_name" {
  description = "Name of the source action"
  type        = string
  default     = "LedgerSource"
}

variable "build_stage_name" {
  description = "Name of the build stage"
  type        = string
  default     = "Build"
}

variable "build_action_name" {
  description = "Name of the build action"
  type        = string
  default     = "Build"
}

variable "codebuild_project_name" {
  description = "Name of the CodeBuild project to use"
  type        = string
  default     = "ledger-codebuild"
}
