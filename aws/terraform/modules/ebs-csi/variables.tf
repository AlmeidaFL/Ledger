variable "cluster_name" {
  description = "Name of the EKS cluster"
  type        = string
}

variable "iam_role_name" {
  description = "Name of the IAM role for EBS CSI driver"
  type        = string
  default     = "ledger-ebs-csi-role"
}

variable "namespace" {
  description = "Kubernetes namespace for the service account"
  type        = string
  default     = "kube-system"
}

variable "service_account" {
  description = "Kubernetes service account name"
  type        = string
  default     = "ebs-csi-controller-sa"
}
