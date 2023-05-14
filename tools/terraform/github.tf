#Create oidc provider and IAM user for ECR
# AmazonEC2ContainerRegistryFullAccess, AssumeRole)
data "aws_iam_policy" "ecr-full-access" {
  arn = "arn:aws:iam::aws:policy/AmazonEC2ContainerRegistryFullAccess"
}

data github_actions_public_key "github" {
  repository = local.repo_name
}

data "aws_iam_policy_document" "ecr-full-access" {
  policy_id = data.aws_iam_policy.ecr-full-access.policy_id

    statement {
      actions   = ["ecr:*", "cloudtrail:LookupEvents"]
      resources = ["*"]

    }

    statement {
      actions   = ["iam:CreateServiceLinkedRole"]
      resources = ["*"]
      condition {
        test     = "ForAnyValue:StringEquals"
        variable = "iam:AWSServiceName"
        values   = ["replication.ecr.amazonaws.com"]
      }
    }
}

module "oidc-github" {
  source  = "unfunco/oidc-github/aws"
  version = "1.2.1"
  github_repositories = ["${local.repo_owner}/${local.repo_name}"]

  iam_role_inline_policies = {
    "ecr_inline_policy" : data.aws_iam_policy_document.ecr-full-access.json
  }
  iam_role_name = "${local.name-prefix}-ecr-full-access"

  tags = { "Name" = "${local.name-prefix}-oidc-github" }
}

#Add secrets to github
resource "github_actions_secret" "aws_owner" {
  repository       = local.repo_name
  secret_name      = "AWS_OWNER"
  plaintext_value  = var.owner_login
}

resource "github_actions_secret" "role_to_assume_arn" {
  repository       = local.repo_name
  secret_name      = "ROLE_TO_ASSUME_ARN"
  plaintext_value  = module.oidc-github.iam_role_arn
}

#temp output
output "ecr-policy" {
  value = data.aws_iam_policy_document.ecr-full-access
}

