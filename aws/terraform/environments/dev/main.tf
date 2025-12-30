terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 6.0"
    }
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "2.38.0"
    }
    helm = {
      source  = "hashicorp/helm"
      version = "3.1.1"
    }
    kubectl = {
      source  = "gavinbunney/kubectl"
      version = ">= 1.19.0"
    }
  }
}

provider "aws" {
  region = "sa-east-1"
}

# Network Module
module "network" {
  source = "../../modules/network"

  vpc_name           = "education-vpc"
  vpc_cidr           = "10.0.0.0/16"
  private_subnets    = ["10.0.1.0/24", "10.0.2.0/24", "10.0.3.0/24"]
  public_subnets     = ["10.0.4.0/24", "10.0.5.0/24", "10.0.6.0/24"]
  enable_nat_gateway = true
  single_nat_gateway = true
}

# EKS Module
module "eks" {
  source = "../../modules/eks"

  cluster_name = "ledger_cluster_v2"
  vpc_id       = module.network.vpc_id
  subnet_ids   = module.network.private_subnets

  depends_on = [module.network]
}

# Configure Kubernetes providers after EKS is created
provider "kubectl" {
  host                   = module.eks.cluster_endpoint
  cluster_ca_certificate = base64decode(module.eks.cluster_certificate_authority_data)
  load_config_file       = false
  exec {
    api_version = "client.authentication.k8s.io/v1beta1"
    command     = "aws"
    args        = ["eks", "get-token", "--cluster-name", module.eks.cluster_name]
  }
}

provider "kubernetes" {
  host                   = module.eks.cluster_endpoint
  cluster_ca_certificate = base64decode(module.eks.cluster_certificate_authority_data)
  exec {
    api_version = "client.authentication.k8s.io/v1beta1"
    command     = "aws"
    args        = ["eks", "get-token", "--cluster-name", module.eks.cluster_name]
  }
}

provider "helm" {
  kubernetes = {
    host                   = module.eks.cluster_endpoint
    cluster_ca_certificate = base64decode(module.eks.cluster_certificate_authority_data)
    exec = {
      api_version = "client.authentication.k8s.io/v1beta1"
      command     = "aws"
      args        = ["eks", "get-token", "--cluster-name", module.eks.cluster_name]
    }
  }
}

# EBS CSI Module
module "ebs_csi" {
  source = "../../modules/ebs-csi"

  cluster_name = module.eks.cluster_name

  depends_on = [module.eks]
}

# Load Balancer Module
module "loadbalancer" {
  source = "../../modules/loadbalancer"

  cluster_name = module.eks.cluster_name
  vpc_id       = module.network.vpc_id

  depends_on = [module.eks]
}

# ArgoCD Module
module "argocd" {
  source = "../../modules/argocd"

  depends_on = [module.eks]
}

# Kyverno Module
module "kyverno" {
  source = "../../modules/kyverno"

  depends_on = [module.eks]
}

# CodeBuild Module (independent)
module "codebuild" {
  source = "../../modules/codebuild"

  project_name    = "ledger-codebuild"
  iam_role_name   = "codebuild-role"
  source_location = "https://github.com/AlmeidaFL/Ledger"
  buildspec_path  = "aws/terraform/resources/buildspec.yml"
}

# CodePipeline Module (depends on CodeBuild)
module "codepipeline" {
  source = "../../modules/codepipeline"

  pipeline_name            = "ledger-codepipeline"
  iam_role_name            = "codepipeline-role"
  s3_bucket_name           = "ledger-codepipeline"
  codestar_connection_name = "ledger-connection-luisfab"
  github_repository        = "AlmeidaFL/Ledger"
  github_branch            = "main"
  detect_changes           = false
  codebuild_project_name   = module.codebuild.project_name

  depends_on = [module.codebuild]
}
