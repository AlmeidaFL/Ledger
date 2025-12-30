resource "aws_iam_role" "lb_controller_role" {
  name = var.iam_role_name

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Principal = {
          Service = "pods.eks.amazonaws.com"
        }
        Action = [
          "sts:AssumeRole",
          "sts:TagSession"
        ]
      }
    ]
  })
}

data "http" "lb_controller_policy_json" {
  url = "https://raw.githubusercontent.com/kubernetes-sigs/aws-load-balancer-controller/main/docs/install/iam_policy.json"
}

resource "aws_iam_policy" "lb_controller" {
  name   = var.iam_policy_name
  policy = data.http.lb_controller_policy_json.response_body
}

resource "aws_iam_role_policy_attachment" "lb_controller_policy" {
  policy_arn = aws_iam_policy.lb_controller.arn
  role       = aws_iam_role.lb_controller_role.name
}

resource "aws_eks_pod_identity_association" "lb_controller" {
  cluster_name    = var.cluster_name
  namespace       = var.namespace
  service_account = var.service_account
  role_arn        = aws_iam_role.lb_controller_role.arn
}

resource "helm_release" "aws_lb_controller" {
  name       = var.helm_release_name
  repository = var.helm_repository
  chart      = var.helm_chart
  namespace  = var.namespace
  version    = var.helm_version

  set = [
    {
      name  = "clusterName"
      value = var.cluster_name
    },
    {
      name  = "serviceAccount.create"
      value = "true"
    },
    {
      name  = "serviceAccount.name"
      value = var.service_account
    },
    {
      name  = "vpcId"
      value = var.vpc_id
    }
  ]
}
