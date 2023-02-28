locals {
  name-prefix       = "${var.owner_login}-${var.env_prefix}-${var.app_name}"
  enable_aurora     = false
  enable_eventstore = true
  enable_pub_sub    = true
  enable_ecr        = true
  enable_command_lambda = true
}

resource "aws_iam_user" "this" {
  name = var.owner_login
}

data "aws_iam_group" "this" {
  group_name = "AdvancedLearning"
}

resource "aws_iam_user_group_membership" "this" {
  user = aws_iam_user.this.name

  groups = [
    data.aws_iam_group.this.group_name
  ]
}

resource "aws_iam_access_key" "this" {
  user = aws_iam_user.this.name
}

data "github_actions_public_key" "this" {
  repository = "Nairda015/Upskill.EventSourcing"
}

resource "github_actions_secret" "access_key" {
  repository       = "Nairda015/Upskill.EventSourcing"
  secret_name      = "AWS_ACCESS_KEY_ID"
  plaintext_value  = aws_iam_access_key.this.id
}

resource "github_actions_secret" "secret_access_key" {
  repository       = "Nairda015/Upskill.EventSourcing"
  secret_name      = "AWS_SECRET_ACCESS_KEY"
  plaintext_value  = aws_iam_access_key.this.secret
}

output "secret_access_key" {
  value = aws_iam_access_key.this.secret
}

output "access_key" {
  value = aws_iam_access_key.this.id
}

