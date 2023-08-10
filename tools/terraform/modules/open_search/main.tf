module "open_search" {
  source                                         = "cyberlabrs/opensearch/aws"
  name                                           = "open-search"
  region                                         = var.region
  advanced_security_options_enabled              = false
  default_policy_for_fine_grained_access_control = true
  
  tags = { Name = "${var.name_prefix}-open-search" }
}

module "systems_manager" {
  source  = "cloudposse/ssm-parameter-store/aws"
  version = "0.10.0"

  parameter_write = [{
    name        = "/Upskill/Databases/OpenSearch/Endpoint"
    value       = module.open_search.endpoint,
    type        = "String"
    overwrite   = "true"
    description = "Endpoint for OpenSearch node"
  }]

  tags = { Name = "${var.name_prefix}-systems-manager" }
}
