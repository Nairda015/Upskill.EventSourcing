module "ecr-commands" {
  enabled                = local.enable_ecr
  source                 = "cloudposse/ecr/aws"
  version                = "0.35.0"
  namespace              = "pgs"
  stage                  = var.env_prefix
  name                   = "${local.name-prefix}-ecr"
  principals_full_access = [aws_iam_user.this.arn]
  principals_lambda      = [aws_iam_user.this.arn]
  force_delete           = true
#  image_names            = [
#    "${var.owner_login}-commands",
##    "${var.owner_login}-listener",
##    "${var.owner_login}-projections",
##    "${var.owner_login}-queries",
#  ]
  tags = { "Name" = "${local.name-prefix}-ecr" }
}

#module "lambda_commands" {
#  create = local.enable_command_lambda
#  
#  source = "terraform-aws-modules/lambda/aws"
#  function_name = "${local.name-prefix}-commands"
#  create_package = false
#
##  attach_policy = true
##  policy = data.aws_iam_policy_document.lambda_commands.json
#  
#  image_uri    = module.ecr-commands.repository_arn_map["${var.owner_login}-commands"]
#  package_type = "Image" 
#}



#module "lambda_function" {
#  source = "terraform-aws-modules/lambda/aws"
#
#  function_name = "${local.name-prefix}-commands"
#  handler       = "index.lambda_handler"
#  runtime       = "dotnet6"
#
#  source_path = "../../src/Commands"
#
#  tags = { Name  = "${local.name-prefix}-commands-lambda" }
#}
