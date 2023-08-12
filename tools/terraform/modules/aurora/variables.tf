variable "name_prefix" { type = string }
variable "vpc_id" { type = string }
variable "database_name" { type = string }
variable "region" { type = string }
variable "public_subnets" { type = list(string) }
variable "master_password" { type = string }
variable "my_ip" { type = string }