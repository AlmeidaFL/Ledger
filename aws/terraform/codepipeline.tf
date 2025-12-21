# data "aws_iam_policy_document" "assume_role" {
#   statement {
#     effect = "Allow"
# 
#     principals {
#       type        = "Service"
#       identifiers = ["codepipeline.amazonaws.com"]
#     }
# 
#     actions = ["sts:AssumeRole"]
#   }
# }
# 
# resource "aws_iam_role" "codepipeline_role" {
#   name               = "test-role"
#   assume_role_policy = data.aws_iam_policy_document.assume_role.json
# }
# 
# data "aws_iam_policy_document" "codepipeline_policy" {
#   statement {
#     effect = "Allow"
# 
#     actions = ["*"]
# 
#     resources = ["*"]
#   }
# }
# 
# resource "aws_iam_role_policy" "codepipeline_policy" {
#   name   = "codepipeline_policy"
#   role   = aws_iam_role.codepipeline_role.id
#   policy = data.aws_iam_policy_document.codepipeline_policy.json
# }
# 
# 
# resource "aws_s3_bucket" "ledger-codepipeline" {
#   bucket = "ledger-codepipeline"
# }
# 
# 
# resource "aws_codepipeline" "ledger-codepipeline" {
#   name     = "ledger-codepipeline"
#   pipeline_type = "V2"
#   role_arn = aws_iam_role.codepipeline_role.arn
# 
#   artifact_store {
#     location = aws_s3_bucket.ledger-codepipeline.bucket
#     type     = "S3"
#   }
# 
#   trigger {
#     provider_type = "CodeStarSourceConnection"
#     git_configuration {
#       source_action_name = "LedgerSource"
#       push {
#         branches {
#           includes = [ "main" ]
#         }
#         file_paths {
#           excludes = [ "**/k8s/*" ]
#         }
#       }
#     }
#   }
# 
#   stage {
#     name = "LedgerSource"
# 
#     action {
#       name             = "LedgerSource"
#       category         = "Source"
#       owner            = "AWS"
#       provider         = "CodeStarSourceConnection"
#       version          = "1"
#       output_artifacts = ["source_output"]
# 
#       configuration = {
#         ConnectionArn    = aws_codestarconnections_connection.ledger.arn
#         FullRepositoryId = "AlmeidaFl/Ledger"
#         BranchName       = "main"
#         DetectChanges = false
#       }
#     }
#   }
# 
#  stage {
#     name = "Build"
# 
#     action {
#       name             = "Build"
#       category         = "Build"
#       owner            = "AWS"
#       provider         = "CodeBuild"
#       input_artifacts  = ["source_output"]
#       output_artifacts = ["build_output"]
#       version          = "1"
# 
#       configuration = {
#         ProjectName = local.ledger-codebuild-name
#       }
#     }
#   }
# }
# 
# resource "aws_codestarconnections_connection" "ledger" {
#   name          = "ledger-connection-luisfab"
#   provider_type = "GitHub"
# }