# Ledger AWS Infrastructure

Modular Terraform configuration for Ledger application infrastructure on AWS.

## Quick Start

```bash
# Deploy everything
cd environments/dev
terraform init
terraform plan
terraform apply

# Or deploy individual components
cd environments/dev/network
terraform init && terraform apply
```

## Structure

```
terraform/
├── modules/                    # Reusable Terraform modules
│   ├── network/               # VPC, subnets, NAT gateway
│   ├── eks/                   # EKS cluster
│   ├── ebs-csi/              # EBS CSI driver (persistent volumes)
│   ├── loadbalancer/         # AWS Load Balancer Controller
│   ├── argocd/               # ArgoCD GitOps
│   ├── kyverno/              # Kyverno Policy Engine
│   ├── codebuild/            # CodeBuild CI
│   └── codepipeline/         # CodePipeline CD
│
├── environments/              # Environment-specific configurations
│   └── dev/                  # Development environment
│       ├── main.tf           # Orchestrator (deploys all components)
│       ├── network/          # Network deployment
│       ├── eks/              # EKS deployment
│       ├── ebs-csi/         # EBS CSI deployment
│       ├── loadbalancer/    # Load Balancer deployment
│       ├── argocd/          # ArgoCD deployment
│       ├── kyverno/         # Kyverno deployment
│       ├── codebuild/       # CodeBuild deployment
│       ├── codepipeline/    # CodePipeline deployment
│       └── README.md        # Detailed usage guide
│
├── resources/                 # Build resources
│   ├── buildspec.yml         # CodeBuild build specification
│   └── build-and-push.sh     # Docker build script
│
└── old_architecture/         # Backup of old monolithic structure
    └── README.md             # Migration notes

```

## Features

### ✅ Dual Execution Modes

**Global Execution** - Deploy everything at once:
```bash
cd environments/dev
terraform apply
```

**Isolated Execution** - Deploy individual components:
```bash
cd environments/dev/codebuild
terraform apply
```

### ✅ Independent State Management

Each component has its own `terraform.tfstate` for:
- Faster deployments
- Reduced blast radius
- Easier troubleshooting
- Component-level rollbacks

### ✅ Smart Dependencies

Components automatically reference each other via `terraform_remote_state`:
- EKS references Network outputs (VPC ID, subnets)
- CodePipeline references CodeBuild outputs (project name)
- Load Balancer references both Network and EKS

### ✅ Production-Ready Features

- **GitOps**: ArgoCD for continuous deployment
- **CI/CD**: CodeBuild + CodePipeline for automated builds
- **Policy Enforcement**: Kyverno for image mutation and validation
- **Load Balancing**: AWS Load Balancer Controller for ALB/NLB
- **Storage**: EBS CSI driver for persistent volumes

## Documentation

- **Complete Guide**: [environments/dev/README.md](environments/dev/README.md)
- **Dependencies**: Network → EKS → (EBS CSI, Load Balancer, ArgoCD, Kyverno)
- **Independent**: CodeBuild, CodePipeline (no EKS dependency)

## Components

| Component | Description | Resources | Dependencies |
|-----------|-------------|-----------|--------------|
| **Network** | VPC, subnets, NAT | 23 | None |
| **EKS** | Kubernetes cluster | ~30 | Network |
| **EBS CSI** | Persistent volumes | 3 | EKS |
| **Load Balancer** | ALB/NLB controller | 6 | EKS, Network |
| **ArgoCD** | GitOps CD | 2 | EKS |
| **Kyverno** | Policy engine | 2 | EKS |
| **CodeBuild** | Docker builds | 3 | None |
| **CodePipeline** | CI/CD pipeline | 5 | CodeBuild |
| **Total** | **All components** | **65** | - |

## Post-Deployment

### CodeStar Connection Activation

After deploying CodePipeline:
1. Go to AWS Console → Developer Tools → Connections
2. Find "ledger-connection-luisfab"
3. Click "Update pending connection"
4. Authorize GitHub access

### ArgoCD Access

```bash
# Get ArgoCD initial password
kubectl -n argocd get secret argocd-initial-admin-secret -o jsonpath="{.data.password}" | base64 -d

# Port forward to access UI
kubectl port-forward svc/argocd-server -n argocd 8080:443
```

## Cleanup

```bash
# Destroy everything
cd environments/dev
terraform destroy

# Or destroy individually (reverse order)
cd environments/dev
for dir in codepipeline codebuild kyverno argocd loadbalancer ebs-csi eks network; do
  cd $dir && terraform destroy -auto-approve && cd ..
done
```

## Migration Notes

This is a **modular refactoring** of the original monolithic structure. All original files are backed up in `old_architecture/` for reference.

No resources need to be destroyed - the modules contain the same configuration, just better organized.

## Region

All resources are deployed in **sa-east-1** (São Paulo, Brazil).

## Support

For issues or questions, see:
- [environments/dev/README.md](environments/dev/README.md) - Detailed deployment guide
