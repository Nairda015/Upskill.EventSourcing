terraform {
  required_providers {
    github = {
      source  = "integrations/github"
      version = "~> 5.0"
    }
  }
}

provider "github" { }

provider "aws" {
  region = var.region
  default_tags {
    tags = {
      Environment = var.env_prefix
      Name        = local.name-prefix
      Owner       = var.owner_login
      ManagedBy   = "terraform"
    }
  }
}