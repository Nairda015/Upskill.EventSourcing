module "ecr-commands" {
  source                 = "cloudposse/ecr/aws"
  version                = "0.35.0"
  namespace              = "pgs"
  stage                  = var.env_prefix
  name                   = "${local.name-prefix}-ecr"
  principals_full_access = [aws_iam_user.this.arn]
  principals_lambda      = [aws_iam_user.this.arn]
  force_delete           = true
  image_names            = [
    "${local.name-prefix}-commands",
    "${local.name-prefix}-queries",
    "${local.name-prefix}-subscriber",
    "${local.name-prefix}-projections",
  ]
  tags                   = { "Name" = "${local.name-prefix}-commands" }
}