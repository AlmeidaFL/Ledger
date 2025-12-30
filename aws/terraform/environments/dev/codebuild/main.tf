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

module "codebuild" {
  source = "../../../modules/codebuild"

  project_name   = "ledger-codebuild"
  iam_role_name  = "codebuild-role"
  source_location = "https://github.com/AlmeidaFL/Ledger"
  buildspec_path  = "aws/terraform/resources/buildspec.yml"
}
