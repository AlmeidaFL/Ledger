variable "project_name" {
  description = "Name of the CodeBuild project"
  type        = string
  default     = "ledger-codebuild"
}

variable "iam_role_name" {
  description = "Name of the IAM role for CodeBuild"
  type        = string
  default     = "codebuild-role"
}

variable "compute_type" {
  description = "Compute type for CodeBuild"
  type        = string
  default     = "BUILD_GENERAL1_SMALL"
}

variable "build_image" {
  description = "Docker image to use for builds"
  type        = string
  default     = "aws/codebuild/amazonlinux2-x86_64-standard:5.0"
}

variable "environment_type" {
  description = "Type of build environment"
  type        = string
  default     = "LINUX_CONTAINER"
}

variable "image_pull_credentials_type" {
  description = "Type of credentials to use for pulling images"
  type        = string
  default     = "CODEBUILD"
}

variable "privileged_mode" {
  description = "Enable privileged mode for Docker builds"
  type        = bool
  default     = true
}

variable "cloudwatch_log_group" {
  description = "CloudWatch log group name"
  type        = string
  default     = "log-group"
}

variable "cloudwatch_log_stream" {
  description = "CloudWatch log stream name"
  type        = string
  default     = "log-stream"
}

variable "source_type" {
  description = "Type of repository that contains the source code"
  type        = string
  default     = "GITHUB"
}

variable "source_location" {
  description = "Location of the source code repository"
  type        = string
  default     = "https://github.com/AlmeidaFL/Ledger"
}

variable "buildspec_path" {
  description = "Path to the buildspec file"
  type        = string
  default     = "aws/terraform/resources/buildspec.yml"
}

variable "codestar_connection_arn" {
  description = "ARN of the CodeStar connection for git clone operations"
  type        = string
  default     = ""
}
