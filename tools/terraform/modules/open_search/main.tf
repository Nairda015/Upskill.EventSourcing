module "open_search" {
  source  = "cyberlabrs/opensearch/aws"
  version = "0.0.20"
  name    = "${var.aws_owner_login}-open-search"
  region  = var.region

  master_password                = var.open_search_master_password
  master_user_name               = var.open_search_master_user
  internal_user_database_enabled = true

  default_policy_for_fine_grained_access_control = true
  advanced_security_options_enabled              = true
  create_linked_role                             = false

  allowed_cidrs  = [var.my_ip]
  engine_version = "OpenSearch_2.7"
  volume_type    = "gp3"
  instance_type  = "t3.medium.search"

  node_to_node_encryption = true
  encrypt_at_rest         = {
    enabled = true
  }

  tags = { Name = "${var.name_prefix}-open-search" }
}

module "systems_manager" {
  source  = "cloudposse/ssm-parameter-store/aws"
  version = "0.10.0"

  parameter_write = [
    {
      name        = "/Upskill/Databases/OpenSearch/Endpoint"
      value       = module.open_search.endpoint,
      type        = "SecureString"
      overwrite   = "true"
      description = "Endpoint for OpenSearch node"
    },
    {
      name        = "/Upskill/Databases/OpenSearch/Password"
      value       = var.open_search_master_password,
      type        = "SecureString"
      overwrite   = "true"
      description = "Password for OpenSearch node"
    },
    {
      name        = "/Upskill/Databases/OpenSearch/Username"
      value       = var.open_search_master_user,
      type        = "SecureString"
      overwrite   = "true"
      description = "Username for OpenSearch node"
    }
  ]

  tags = { Name = "${var.name_prefix}-systems-manager" }
}
