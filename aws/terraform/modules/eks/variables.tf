variable "cluster_name" {
  description = "Name of the EKS cluster"
  type        = string
}

variable "vpc_id" {
  description = "VPC ID where the cluster will be deployed"
  type        = string
}

variable "subnet_ids" {
  description = "List of subnet IDs for the EKS cluster"
  type        = list(string)
}

variable "endpoint_public_access" {
  description = "Enable public API server endpoint"
  type        = bool
  default     = true
}

variable "kubernetes_version" {
  description = "Kubernetes version"
  type        = string
  default     = "1.31"
}

variable "iam_role_name" {
  description = "IAM role name for EKS cluster"
  type        = string
  default     = "ledger-cluster-role"
}

variable "enable_cluster_creator_admin_permissions" {
  description = "Enable cluster creator admin permissions"
  type        = bool
  default     = true
}

variable "node_iam_role_name" {
  description = "IAM role name for EKS nodes"
  type        = string
  default     = "ledger-node-role"
}

variable "node_min_size" {
  description = "Minimum number of nodes"
  type        = number
  default     = 2
}

variable "node_max_size" {
  description = "Maximum number of nodes"
  type        = number
  default     = 3
}

variable "node_desired_size" {
  description = "Desired number of nodes"
  type        = number
  default     = 2
}

variable "node_instance_types" {
  description = "Instance types for nodes"
  type        = list(string)
  default     = ["m5.xlarge"]
}

variable "node_ami_type" {
  description = "AMI type for nodes"
  type        = string
  default     = "AL2023_x86_64_STANDARD"
}

variable "node_capacity_type" {
  description = "Capacity type (ON_DEMAND or SPOT)"
  type        = string
  default     = "SPOT"
}

variable "node_disk_size" {
  description = "Disk size for nodes in GB"
  type        = number
  default     = 20
}

variable "node_environment" {
  description = "Environment label for nodes"
  type        = string
  default     = "test"
}

variable "node_project" {
  description = "Project label for nodes"
  type        = string
  default     = "ledger"
}
