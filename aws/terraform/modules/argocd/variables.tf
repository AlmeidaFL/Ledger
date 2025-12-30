variable "helm_release_name" {
  description = "Name of the Helm release"
  type        = string
  default     = "argocd"
}

variable "helm_repository" {
  description = "Helm chart repository URL"
  type        = string
  default     = "https://argoproj.github.io/argo-helm"
}

variable "helm_chart" {
  description = "Name of the Helm chart"
  type        = string
  default     = "argo-cd"
}

variable "namespace" {
  description = "Kubernetes namespace for ArgoCD"
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
  default     = "9.1.10"
}

variable "bootstrap_app_name" {
  description = "Name of the bootstrap ArgoCD Application"
  type        = string
  default     = "bootstrap-ledger"
}

variable "ledger_repo_url" {
  description = "URL of the Ledger Git repository"
  type        = string
  default     = "https://github.com/AlmeidaFL/Ledger.git"
}

variable "ledger_repo_branch" {
  description = "Branch of the Ledger repository"
  type        = string
  default     = "main"
}

variable "ledger_repo_path" {
  description = "Path in the Ledger repository containing k8s manifests"
  type        = string
  default     = "k8s/argocd"
}

variable "auto_prune" {
  description = "Enable automatic pruning of resources"
  type        = bool
  default     = true
}

variable "auto_self_heal" {
  description = "Enable automatic self-healing"
  type        = bool
  default     = true
}
