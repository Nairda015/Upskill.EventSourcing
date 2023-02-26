#resource "aws_iam_user" "this" {
#  name = var.owner_login
#}
#
#data "aws_iam_group" "this" {
#  group_name = "AdvancedLearning"
#}
#
#resource "aws_iam_user_group_membership" "this" {
#  user = aws_iam_user.this.name
#
#  groups = [
#    data.aws_iam_group.this.group_name
#  ]
#}
#
#
