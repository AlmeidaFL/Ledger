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

# Look up the CodeStar connection for git clone operations
data "aws_codestarconnections_connection" "github" {
  name = "ledger-connection"
}

module "codebuild" {
  source = "../../../modules/codebuild"

  project_name            = "ledger-codebuild"
  iam_role_name           = "codebuild-role"
  source_location         = "https://github.com/AlmeidaFL/Ledger"
  buildspec_path          = "aws/terraform/resources/buildspec.yml"
  codestar_connection_arn = data.aws_codestarconnections_connection.github.arn
}
