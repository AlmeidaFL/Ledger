variable "helm_release_name" {
  description = "Name of the Helm release"
  type        = string
  default     = "signoz"
}

variable "helm_repository" {
  description = "Helm chart repository URL"
  type        = string
  default     = "https://charts.signoz.io"
}

variable "helm_chart" {
  description = "Name of the Helm chart"
  type        = string
  default     = "signoz"
}

variable "namespace" {
  description = "Kubernetes namespace for SigNoz"
  type        = string
  default     = "argocd"
}

variable "create_namespace" {
  description = "Create namespace if it doesn't exist"
  type        = bool
  default     = true
}

variable "helm_version" {
  description = "Version of the Helm chart"
  type        = string
  default     = "0.105.2"
}