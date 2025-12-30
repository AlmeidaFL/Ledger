# Network Outputs
output "vpc_id" {
  description = "The ID of the VPC"
  value       = module.network.vpc_id
}

output "private_subnets" {
  description = "List of IDs of private subnets"
  value       = module.network.private_subnets
}

output "public_subnets" {
  description = "List of IDs of public subnets"
  value       = module.network.public_subnets
}

# EKS Outputs
output "cluster_name" {
  description = "Name of the EKS cluster"
  value       = module.eks.cluster_name
}

output "cluster_endpoint" {
  description = "Endpoint for EKS control plane"
  value       = module.eks.cluster_endpoint
}

output "cluster_id" {
  description = "The ID of the EKS cluster"
  value       = module.eks.cluster_id
}

# EBS CSI Outputs
output "ebs_csi_role_arn" {
  description = "ARN of the IAM role for EBS CSI driver"
  value       = module.ebs_csi.iam_role_arn
}

# Load Balancer Outputs
output "lb_controller_role_arn" {
  description = "ARN of the IAM role for Load Balancer Controller"
  value       = module.loadbalancer.iam_role_arn
}

# ArgoCD Outputs
output "argocd_namespace" {
  description = "Namespace where ArgoCD is deployed"
  value       = module.argocd.namespace
}

output "argocd_bootstrap_app" {
  description = "Name of the bootstrap application"
  value       = module.argocd.bootstrap_app_name
}

# Kyverno Outputs
output "kyverno_namespace" {
  description = "Namespace where Kyverno is deployed"
  value       = module.kyverno.namespace
}

# CodeBuild Outputs
output "codebuild_project_name" {
  description = "Name of the CodeBuild project"
  value       = module.codebuild.project_name
}

output "codebuild_project_arn" {
  description = "ARN of the CodeBuild project"
  value       = module.codebuild.project_arn
}

# CodePipeline Outputs
output "codepipeline_name" {
  description = "Name of the CodePipeline"
  value       = module.codepipeline.pipeline_name
}

output "codepipeline_arn" {
  description = "ARN of the CodePipeline"
  value       = module.codepipeline.pipeline_arn
}

output "codepipeline_s3_bucket" {
  description = "S3 bucket for pipeline artifacts"
  value       = module.codepipeline.s3_bucket_name
}

output "codestar_connection_arn" {
  description = "ARN of the CodeStar connection"
  value       = module.codepipeline.codestar_connection_arn
}

output "codestar_connection_status" {
  description = "Status of the CodeStar connection (needs manual activation in AWS Console)"
  value       = module.codepipeline.codestar_connection_status
}
