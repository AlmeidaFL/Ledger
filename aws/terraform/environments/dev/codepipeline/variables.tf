variable "codebuild_project_name" {
  description = "Name of the CodeBuild project (used if codebuild state doesn't exist)"
  type        = string
  default     = "ledger-codebuild"
}
