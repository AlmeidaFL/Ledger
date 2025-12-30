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

# Reference network outputs via remote state or data source
# For now, using data sources to get existing VPC
data "terraform_remote_state" "network" {
  backend = "local"

  config = {
    path = "../network/terraform.tfstate"
  }
}

module "eks" {
  source = "../../../modules/eks"

  cluster_name = "ledger_cluster_v2"
  vpc_id       = data.terraform_remote_state.network.outputs.vpc_id
  subnet_ids   = data.terraform_remote_state.network.outputs.private_subnets
}
