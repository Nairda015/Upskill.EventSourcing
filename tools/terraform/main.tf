locals {
  name_prefix = "${var.aws_owner_login}-${var.env_prefix}-${var.app_name}"
}

module "gh_integration" {
  source          = "./modules/gh_integration"
  name_prefix     = local.name_prefix
  app_name        = var.app_name
  aws_owner_login = var.aws_owner_login
  enable_ecr      = var.enable_ecr
  enable_github   = var.enable_github
  env_prefix      = var.env_prefix
  repo_name       = var.repo_name
  repo_owner      = var.repo_owner
}

module "vpc" {
  source  = "terraform-aws-modules/vpc/aws"
  version = "5.1.1"

  name = "${local.name_prefix}-vpc"
  cidr = var.vpc_cidr_block

  azs            = ["${var.region}a", "${var.region}b"]
#  private_subnets = [var.subnet_cidr_block]
  public_subnets = var.subnets_cidr_block

#  enable_ipv6 = true
#  enable_nat_gateway = false
#  single_nat_gateway = false

  public_subnet_tags = { Name = "${local.name_prefix}-subnet" }
  vpc_tags           = { Name = "${local.name_prefix}-vpc" }
}

#databases
module "event_store_db" {
  count              = var.enable_eventstore ? 1 : 0
  source             = "./modules/eventstore"
  avail_zone         = var.avail_zone
  my_ip              = var.my_ip
  name_prefix        = local.name_prefix
  public_key_path    = var.public_key_path
  vpc_id             = module.vpc.vpc_id
  subnet_id          = module.vpc.public_subnets[0]
  ecr_repository_url = module.gh_integration.ecr.repository_url
}

module "aurora" {
  count          = var.enable_aurora ? 1 : 0
  source         = "./modules/aurora"
  database_name  = var.database_name
  name_prefix    = local.name_prefix
  public_subnets = module.vpc.public_subnets
  region         = var.region
  vpc_id         = module.vpc.vpc_id
}

module "open_search" {
  count                                          = var.enable_open_search ? 1 : 0
  source                                         = "cyberlabrs/opensearch/aws"
  name                                           = "basic-os"
  region                                         = "eu-central-1"
  advanced_security_options_enabled              = false
  default_policy_for_fine_grained_access_control = true
}

#pub_sub
module "pub_sub" {
  count           = var.enable_pub_sub ? 1 : 0
  source          = "./modules/pub_sub"
  name_prefix     = local.name_prefix
  aws_owner_login = var.aws_owner_login
}

#compute
module "compute" {
  count  = var.enable_compute ? 1 : 0
  source = "./modules/compute"

  aws_owner_login       = var.aws_owner_login
  ecr_repository_url    = module.gh_integration.ecr.repository_url
  
  enable_command_lambda = var.enable_command_lambda
  enable_projections_lambda = var.enable_command_lambda
  enable_queries_lambda = var.enable_command_lambda
  
  name_prefix           = local.name_prefix
}


# https://github.com/integrations/terraform-provider-github/issues/1827
# Github Actions Secrets
resource "github_actions_secret" "aws_owner" {
  repository      = var.repo_name
  secret_name     = "AWS_OWNER"
  plaintext_value = var.aws_owner_login
}
resource "github_actions_secret" "role_to_assume_arn" {
  repository      = var.repo_name
  secret_name     = "ROLE_TO_ASSUME_ARN"
  plaintext_value = module.gh_integration.github_iam_role_arn
}
resource "github_actions_secret" "ecr_repository_name" {
  repository      = var.repo_name
  secret_name     = "ECR_REPOSITORY_NAME"
  plaintext_value = module.gh_integration.ecr.repository_name
}