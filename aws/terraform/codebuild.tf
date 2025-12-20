data "aws_caller_identity" "current" {}

data "aws_region" "current" {}

resource "aws_s3_bucket" "ledger-codebuild" {
  bucket = "ledger-codebuild"
}

data "aws_iam_policy_document" "codebuild-assume-policy" {
  statement {
    effect = "Allow"

    principals {
      type        = "Service"
      identifiers = ["codebuild.amazonaws.com"]
    }

    actions = ["sts:AssumeRole"]
  }

}

resource "aws_iam_role" "codebuild-role" {
  name               = "codebuild-role"
  assume_role_policy = data.aws_iam_policy_document.codebuild-assume-policy.json
}

data "aws_iam_policy_document" "codebuild_permissions" {
  statement {
    effect = "Allow"
    actions = ["*"]
    resources = ["*"]
  }
}

resource "aws_iam_role_policy" "codebuild_policy" {
  role   = aws_iam_role.codebuild-role.id
  policy = data.aws_iam_policy_document.codebuild_permissions.json
}

resource "aws_codebuild_project" "ledger-codebuild" {
  name         = "ledger-codebuild"
  service_role = aws_iam_role.codebuild-role.arn

  artifacts {
    type = "NO_ARTIFACTS"
  }

  environment {
    compute_type                = "BUILD_GENERAL1_SMALL"
    image                       = "aws/codebuild/amazonlinux2-x86_64-standard:4.0"
    type                        = "LINUX_CONTAINER"
    image_pull_credentials_type = "CODEBUILD"
    privileged_mode = true

    environment_variable {
        name  = "AWS_ACCOUNT_ID"
        value = data.aws_caller_identity.current.account_id
    }

    environment_variable {
        name  = "AWS_REGION"
        value = data.aws_region.current.name
    }
  }


  logs_config {
    cloudwatch_logs {
      group_name  = "log-group"
      stream_name = "log-stream"
    }

    s3_logs {
      status   = "ENABLED"
      location = "${aws_s3_bucket.ledger-codebuild.arn}/build-log"
    }
  }


  source {
    type     = "GITHUB"
    location = "https://github.com/AlmeidaFL/Ledger"
    buildspec = "aws/terraform/resources/buildspec.yml"
  }
}