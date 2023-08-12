module "aurora" {
  source                      = "terraform-aws-modules/rds-aurora/aws"
  name                        = "${var.name_prefix}-postgresql"
  engine                      = data.aws_rds_engine_version.this.engine
  engine_version              = data.aws_rds_engine_version.this.version
  engine_mode                 = "provisioned"
  deletion_protection         = false
  database_name               = var.database_name
  master_username             = "root"
  manage_master_user_password = false
  master_password             = var.master_password
  publicly_accessible         = true
  
  vpc_id  = var.vpc_id
  subnets = var.public_subnets

  create_security_group  = false
  vpc_security_group_ids = [aws_security_group.this.id]
  create_db_subnet_group = true

  monitoring_interval = 60

  apply_immediately    = true
  skip_final_snapshot  = true
  storage_encrypted    = true

  serverlessv2_scaling_configuration = {
    min_capacity = 1
    max_capacity = 4
  }

  instance_class = "db.serverless"
  instances      = {
    one = {}
  }

  tags = { Name = "${var.name_prefix}-aurora" }
}

data "aws_rds_engine_version" "this" {
  engine = "aurora-postgresql"

  filter {
    name   = "engine-mode"
    values = ["serverless"]
  }
}

module "systems_manager" {
  source  = "cloudposse/ssm-parameter-store/aws"
  version = "0.10.0"

  parameter_write = [
    {
      name        = "/Upskill/Databases/Postgres/ConnectionString"
      value       = "User ID = ${module.aurora.cluster_master_username}; Password = ${module.aurora.cluster_master_password}; Server = ${module.aurora.cluster_endpoint}; Port = ${module.aurora.cluster_port}; Database = ${module.aurora.cluster_database_name}; Integrated Security = true; Pooling = true;",
      type        = "SecureString"
      overwrite   = "true"
      description = "Connection string for database"
    }
  ]

  tags = { Name = "${var.name_prefix}-systems-manager" }
}

resource "aws_security_group" "this" {
  name   = "${var.name_prefix}-aurora-sg"
  vpc_id = var.vpc_id
  ingress {
    from_port   = 5432
    to_port     = 5432
    protocol    = "tcp"
    cidr_blocks = [var.my_ip]
  }
  egress {
    from_port       = 0
    to_port         = 0
    protocol        = "-1"
    cidr_blocks     = ["0.0.0.0/0"]
    prefix_list_ids = []
  }
  tags = { Name = "${var.name_prefix}-security-group" }
}