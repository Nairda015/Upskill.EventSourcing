#module "event-store-db" {
#  source          = "./modules/eventstore"
#  avail_zone      = var.avail_zone
#  my_ip           = var.my_ip
#  name_prefix     = local.name-prefix
#  public_key_path = var.public_key_path
#  vpc_id          = module.vpc.vpc_id
#  subnet_id       = module.vpc.public_subnets[0]
#}