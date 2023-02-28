#module "ecr-commands" {
#  source                 = "cloudposse/ecr/aws"
#  version                = "0.35.0"
#  namespace              = "pgs"
#  stage                  = var.env_prefix
#  name                   = "${local.name-prefix}-ecr"
#  principals_full_access = [aws_iam_user.this.arn]
#  principals_lambda      = [aws_iam_user.this.arn]
#  force_delete           = true
#  image_names            = [
#    "${var.owner_login}-commands",
#    "${var.owner_login}-queries",
#    "${var.owner_login}-listener",
##    "${var.owner_login}-projections",
#  ]
#  tags = { "Name" = "${local.name-prefix}-ecr" }
#}


#module "lambda_function" {
#  source = "terraform-aws-modules/lambda/aws"
#
#  function_name = "${local.name-prefix}-commands-lambda"
#  handler       = "index.lambda_handler"
#  runtime       = "dotnet6"
#
#  source_path = "../../src/Commands"
#
#  tags = { Name  = "${local.name-prefix}-commands-lambda" }
#}

#
#module "lambda_function_container_image" {
#  source = "terraform-aws-modules/lambda/aws"
#
#  function_name = "my-lambda-existing-package-local"
#  description   = "My awesome lambda function"
#
#  create_package = false
#
#  image_uri    = "132367819851.dkr.ecr.eu-west-1.amazonaws.com/complete-cow:1.0"
#  package_type = "Image"
#}