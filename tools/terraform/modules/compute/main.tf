module "lambda_commands" {
  create  = var.enable_command_lambda
  source  = "terraform-aws-modules/lambda/aws"
  version = "5.3.0"

  function_name  = "${var.aws_owner_login}-commands"
  create_package = false
  image_uri      = "${var.ecr_repository_url}:Commands-latest"
  package_type   = "Image"
  memory_size    = 256

  //attach_policy = true
  //policy = aws_iam_policy.policy-command.arn

  create_lambda_function_url = true
  authorization_type         = "NONE"

  tags = { "Name" = "${var.name_prefix}-commands" }
}

module "lambda_queries" {
  create  = var.enable_queries_lambda
  source  = "terraform-aws-modules/lambda/aws"
  version = "5.3.0"

  function_name  = "${var.aws_owner_login}-queries"
  create_package = false
  image_uri      = "${var.ecr_repository_url}:Queries-latest"
  package_type   = "Image"
  memory_size    = 256

  create_lambda_function_url = true
  authorization_type         = "NONE"

  tags = { "Name" = "${var.name_prefix}-queries" }
}

module "lambda_projections" {
  create  = var.enable_projections_lambda
  source  = "terraform-aws-modules/lambda/aws"
  version = "5.3.0"

  function_name  = "${var.aws_owner_login}-projections"
  create_package = false
  image_uri      = "${var.ecr_repository_url}:Projections-latest"
  package_type   = "Image"
  memory_size    = 256

  create_lambda_function_url = true
  authorization_type         = "NONE"

  tags = { "Name" = "${var.name_prefix}-projections" }
}


#resource "aws_iam_policy" "policy-command" {
#  name        = "${local.name-prefix}-command-policy"
#  path        = "/"
#
#  # Terraform's "jsonencode" function converts a
#  # Terraform expression result to valid JSON syntax.
#  policy = jsonencode({
#    Version = "2012-10-17"
#    Statement = [
#      {
#        Action = [
#          "rds:*",
#          "ssm:*",
#        ]
#        Effect   = "Allow"
#        Resource = "*"
#      },
#    ]
#  })
#}
#
#resource "aws_iam_policy" "policy-queries" {
#  name        = "${local.name-prefix}-queries-policy"
#  path        = "/"
#
#  # Terraform's "jsonencode" function converts a
#  # Terraform expression result to valid JSON syntax.
#  policy = jsonencode({
#    Version = "2012-10-17"
#    Statement = [
#      {
#        Action = [
#          "rds:*",
#          "aoss:*",
#          "ssm:*",
#        ]
#        Effect   = "Allow"
#        Resource = "*"
#      },
#    ]
#  })
#}
#
#resource "aws_iam_policy" "policy-listener" {
#  name        = "${local.name-prefix}-query-policy"
#  path        = "/"
#
#  # Terraform's "jsonencode" function converts a
#  # Terraform expression result to valid JSON syntax.
#  policy = jsonencode({
#    Version = "2012-10-17"
#    Statement = [
#      {
#        Action = [
#          "sns:Publish",
#          "ssm:*",
#        ]
#        Effect   = "Allow"
#        Resource = "*"
#      }
#    ]
#  })
#}
#
#resource "aws_iam_policy" "policy-projections" {
#  name        = "${local.name-prefix}-projections-policy"
#  path        = "/"
#
#  # Terraform's "jsonencode" function converts a
#  # Terraform expression result to valid JSON syntax.
#  policy = jsonencode({
#    Version = "2012-10-17"
#    Statement = [
#      {
#        Action = [
#          "sqs:*",
#          "aoss:*",
#          "ssm:*",
#        ]
#        Effect   = "Allow"
#        Resource = "*"
#      }
#    ]
#  })
#}
