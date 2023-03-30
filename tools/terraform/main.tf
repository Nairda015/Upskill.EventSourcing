locals {
  name-prefix       = "${var.owner_login}-${var.env_prefix}-${var.app_name}"
  enable_aurora     = false
  enable_eventstore = true
  enable_pub_sub    = true
  enable_ecr        = false
  enable_command_lambda = false
  repo_name         = "Upskill.EventSourcing"
  repo_owner        = "Nairda015"
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

