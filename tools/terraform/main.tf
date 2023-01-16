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
// dynamodb table for state
// aurora for categories
// elastic for products
// sns
// sqs x2 with lambda handler
// query lambda
// command lambda
// api gateway
// cloudwatch

module "network" {
  source            = "./modules/network"
  avail_zone        = var.avail_zone
  name_prefix       = local.name-prefix
  subnet_cidr_block = var.subnet_cidr_block
  vpc_cidr_block    = var.vpc_cidr_block
}

module "event-store-db" {
  source          = "./modules/eventstore"
  avail_zone      = var.avail_zone
  my_ip           = var.my_ip
  name_prefix     = local.name-prefix
  public_key_path = var.public_key_path
  subnet_id       = module.network.subnet.id
  vpc_id          = module.network.vpc.id
}