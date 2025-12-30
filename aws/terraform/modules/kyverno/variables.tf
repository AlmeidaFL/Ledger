variable "helm_release_name" {
  description = "Name of the Helm release"
  type        = string
  default     = "kyverno"
}

variable "helm_repository" {
  description = "Helm chart repository URL"
  type        = string
  default     = "https://kyverno.github.io/kyverno/"
}

variable "helm_chart" {
  description = "Name of the Helm chart"
  type        = string
  default     = "kyverno"
}

variable "helm_version" {
  description = "Version of the Helm chart"
  type        = string
  default     = "3.6.1"
}

variable "namespace" {
  description = "Kubernetes namespace for Kyverno"
  type        = string
  default     = "kyverno"
}

variable "create_namespace" {
  description = "Create namespace if it doesn't exist"
  type        = bool
  default     = true
}

variable "policy_name" {
  description = "Name of the Kyverno ClusterPolicy"
  type        = string
  default     = "resolve-ecr-alias"
}

variable "image_prefix" {
  description = "Image prefix to be replaced (e.g., 'private-repo/')"
  type        = string
  default     = "private-repo/"
}

variable "ecr_registry" {
  description = "ECR registry URL to replace with"
  type        = string
  default     = "010928197103.dkr.ecr.sa-east-1.amazonaws.com"
}
