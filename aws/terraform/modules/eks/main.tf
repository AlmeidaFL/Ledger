module "eks" {
  source                                   = "terraform-aws-modules/eks/aws"
  version                                  = "21.10.1"
  name                                     = var.cluster_name
  endpoint_public_access                   = var.endpoint_public_access
  kubernetes_version                       = var.kubernetes_version
  iam_role_name                            = var.iam_role_name
  vpc_id                                   = var.vpc_id
  subnet_ids                               = var.subnet_ids
  enable_cluster_creator_admin_permissions = var.enable_cluster_creator_admin_permissions

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
    aws-ebs-csi-driver = {
      most_recent    = true
      before_compute = false
    }
  }

  eks_managed_node_groups = {
    projeto-ledger = {
      metadata_options = {
        http_endpoint               = "enabled"
        http_tokens                 = "required"
        http_put_response_hop_limit = 2
      }
      create_iam_role = true
      iam_role_name   = var.node_iam_role_name
      iam_role_additional_policies = {
        CloudWatchLogs = "arn:aws:iam::aws:policy/CloudWatchAgentServerPolicy"
        EcrPull        = "arn:aws:iam::aws:policy/AmazonEC2ContainerRegistryReadOnly"
      }
      min_size       = var.node_min_size
      max_size       = var.node_max_size
      desired_size   = var.node_desired_size
      instance_types = var.node_instance_types
      ami_type       = var.node_ami_type
      capacity_type  = var.node_capacity_type
      disk_size      = var.node_disk_size
      labels = {
        Environment = var.node_environment
        Project     = var.node_project
      }
      tags = {
        ExtraTag = "alguma-coisa"
      }
    }
  }
}
