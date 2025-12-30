# Global variables for the dev environment
# Can be customized for different environments (prod, staging, etc.)

locals {
  ledger_cluster_name = "ledger_cluster_v2"
  ledger_repo         = "https://github.com/AlmeidaFL/Ledger.git"
  region              = "sa-east-1"
}
