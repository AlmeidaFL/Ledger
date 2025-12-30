output "iam_role_arn" {
  description = "ARN of the IAM role for EBS CSI driver"
  value       = module.ebs_csi.iam_role_arn
}

output "iam_role_name" {
  description = "Name of the IAM role for EBS CSI driver"
  value       = module.ebs_csi.iam_role_name
}
