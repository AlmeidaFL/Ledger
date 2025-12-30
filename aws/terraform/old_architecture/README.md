# Old Architecture Files (Backup)

This directory contains the original Terraform files from before the modular refactoring.

## Contents

- **argocd.tf** - Original ArgoCD configuration
- **codepipeline.tf** - Original CodePipeline configuration (commented out)
- **ebs.tf** - Original EBS CSI driver configuration
- **ecr.tf** - Original ECR repositories (commented out)
- **eks.tf** - Original EKS cluster configuration
- **kyverno.tf** - Original Kyverno configuration
- **loadbalancer.tf** - Original Load Balancer Controller configuration
- **main.tf** - Original provider configuration
- **variables.tf** - Original variables
- **terraform.tfstate** - Old state file from monolithic structure
- **terraform.tfstate.backup** - Backup of old state

## New Structure

All of these have been refactored into:
- `modules/` - Reusable components
- `environments/dev/` - Deployment configurations with isolated states

## Can I Delete This?

**Keep this directory for reference** until you've successfully deployed the new modular structure and verified everything works.

Once you've confirmed the new structure works, you can safely delete this directory.

## Old codebuild/ Directory

The `../codebuild/` directory also contains old files:
- `codebuild.tf` - Now located at `modules/codebuild/` and `environments/dev/codebuild/`
- State files - Now each component has its own state

This can also be removed after verification.
