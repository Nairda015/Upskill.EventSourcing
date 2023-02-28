#module "sns" {
#  source  = "terraform-aws-modules/sns/aws"
#  version = ">= 5.0"
#
#  name = "${var.owner_login}-topic"
#
#  topic_policy_statements = {
#    sqs = {
#      sid = "SQSSubscribe"
#      actions = [
#        "sns:Subscribe",
#        "sns:Receive",
#      ]
#
#      principals = [{
#        type        = "AWS"
#        identifiers = ["*"]
#      }]
#
#      conditions = [{
#        test     = "StringLike"
#        variable = "sns:Endpoint"
#        values   = [module.sqs.queue_arn]
#      }]
#    }
#  }
#
#  subscriptions = {
#
#    sqs = {
#      raw_message_delivery = true
#      protocol = "sqs"
#      endpoint = module.sqs.queue_arn
#    }
#  }
#
#  tags = { Name  = "${local.name-prefix}-sns" }
#}
#
#
#
#module "sqs" {
#  source = "terraform-aws-modules/sqs/aws"
#
#  name = "${var.owner_login}-queue-categories"
#
##  fifo_queue = true
#
#  create_queue_policy = true
#  queue_policy_statements = {
#    sns = {
#      sid     = "SNSPublish"
#      actions = ["sqs:SendMessage"]
#
#      principals = [
#        {
#          type        = "Service"
#          identifiers = ["sns.amazonaws.com"]
#        }
#      ]
#
#      condition = {
#        test     = "ArnEquals"
#        variable = "aws:SourceArn"
#        values   = [module.sns.topic_arn]
#      }
#    }
#  }
#
#  tags = { Name  = "${local.name-prefix}-sqs" }
#}