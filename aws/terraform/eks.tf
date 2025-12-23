data "aws_availability_zones" "available" {

  filter {
    name   = "opt-in-status"
    values = ["opt-in-not-required"]
  }
}


module "vpc" {
  source  = "terraform-aws-modules/vpc/aws"
  version = "6.5.1"
  name    = "education-vpc"

  cidr = "10.0.0.0/16"
  azs  = slice(data.aws_availability_zones.available.names, 0, 3)

  private_subnets = ["10.0.1.0/24", "10.0.2.0/24", "10.0.3.0/24"]
  public_subnets  = ["10.0.4.0/24", "10.0.5.0/24", "10.0.6.0/24"]

  enable_nat_gateway   = true
  single_nat_gateway   = true
  enable_dns_hostnames = true
  enable_dns_support   = true

  public_subnet_tags = {
    "kubernetes.io/role/elb" = 1
  }

  private_subnet_tags = {
    "kubernetes.io/role/internal-elb" = 1
  }
}

#https://dev.to/aws-builders/gitops-with-argocd-on-amazon-eks-using-terraform-a-complete-implementation-guide-1inc
module "eks" {
  source                                   = "terraform-aws-modules/eks/aws"
  version                                  = "21.10.1"
  name                                     = local.ledger_cluster_name
  endpoint_public_access                   = true
  kubernetes_version                       = "1.31"
  iam_role_name                            = "ledger-cluster-role"
  vpc_id                                   = module.vpc.vpc_id
  subnet_ids                               = module.vpc.private_subnets
  enable_cluster_creator_admin_permissions = true


  addons = {
    coredns = {
      most_recent    = true
      before_compute = true
    }
    kube-proxy = {
      most_recent    = true
      before_compute = true
    }
    vpc-cni = {
      most_recent    = true
      before_compute = true
    }
    eks-pod-identity-agent = {
      most_recent    = true
      before_compute = true
    }
  }

  eks_managed_node_groups = {
    projeto-ledger = {
      create_iam_role = true
      iam_role_name   = "ledger-node-role"
      iam_role_additional_policies = {
        CloudWatchLogs = "arn:aws:iam::aws:policy/CloudWatchAgentServerPolicy"
      }
      min_size       = 1
      max_size       = 3
      desired_size   = 2
      instance_types = ["t3.medium"]
      ami_type       = "AL2023_x86_64_STANDARD"
      capacity_type  = "ON_DEMAND"
      disk_size      = 20
      labels = {
        Environment = "test"
        Project     = "ledger"
      }
      tags = {
        ExtraTag = "alguma-coisa"
      }
    }
  }


}