module "event-store-db" {
  count           = local.enable_eventstore ? 1 : 0
  source          = "./modules/eventstore"
  avail_zone      = var.avail_zone
  my_ip           = var.my_ip
  name_prefix     = local.name-prefix
  public_key_path = var.public_key_path
  vpc_id          = module.vpc.vpc_id
  subnet_id       = module.vpc.public_subnets[0]
}

#module "opensearch" {
#  
#  source                                         = "cyberlabrs/opensearch/aws"
#  name                                           = "basic-os"
#  region                                         = "eu-central-1"
#  advanced_security_options_enabled              = true
#  default_policy_for_fine_grained_access_control = true
#  
#}
# outputs   custom_endpoint*

data "aws_rds_engine_version" "this" {
  engine             = "aurora-postgresql"
  preferred_versions = ["14.6"]
}

module "aurora" {
  create_cluster      = local.enable_aurora
  source              = "terraform-aws-modules/rds-aurora/aws"
  name                = "${local.name-prefix}-aurora"
  engine              = "aurora-postgresql"
  engine_version      = data.aws_rds_engine_version.this.version
  engine_mode         = "serverless"
  availability_zones  = ["${var.region}a", "${var.region}b"]
  deletion_protection = false
  database_name       = var.database_name

  serverlessv2_scaling_configuration = {
    min_capacity = 2
    max_capacity = 10
  }

  vpc_id  = module.vpc.vpc_id
  subnets = module.vpc.public_subnets

  create_security_group = true
  allowed_cidr_blocks   = module.vpc.private_subnets_cidr_blocks

  monitoring_interval = 60

  apply_immediately    = true
  skip_final_snapshot  = true
  storage_encrypted    = true
  enable_http_endpoint = true

  tags = { Name = "${local.name-prefix}-aurora" }
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