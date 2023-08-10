output "ecr" {
  value = module.ecr
}

output "github_iam_role_arn" {
  value = module.oidc-github.iam_role_arn
}