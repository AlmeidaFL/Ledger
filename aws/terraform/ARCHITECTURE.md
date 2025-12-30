# Architecture Overview

## CodeBuild vs CodePipeline

### How They Work Together

```
┌─────────────────────────────────────────────────────────────┐
│                    YOUR TERRAFORM SETUP                      │
└─────────────────────────────────────────────────────────────┘

Step 1: Deploy CodeBuild
─────────────────────────────────────────────────────────────
environments/dev/codebuild/
  ├── main.tf
  │   └─> Creates in AWS:
  │       ├─ IAM Role (codebuild-role)
  │       ├─ IAM Policy (full permissions)
  │       └─ CodeBuild Project: "ledger-codebuild"
  │          ├─ Build image: amazonlinux2
  │          ├─ Compute: SMALL
  │          ├─ Source: GitHub (AlmeidaFL/Ledger)
  │          ├─ Buildspec: aws/terraform/resources/buildspec.yml
  │          └─ Privileged mode: true (for Docker)
  │
  └── terraform.tfstate
      └─ Outputs:
         └─ project_name = "ledger-codebuild"


Step 2: Deploy CodePipeline
─────────────────────────────────────────────────────────────
environments/dev/codepipeline/
  ├── main.tf
  │   ├─> Reads: ../codebuild/terraform.tfstate
  │   │   └─ Gets: project_name = "ledger-codebuild"
  │   │
  │   └─> Creates in AWS:
  │       ├─ IAM Role (codepipeline-role)
  │       ├─ S3 Bucket (ledger-codepipeline)
  │       ├─ CodeStar Connection → GitHub
  │       └─ CodePipeline: "ledger-codepipeline"
  │          │
  │          ├─ Stage 1: Source
  │          │   └─ GitHub → main branch
  │          │
  │          └─ Stage 2: Build
  │              └─ References: "ledger-codebuild" ← MUST EXIST!
  │
  └── terraform.tfstate
```

### What Gets Created in AWS

```
AWS Account (sa-east-1)
│
├─ CodeBuild
│  └─ Project: "ledger-codebuild"
│     ├─ Configuration: From environments/dev/codebuild/
│     ├─ IAM Role: codebuild-role
│     └─ Can be triggered: Manually OR by CodePipeline
│
├─ CodePipeline
│  └─ Pipeline: "ledger-codepipeline"
│     ├─ Configuration: From environments/dev/codepipeline/
│     ├─ IAM Role: codepipeline-role
│     ├─ S3 Bucket: ledger-codepipeline (artifacts)
│     ├─ Stage 1: Pull from GitHub
│     └─ Stage 2: Trigger "ledger-codebuild" project
│
└─ CodeStar Connection
   └─ Name: "ledger-connection-luisfab"
      ├─ Type: GitHub
      └─ Status: PENDING (needs manual activation)
```

## Remote State Explanation

### Why `terraform_remote_state`?

**Problem**: CodePipeline needs to know the CodeBuild project name.

**Bad Solution** ❌:
```hcl
# Hardcode in both places
# codebuild/main.tf
project_name = "ledger-codebuild"

# codepipeline/main.tf
codebuild_project_name = "ledger-codebuild"  # Duplicate!
```

**Good Solution** ✅:
```hcl
# codebuild/main.tf
module "codebuild" {
  project_name = "ledger-codebuild"
}

output "project_name" {
  value = module.codebuild.project_name  # Single source of truth
}

# codepipeline/main.tf
data "terraform_remote_state" "codebuild" {
  backend = "local"
  config = {
    path = "../codebuild/terraform.tfstate"  # Read from here
  }
}

module "codepipeline" {
  # Use the output from codebuild's state
  codebuild_project_name = data.terraform_remote_state.codebuild[0].outputs.project_name
}
```

### Benefits

1. **Single Source of Truth**: Project name defined once in codebuild
2. **Automatic Updates**: If you rename the project, codepipeline sees it
3. **Validation**: Ensures codebuild was deployed first
4. **No Duplication**: DRY principle

### Fallback Logic

```hcl
# Check if codebuild state exists
count = fileexists("../codebuild/terraform.tfstate") ? 1 : 0

# Use remote state if exists, otherwise use default
codebuild_project_name = length(data.terraform_remote_state.codebuild) > 0
  ? data.terraform_remote_state.codebuild[0].outputs.project_name  # From state
  : var.codebuild_project_name  # Default: "ledger-codebuild"
```

**Why?** Allows testing codepipeline independently without deploying codebuild first.

## Deployment Order

### Option 1: Global (Orchestrator)
```bash
cd environments/dev
terraform apply

# Creates in order:
# 1. Network (VPC)
# 2. EKS (depends on Network)
# 3. EBS CSI, Load Balancer, ArgoCD, Kyverno (depend on EKS)
# 4. CodeBuild (independent)
# 5. CodePipeline (depends on CodeBuild)
```

### Option 2: Isolated (Manual Order)
```bash
# Independent - can deploy anytime
cd environments/dev/codebuild
terraform apply

# Depends on codebuild
cd environments/dev/codepipeline
terraform apply  # Will read codebuild state
```

## Runtime Flow (After Deployment)

```
┌─────────────────────────────────────────────────────────────┐
│                    ACTUAL EXECUTION                          │
└─────────────────────────────────────────────────────────────┘

Developer pushes code to GitHub main branch
             ↓
CodeStar Connection detects change
             ↓
CodePipeline: Stage 1 (Source)
  - Pulls code from GitHub
  - Stores in S3 bucket: ledger-codepipeline
             ↓
CodePipeline: Stage 2 (Build)
  - Triggers: ledger-codebuild project
  - Passes: Source code artifact
             ↓
CodeBuild: ledger-codebuild
  - Uses: Configuration from Terraform
  - Runs: aws/terraform/resources/buildspec.yml
  - Builds: Docker images
  - Pushes: Images to ECR
             ↓
ArgoCD (running in EKS)
  - Detects: New images in ECR
  - Updates: Kubernetes deployments
  - Deploys: To EKS cluster
```

## Key Takeaways

| Aspect | CodeBuild | CodePipeline |
|--------|-----------|--------------|
| **What it does** | Builds Docker images | Orchestrates the flow |
| **Triggered by** | Manual or CodePipeline | GitHub pushes |
| **Configuration** | Your Terraform | Your Terraform |
| **Relationship** | Independent project | References CodeBuild |
| **AWS creates** | Nothing extra | Nothing extra |
| **State** | Own state file | Own state file |

**IMPORTANT**:
- ✅ Both use YOUR Terraform configurations
- ✅ AWS does NOT create extra resources
- ✅ CodePipeline only REFERENCES CodeBuild by name
- ✅ CodeBuild must exist before CodePipeline can use it
