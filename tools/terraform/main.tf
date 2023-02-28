provider "aws" {
  region = var.region
  default_tags {
    tags = {
      Environment = var.env_prefix
      Name        = local.name-prefix
      Owner       = var.owner_login
      ManagedBy   = "terraform"
    }
  }
}

locals {
  name-prefix       = "${var.owner_login}-${var.env_prefix}-${var.app_name}"
  enable_aurora     = false
  enable_eventstore = true
  enable_pub_sub    = true
}

// for fargate:
// vpc subnet security group with ingress rules for lambda
//
// dynamodb table for state
// aurora for categories
// elastic for products
// sns
// sqs
// query lambda
// command lambda
// api gateway
// cloudwatch

resource "aws_iam_user" "this" {
  name = var.owner_login
}

data "aws_iam_group" "this" {
  group_name = "AdvancedLearning"
}

resource "aws_iam_user_group_membership" "this" {
  user = aws_iam_user.this.name

  groups = [
    data.aws_iam_group.this.group_name
  ]
}

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

