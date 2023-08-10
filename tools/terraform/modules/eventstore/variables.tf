variable "name_prefix" { type = string }
variable "avail_zone" { type = string }
variable "my_ip" { type = string }
variable "vpc_id" { type = string }
variable "subnet_id" { type = string }
variable "public_key_path" { type = string }
variable "ecr_repository_url" { type = string }
variable "enable_listener_lambda" { type = bool }