## commented because of long provision time

#data "aws_rds_engine_version" "this" {
#  engine             = "aurora-postgresql"
#  preferred_versions = ["14.6"]
#}
#
#module "aurora" {
#  source              = "terraform-aws-modules/rds-aurora/aws"
#  name                = "${local.name-prefix}-aurora"
#  engine              = "aurora-postgresql"
#  engine_version      = data.aws_rds_engine_version.this.version
#  engine_mode         = "serverless"
#  availability_zones  = ["${var.region}a", "${var.region}b"]
#  deletion_protection = false
#
#  serverlessv2_scaling_configuration = {
#    min_capacity = 2
#    max_capacity = 10
#  }
#
#  vpc_id  = module.vpc.vpc_id
#  subnets = module.vpc.public_subnets
#
#  create_security_group = true
#  allowed_cidr_blocks   = module.vpc.private_subnets_cidr_blocks
#
#  monitoring_interval = 60
#
#  apply_immediately    = true
#  skip_final_snapshot  = true
#  storage_encrypted    = true
#  enable_http_endpoint = true
#
#  tags = { Name = "${local.name-prefix}-aurora" }
#}