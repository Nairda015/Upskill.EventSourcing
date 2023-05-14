module "systems_manager" {
  count   = local.enable_eventstore || local.enable_aurora ? 1 : 0
  source  = "cloudposse/ssm-parameter-store/aws"
  version = "0.10.0"

  parameter_write = [for param in local.parameters : param if param.enabled]

  tags = { Name = "${local.name-prefix}-systems-manager" }
}

locals {
  parameters = [
    {
      name        = "/Upskill/Databases/EventStore/ConnectionString"
      value       = "esdb://${local.enable_eventstore ? one(module.event-store-db[*].ecs_public_ipv4) : ""}:2113?tls=false",
      type        = "String"
      overwrite   = "true"
      description = "Connection string for database"
      enabled     = local.enable_eventstore
    },
    {
      name        = "/Upskill/Databases/Postgres/ConnectionString"
      value       = "User ID = ${module.aurora.cluster_master_username}; Password = temp; Server = ${module.aurora.cluster_endpoint}; Port = ${module.aurora.cluster_port}; Database = ${module.aurora.cluster_database_name}; Integrated Security = true; Pooling = true;",
      type        = "String"
      overwrite   = "true"
      description = "Connection string for database"
      enabled     = local.enable_aurora
    },
  ]
}

#${module.aurora.cluster_master_password}