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
  name-prefix = "${var.owner_login}-${var.env_prefix}-${var.app_name}"
}

// for fargate:
// vpc subnet security group with ingress rules for lambda
//
// dynamodb table for state
// aurora for categories
// elastic for products
// sns
// sqs x2 with lambda handler
// query lambda
// command lambda
// api gateway
// cloudwatch

module "event-store-db" {
  source          = "./modules/eventstore"
  avail_zone      = var.avail_zone
  my_ip           = var.my_ip
  name_prefix     = local.name-prefix
  public_key_path = var.public_key_path
  vpc_id          = module.vpc.vpc_id
  subnet_id       = module.vpc.public_subnets[0]
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

module "lambda_function" {
  source = "terraform-aws-modules/lambda/aws"

  function_name = "${local.name-prefix}-commands-lambda"
  handler       = "index.lambda_handler"
  runtime       = "dotnet6"

  source_path = "../../src/Commands"

  tags = { Name  = "${local.name-prefix}-commands-lambda" }
}

#
#module "lambda_function_container_image" {
#  source = "terraform-aws-modules/lambda/aws"
#
#  function_name = "my-lambda-existing-package-local"
#  description   = "My awesome lambda function"
#
#  create_package = false
#
#  image_uri    = "132367819851.dkr.ecr.eu-west-1.amazonaws.com/complete-cow:1.0"
#  package_type = "Image"
#}