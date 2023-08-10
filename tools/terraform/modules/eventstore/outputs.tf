output "ecs_private_ipv4" {
  value = data.aws_network_interface.this.private_ip
}

output "ecs_public_ipv4" {
  value = data.aws_network_interface.this.association[0].public_ip
}