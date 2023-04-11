## Conclusions

### Categories Commands
- [x] AddCategory - unique name and parent, parent can be null
- [x] DeleteCategory - only if empty and dont have subcategories //TODO: check in OpenSearch

### Products Commands:
- [x] CreateProduct  
- [x] ChangeCategory  
- [x] ChangePrice (split this into multiple with additional metadata for eg promo price)  
- [x] ChangeDescription  
- [x] UpdateMetadata - add, remove  
- [x] MarkAsObsolete

### Categories Queries - Aurora
- [x] ByName - first 3 chars 
- [x] AllChildren  
- [X] ById  
- [x] MainCategories - all without parent  

### GetProducts Queries: - OpenSearch
- [x] GetById
- [x] GetPagedByCategory 
- [x] FullTextSearch - name and description  
- [x] FilterByPrice  
- [x] FilterByMetadata  

### Dynamo
- [ ] GetPriceHistory* - lowest price from 30 last days :D and charts (query from es or create projection for dynamo)

### Other
- [x] moving data to archives - EFS Infrequent Access  
- [ ] logs - CloudWatch  
- [ ] alarms for errors with mail notification - CloudWatch  

### Terraform:
- [ ] create IAMs policies for: 
    - [ ] Aurora (for commands and queries service)
    - [ ] OpenSearch (for projection and queries service)
    - [ ] Dynamo (for projection and queries service)*
    - [ ] ECR (for github - AmazonEC2ContainerRegistryFullAccess, AssumeRole)
    - [ ] ECS (for command service)
    - [ ] API Gateway (forwards request to lambda)
- [ ] add open search to terraform
- [ ] add CICD for ECR
- [ ] add secrets to gh
- [ ] add background service to ECS from ECR (listener)
- [ ] add lambda to terraform from ECR (commands, queries and projections services)
- [ ] add dynamo to terraform*
- [ ] add api gateway to terraform
- [ ] enable cloudwatch logs for all apps (maybe integrate with xray)