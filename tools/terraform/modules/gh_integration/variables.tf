variable "app_name" { type = string }
variable "env_prefix" { type = string }
variable "name_prefix" { type = string }

variable "aws_owner_login" { type = string }

variable "enable_github" { type = bool }
variable "enable_ecr" { type = bool }

variable "repo_owner" { type = string }
variable "repo_name" { type = string }