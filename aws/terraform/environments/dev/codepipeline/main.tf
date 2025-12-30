terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 6.0"
    }
  }
}

provider "aws" {
  region = "sa-east-1"
}

# Try to reference CodeBuild project from remote state if it exists
# Otherwise use the variable default
data "terraform_remote_state" "codebuild" {
  backend = "local"

  config = {
    path = "../codebuild/terraform.tfstate"
  }

  # If remote state doesn't exist, plan will use var.codebuild_project_name
  count = fileexists("../codebuild/terraform.tfstate") ? 1 : 0
}

module "codepipeline" {
  source = "../../../modules/codepipeline"

  pipeline_name            = "ledger-codepipeline"
  iam_role_name            = "codepipeline-role"
  s3_bucket_name           = "ledger-codepipeline"
  codestar_connection_name = "ledger-connection-luisfab"
  github_repository        = "AlmeidaFL/Ledger"
  github_branch            = "main"
  detect_changes           = false
  codebuild_project_name   = length(data.terraform_remote_state.codebuild) > 0 ? data.terraform_remote_state.codebuild[0].outputs.project_name : var.codebuild_project_name
}
