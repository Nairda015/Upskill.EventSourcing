variable "owner_login" { type = string }
variable "app_name" { type = string }
variable "env_prefix" { type = string }
variable "region" { type = string }
variable "avail_zone" { type = string }
variable "vpc_cidr_block" { type = string }
variable "subnets_cidr_block" { type = list(string) }
variable "my_ip" { type = string }
variable "public_key_path" { type = string }
