variable "app_name" { type = string }
variable "env_prefix" { type = string }

variable "aws_owner_login" { type = string }
variable "region" { type = string }
variable "avail_zone" { type = string }
variable "vpc_cidr_block" { type = string }
variable "subnets_cidr_block" { type = list(string) }

variable "my_ip" { type = string }
variable "public_key_path" { type = string }

variable "database_name" { type = string }

variable "github_token" { type = string }
variable "repo_owner" { type = string }
variable "repo_name" { type = string }


variable "enable_github" { type = bool }
variable "enable_ecr" { type = bool }

variable "enable_aurora" {type = bool}
variable "enable_open_search" {type = bool}
variable "enable_eventstore" {type = bool}
variable "enable_pub_sub" {type = bool}

variable "enable_compute" { type = bool }
variable "enable_command_lambda" { type = bool }
variable "enable_queries_lambda" { type = bool }
variable "enable_projections_lambda" { type = bool }
variable "enable_listener_lambda" { type = bool }


