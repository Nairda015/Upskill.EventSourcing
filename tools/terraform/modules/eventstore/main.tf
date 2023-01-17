locals {
  lunch-type = "FARGATE"
}

resource "aws_security_group" "this" {
  name   = "${var.name_prefix}-sg"
  vpc_id = var.vpc_id
  ingress {
    from_port   = 2113
    to_port     = 2113
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }
  egress {
    from_port       = 0
    to_port         = 0
    protocol        = "-1"
    cidr_blocks     = ["0.0.0.0/0"]
    prefix_list_ids = []
  }
  tags = { Name = "${var.name_prefix}-security-group" }
}

resource "aws_key_pair" "this" {
  key_name   = "${var.name_prefix}-server-key"
  public_key = file(var.public_key_path)
  tags = { Name = "${var.name_prefix}-key_pair" }
}

resource "aws_ecs_cluster_capacity_providers" "this" {
  cluster_name = aws_ecs_cluster.this.name
  capacity_providers = [local.lunch-type]

  default_capacity_provider_strategy {
    base              = 1
    weight            = 100
    capacity_provider = local.lunch-type
  }
}

resource "aws_ecs_cluster" "this" {
  name = "${var.name_prefix}-ecs_cluster"
  setting {
    name  = "containerInsights"
    value = "enabled"
  }
  tags = { Name = "${var.name_prefix}-ecs_cluster" }
}

resource "aws_ecs_service" "this" {
  name                               = "${var.name_prefix}-service"
  cluster                            = aws_ecs_cluster.this.arn
  deployment_maximum_percent         = 200
  deployment_minimum_healthy_percent = 0
  desired_count                      = 1
  launch_type                        = local.lunch-type
  task_definition                    = "${aws_ecs_task_definition.this.family}:${aws_ecs_task_definition.this.revision}"
  network_configuration {
    assign_public_ip = true
    security_groups  = [aws_security_group.this.id]
    subnets          = [var.subnet_id]
  }
  tags = { Name = "${var.name_prefix}-service" }
}

resource "aws_ecs_task_definition" "this" {
  container_definitions    = jsonencode([module.ecs-container-definition.json_map_object])
  family                   = "${var.name_prefix}-task-definition"
  requires_compatibilities = [local.lunch-type]
  cpu                      = "512"
  memory                   = "1024"
  network_mode             = "awsvpc"
  tags = { Name = "${var.name_prefix}-task-definition" }
}

module "ecs-container-definition" {
  source          = "cloudposse/ecs-container-definition/aws"
  version         = "0.58.1"
  container_image = "docker.io/eventstore/eventstore:latest"
  container_name  = "event-store-db"
  environment = [
    {
      "name": "EVENTSTORE_INSECURE",
      "value": "true"
    }
  ]
  container_memory = 1024
  container_cpu = 512

  port_mappings = [
    {
      containerPort = 1113
      hostPort      = 1113
      protocol       = "tcp"
    },
    {
      containerPort = 2113
      hostPort      = 2113
      protocol       = "tcp"
    }
  ]
}