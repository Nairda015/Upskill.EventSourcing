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