#common
locals { lunch-type = "FARGATE" }
resource "aws_security_group" "this" {
  name   = "${var.name_prefix}-eventstore-sg"
  vpc_id = var.vpc_id
  ingress {
    from_port   = 2113
    to_port     = 2113
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }
  ingress {
    from_port   = 80
    to_port     = 80
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
  tags       = { Name = "${var.name_prefix}-key_pair" }
}
data "aws_network_interfaces" "this" {
  depends_on = [aws_ecs_service.eventstore_ecs_service]

  filter {
    name   = "group-id"
    values = [aws_security_group.this.id]
  }
  filter {
    name   = "tag:Name"
    values = ["${var.name_prefix}-eventstore-service"]
  }
}
data "aws_network_interface" "this" {
  id = data.aws_network_interfaces.this.ids[0]
}
module "systems_manager" {
  source  = "cloudposse/ssm-parameter-store/aws"
  version = "0.10.0"

  parameter_write = [
    {
      name        = "/Upskill/Databases/EventStore/ConnectionString"
      value       = "esdb://${data.aws_network_interface.this.association[0].public_ip}:2113?tls=false",
      type        = "SecureString"
      overwrite   = "true"
      description = "Connection string for database"
    }
  ]

  tags = { Name = "${var.name_prefix}-systems-manager" }
}


#cluster
resource "aws_ecs_cluster_capacity_providers" "this" {
  cluster_name       = aws_ecs_cluster.this.name
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


#eventstore
resource "aws_ecs_service" "eventstore_ecs_service" {
  name                               = "${var.name_prefix}-eventstore-service"
  cluster                            = aws_ecs_cluster.this.arn
  deployment_maximum_percent         = 200
  deployment_minimum_healthy_percent = 0
  desired_count                      = 1
  launch_type                        = local.lunch-type
  task_definition                    = "${aws_ecs_task_definition.eventstore_ecs_task_definition.family}:${aws_ecs_task_definition.eventstore_ecs_task_definition.revision}"
  wait_for_steady_state              = true
  network_configuration {
    assign_public_ip = true
    security_groups  = [aws_security_group.this.id]
    subnets          = [var.subnet_id]
  }
  propagate_tags = "SERVICE"
  tags           = { Name = "${var.name_prefix}-eventstore-service" }
}
resource "aws_ecs_task_definition" "eventstore_ecs_task_definition" {
  container_definitions    = jsonencode([module.eventstore-container-definition.json_map_object])
  family                   = "${var.name_prefix}-eventstore-task-definition"
  requires_compatibilities = [local.lunch-type]
  cpu                      = "512"
  memory                   = "1024"
  network_mode             = "awsvpc"
  tags                     = { Name = "${var.name_prefix}-task-definition" }
}
module "eventstore-container-definition" {
  source          = "cloudposse/ecs-container-definition/aws"
  version         = "0.58.1"
  container_image = "docker.io/eventstore/eventstore:latest"
  container_name  = "event-store-db"
  environment     = [
    {
      "name" : "EVENTSTORE_INSECURE",
      "value" : "true"
    },
    {
      "name" : "EVENTSTORE_RUN_PROJECTIONS",
      "value" : "All"
    },
    {
      "name" : "EVENTSTORE_CLUSTER_SIZE",
      "value" : "1"
    },
    {
      "name" : "EVENTSTORE_START_STANDARD_PROJECTIONS",
      "value" : "true"
    },
    {
      "name" : "EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP",
      "value" : "true"
    }
  ]
  container_memory = 1024
  container_cpu    = 512

  port_mappings = [
    {
      containerPort = 1113
      hostPort      = 1113
      protocol      = "tcp"
    },
    {
      containerPort = 2113
      hostPort      = 2113
      protocol      = "tcp"
    }
  ]
}


#listener
resource "aws_ecs_service" "listener_ecs_service" {
  count                              = var.enable_listener_lambda ? 1 : 0
  name                               = "${var.name_prefix}-listener-service"
  depends_on                         = [module.systems_manager]
  cluster                            = aws_ecs_cluster.this.arn
  deployment_maximum_percent         = 200
  deployment_minimum_healthy_percent = 0
  desired_count                      = 1
  launch_type                        = local.lunch-type
  task_definition                    = "${aws_ecs_task_definition.listener_ecs_task_definition.family}:${aws_ecs_task_definition.listener_ecs_task_definition.revision}"
  wait_for_steady_state              = true
  network_configuration {
    assign_public_ip = true
    security_groups  = [aws_security_group.this.id]
    subnets          = [var.subnet_id]
  }
  propagate_tags = "SERVICE"
  tags           = { Name = "${var.name_prefix}-listener-service" }
}
resource "aws_ecs_task_definition" "listener_ecs_task_definition" {
  container_definitions    = jsonencode([module.listener-container-definition.json_map_object])
  family                   = "${var.name_prefix}-listener-task-definition"
  requires_compatibilities = [local.lunch-type]
  cpu                      = "512"
  memory                   = "1024"
  network_mode             = "awsvpc"
  execution_role_arn       = aws_iam_role.this.arn
  task_role_arn            = aws_iam_role.this.arn
  tags                     = { Name = "${var.name_prefix}-task-definition" }
}
module "listener-container-definition" {
  source           = "cloudposse/ecs-container-definition/aws"
  version          = "0.58.1"
  container_image  = "${var.ecr_repository_url}:Listener-latest"
  container_name   = "listener"
  container_memory = 1024
  container_cpu    = 512
  port_mappings    = [
    {
      containerPort = 80
      hostPort      = 80
      protocol      = "tcp"
    },
    {
      containerPort = 443
      hostPort      = 443
      protocol      = "tcp"
    }
  ]
  log_configuration = {
    logDriver : "awslogs",
    options : {
      "awslogs-create-group" : "true",
      "awslogs-group" : "/aws/lambda/afranczak-listener",
      "awslogs-region" : var.region,
      "awslogs-stream-prefix" : "/aws/lambda/afranczak-listener"
    }
  }
}


#listener-role
resource "aws_iam_role" "this" {
  name               = "listener-role"
  assume_role_policy = jsonencode({
    Version   = "2012-10-17",
    Statement = [
      {
        Action    = "sts:AssumeRole",
        Effect    = "Allow",
        Principal = {
          Service = "ecs-tasks.amazonaws.com"
        }
      }
    ]
  })
}
resource "aws_iam_policy" "this" {
  name        = "${var.name_prefix}-listener-policy"
  description = "Policy for listener"

  policy = jsonencode({
    Version   = "2012-10-17",
    Statement = [
      {
        Action = [
          "ecr:GetDownloadUrlForLayer",
          "ecr:BatchGetImage",
          "ecr:BatchCheckLayerAvailability",
          "ecr:GetAuthorizationToken"
        ],
        Effect   = "Allow",
        Resource = "*"
      },
      {
        Action = [
          "logs:CreateLogStream",
          "logs:PutLogEvents",
          "logs:CreateLogGroup"
        ],
        Effect   = "Allow",
        Resource = "*"
      },
      {
        Action = [
          "sns:Publish",
          "sns:ListTopics",
          "ssm:*",
        ]
        Effect   = "Allow"
        Resource = "*"
      }
    ]
  })
}
resource "aws_iam_role_policy_attachment" "ecs_execution_attachment" {
  policy_arn = aws_iam_policy.this.arn
  role       = aws_iam_role.this.name
}






