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

data "terraform_remote_state" "eks" {
  backend = "local"

  config = {
    path = "../eks/terraform.tfstate"
  }
}

module "ebs_csi" {
  source = "../../../modules/ebs-csi"

  cluster_name = data.terraform_remote_state.eks.outputs.cluster_name
}
