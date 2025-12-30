variable "cluster_name" {
  description = "Name of the EKS cluster"
  type        = string
}

variable "vpc_id" {
  description = "VPC ID where the load balancer will operate"
  type        = string
}

variable "iam_role_name" {
  description = "Name of the IAM role for Load Balancer Controller"
  type        = string
  default     = "ledger-lb-controller-role"
}

variable "iam_policy_name" {
  description = "Name of the IAM policy for Load Balancer Controller"
  type        = string
  default     = "AWSLoadBalancerControllerIAMPolicy"
}

variable "namespace" {
  description = "Kubernetes namespace for the service account"
  type        = string
  default     = "kube-system"
}

variable "service_account" {
  description = "Kubernetes service account name"
  type        = string
  default     = "aws-load-balancer-controller"
}

variable "helm_release_name" {
  description = "Name of the Helm release"
  type        = string
  default     = "aws-load-balancer-controller"
}

variable "helm_repository" {
  description = "Helm chart repository URL"
  type        = string
  default     = "https://aws.github.io/eks-charts"
}

variable "helm_chart" {
  description = "Name of the Helm chart"
  type        = string
  default     = "aws-load-balancer-controller"
}

variable "helm_version" {
  description = "Version of the Helm chart"
  type        = string
  default     = "1.17.0"
}
