module "aurora" {
  source              = "terraform-aws-modules/rds-aurora/aws"
  name                = "${var.name_prefix}-aurora"
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

  vpc_id  = var.vpc_id
  subnets = var.public_subnets

  create_security_group = true

  monitoring_interval = 60

  apply_immediately    = true
  skip_final_snapshot  = true
  storage_encrypted    = true
  enable_http_endpoint = true

  tags = { Name = "${var.name_prefix}-aurora" }
}

data "aws_rds_engine_version" "this" {
  engine             = "aurora-postgresql"
  preferred_versions = ["14.6"]
}

module "systems_manager" {
  source  = "cloudposse/ssm-parameter-store/aws"
  version = "0.10.0"

  parameter_write = [{
    name        = "/Upskill/Databases/Postgres/ConnectionString"
    value       = "User ID = ${module.aurora.cluster_master_username}; Password = ${module.aurora.cluster_master_password}; Server = ${module.aurora.cluster_endpoint}; Port = ${module.aurora.cluster_port}; Database = ${module.aurora.cluster_database_name}; Integrated Security = true; Pooling = true;",
    type        = "String"
    overwrite   = "true"
    description = "Connection string for database"
  }]

  tags = { Name = "${var.name_prefix}-systems-manager" }
}