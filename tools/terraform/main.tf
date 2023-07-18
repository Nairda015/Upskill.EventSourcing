locals {
  name-prefix           = "${var.owner_login}-${var.env_prefix}-${var.app_name}"
  repo_name             = "Upskill.EventSourcing"
  repo_owner            = "Nairda015"
  enable_aurora         = false
  enable_open_search    = false
  enable_eventstore     = true
  enable_pub_sub        = true
  enable_ecr            = true
  enable_command_lambda = false
}

#resource "aws_iam_user" "this" {
#  name = var.owner_login
#}
#
#data "aws_iam_group" "this" {
#  group_name = "AdvancedLearning"
#}
#
#resource "aws_iam_user_group_membership" "this" {
#  user = aws_iam_user.this.name
#
#  groups = [
#    data.aws_iam_group.this.group_name
#  ]
#}
#
#resource "aws_iam_access_key" "this" {
#  user = aws_iam_user.this.name
#}

module "vpc" {
  source  = "terraform-aws-modules/vpc/aws"
  version = "3.19.0"

  name = "${local.name-prefix}-vpc"
  cidr = var.vpc_cidr_block

  azs            = ["${var.region}a", "${var.region}b"]
  #private_subnets = [var.subnet_cidr_block]
  public_subnets = var.subnets_cidr_block

  enable_ipv6 = true

  enable_nat_gateway = false
  single_nat_gateway = true

  public_subnet_tags = { Name = "${local.name-prefix}-subnet" }
  vpc_tags           = { Name = "${local.name-prefix}-vpc" }
}

#databases
module "event_store_db" {
  count           = local.enable_eventstore ? 1 : 0
  source          = "./modules/eventstore"
  avail_zone      = var.avail_zone
  my_ip           = var.my_ip
  name_prefix     = local.name-prefix
  public_key_path = var.public_key_path
  vpc_id          = module.vpc.vpc_id
  subnet_id       = module.vpc.public_subnets[0]
}

module "aurora" {
  count          = local.enable_aurora ? 1 : 0
  source         = "./modules/aurora"
  database_name  = var.database_name
  name_prefix    = local.name-prefix
  public_subnets = module.vpc.public_subnets
  region         = var.region
  vpc_id         = module.vpc.vpc_id
}

module "open_search" {
  count                                          = local.enable_open_search ? 1 : 0
  source                                         = "cyberlabrs/opensearch/aws"
  name                                           = "basic-os"
  region                                         = "eu-central-1"
  advanced_security_options_enabled              = true
  default_policy_for_fine_grained_access_control = true
}

#pub_sub
module "pub_sub" {
  count       = local.enable_pub_sub ? 1 : 0
  source      = "./modules/pub_sub"
  name_prefix = local.name-prefix
  owner_login = var.owner_login
}

