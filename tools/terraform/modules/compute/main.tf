#
#
#
#module "ecr-commands" {
#  enabled                = local.enable_ecr
#  source                 = "cloudposse/ecr/aws"
#  version                = "0.35.0"
#  namespace              = "pgs"
#  stage                  = var.env_prefix
#  name                   = "${local.name-prefix}-ecr"
#  principals_full_access = [aws_iam_user.this.arn]
#  principals_lambda      = [aws_iam_user.this.arn]
#  force_delete           = true
#  enable_lifecycle_policy = false
#  image_names            = [
#    "${var.owner_login}-commands",
#    "${var.owner_login}-listener",
#    "${var.owner_login}-projections",
#    "${var.owner_login}-queries",
#  ]
#  tags = { "Name" = "${local.name-prefix}-ecr" }
#}
#
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
#
#
#module "lambda_commands" {
#  create = local.enable_command_lambda
#
#  source = "terraform-aws-modules/lambda/aws"
#  function_name = "${local.name-prefix}-commands"
#  create_package = false
#
#  attach_policy = true
#  policy = aws_iam_policy.policy-command.arn
#
#  image_uri    = module.ecr-commands.repository_arn_map["${var.owner_login}-commands"]
#  package_type = "Image" 
#}
#
#
#
##module "lambda_function" {
##  source = "terraform-aws-modules/lambda/aws"
##
##  function_name = "${local.name-prefix}-commands"
##  handler       = "index.lambda_handler"
##  runtime       = "dotnet6"
##
##  source_path = "../../src/Commands"
##
##  tags = { Name  = "${local.name-prefix}-commands-lambda" }
##}
