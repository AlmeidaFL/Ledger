# Ledger AWS Infrastructure - Dev Environment

This directory contains the Terraform configuration for the Ledger development environment.

## Structure

```
environments/dev/
├── main.tf              # ORCHESTRATOR: Deploy all components at once
├── outputs.tf           # All outputs from all modules
├── variables.tf         # Global variables
├── README.md            # This file
├── network/             # VPC deployment (isolated)
├── eks/                 # EKS cluster deployment (isolated)
├── ebs-csi/            # EBS CSI driver (isolated)
├── loadbalancer/       # AWS Load Balancer Controller (isolated)
├── argocd/             # ArgoCD GitOps (isolated)
├── kyverno/            # Kyverno Policy Engine (isolated)
├── codebuild/          # CodeBuild CI/CD (isolated)
└── codepipeline/       # CodePipeline (isolated)
```

## Usage

### Deploy Everything (Global Execution)

```bash
cd environments/dev
terraform init
terraform plan
terraform apply
```

This will deploy all components with proper dependencies in one go.

### Deploy Individual Components (Isolated Execution)

#### 1. Deploy Network (VPC)
```bash
cd environments/dev/network
terraform init
terraform plan
terraform apply
```

#### 2. Deploy EKS Cluster
```bash
cd environments/dev/eks
terraform init
terraform plan
terraform apply
```

#### 3. Deploy EBS CSI Driver
```bash
cd environments/dev/ebs-csi
terraform init
terraform plan
terraform apply
```

#### 4. Deploy Load Balancer Controller
```bash
cd environments/dev/loadbalancer
terraform init
terraform plan
terraform apply
```

#### 5. Deploy ArgoCD
```bash
cd environments/dev/argocd
terraform init
terraform plan
terraform apply
```

#### 6. Deploy Kyverno
```bash
cd environments/dev/kyverno
terraform init
terraform plan
terraform apply
```

#### 7. Deploy CodeBuild (Independent)
```bash
cd environments/dev/codebuild
terraform init
terraform plan
terraform apply
```

#### 8. Deploy CodePipeline
```bash
cd environments/dev/codepipeline
terraform init
terraform plan
terraform apply
```

**Note**: After applying, you must manually activate the CodeStar GitHub connection in the AWS Console.

## Dependencies

- **Network** → No dependencies
- **EKS** → Requires Network
- **EBS CSI** → Requires EKS
- **Load Balancer** → Requires EKS + Network
- **ArgoCD** → Requires EKS
- **Kyverno** → Requires EKS
- **CodeBuild** → No dependencies (independent)
- **CodePipeline** → Requires CodeBuild (can run standalone with default project name)

## Important Notes

1. **State Management**: Each component has its own `terraform.tfstate` file when deployed individually
2. **Order Matters**: When deploying individually, follow the dependency chain (Network → EKS → Others)
3. **Orchestrator**: The root `main.tf` handles dependencies automatically when deploying everything
4. **Remote State**: Individual components use `terraform_remote_state` data sources to reference outputs from other components
5. **CodeStar Connection**: After deploying CodePipeline, you must manually activate the GitHub connection in the AWS Console → Developer Tools → Connections

## Cleanup

To destroy everything:

```bash
# Option 1: Destroy via orchestrator (destroys everything)
cd environments/dev
terraform destroy

# Option 2: Destroy individually (reverse order)
cd codepipeline && terraform destroy
cd ../codebuild && terraform destroy
cd ../kyverno && terraform destroy
cd ../argocd && terraform destroy
cd ../loadbalancer && terraform destroy
cd ../ebs-csi && terraform destroy
cd ../eks && terraform destroy
cd ../network && terraform destroy
```
