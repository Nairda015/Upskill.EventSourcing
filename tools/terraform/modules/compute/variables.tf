variable "name_prefix" { type = string }
variable "aws_owner_login" { type = string }

variable "ecr_repository_url" { type = string }

variable "enable_command_lambda" { type = bool }
variable "enable_queries_lambda" { type = bool }
variable "enable_projections_lambda" { type = bool }
