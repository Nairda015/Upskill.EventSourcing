## IAM Role to be granted ECR permissions
#data "aws_iam_role" "ecr" {
#  name = "ecr"
#}
#
#module "ecr-commands" {
#  source                 = "cloudposse/ecr/aws"
#  version                = "0.35.0"
#  namespace              = "pgs"
#  stage                  = var.env_prefix
#  name                   = "${local.name-prefix}-comands"
#  principals_full_access = [data.aws_iam_role.ecr.arn]
#  tags                   = { "Name" = "${local.name-prefix}-commands" }
#}
#
#module "ecr-queries" {
#  source                 = "cloudposse/ecr/aws"
#  version                = "0.35.0"
#  namespace              = "pgs"
#  stage                  = var.env_prefix
#  name                   = "${local.name-prefix}-queries"
#  principals_full_access = [data.aws_iam_role.ecr.arn]
#  tags                   = { "Name" = "${local.name-prefix}-queries" }
#}
#
#module "ecr-event-store-subscriber" {
#  source                 = "cloudposse/ecr/aws"
#  version                = "0.35.0"
#  namespace              = "pgs"
#  stage                  = var.env_prefix
#  name                   = "${local.name-prefix}-event-store-subscriber"
#  principals_full_access = [data.aws_iam_role.ecr.arn]
#  tags                   = { "Name" = "${local.name-prefix}-event-store-subscriber" }
#}
#
#module "ecr-projection-open-search" {
#  source                 = "cloudposse/ecr/aws"
#  version                = "0.35.0"
#  namespace              = "pgs"
#  stage                  = var.env_prefix
#  name                   = "${local.name-prefix}-projections-open-search"
#  principals_full_access = [data.aws_iam_role.ecr.arn]
#  tags                   = { "Name" = "${local.name-prefix}-projection-open-search" }
#}
#
#module "ecr-projection-aurora" {
#  source                 = "cloudposse/ecr/aws"
#  version                = "0.35.0"
#  namespace              = "pgs"
#  stage                  = var.env_prefix
#  name                   = "${local.name-prefix}-projections-aurora"
#  principals_full_access = [data.aws_iam_role.ecr.arn]
#  tags                   = { "Name" = "${local.name-prefix}-projections-aurora" }
#}