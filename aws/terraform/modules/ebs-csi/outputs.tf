output "iam_role_arn" {
  description = "ARN of the IAM role for EBS CSI driver"
  value       = aws_iam_role.ebs_csi_role.arn
}

output "iam_role_name" {
  description = "Name of the IAM role for EBS CSI driver"
  value       = aws_iam_role.ebs_csi_role.name
}
