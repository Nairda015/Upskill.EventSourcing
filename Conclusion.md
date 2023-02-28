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
- [ ] ByName - first 3 chars 
- [x] AllChildren  
- [X] ById  
- [x] MainCategories - all without parent  

### GetProducts Queries: - OpenSearch
- [ ] GetPagedByCategoryWithOrder  
- [ ] GetPriceHistory - lowest price from 30 last days :D and charts (query from es or create projection for dynamo) 
- [ ] FullTextSearch - name and description  
- [ ] FilterByPrice  
- [ ] FilterByMetadata  

- [ ] moving data to archives - EFS Infrequent Access  
- [ ] logs - CloudWatch  
- [ ] alarms for errors with mail notification - CloudWatch  


### TODO:
- [ ] replace ef with dapper in commands
- [ ] add sql for creating category table
- [ ] add open search to projection
- [ ] add open search to terraform
- [ ] implement queries
- [ ] add CICD for ecr
- [ ] add secrets to gh
- [ ] create IAM for ECR
- [ ] add service to ECS from ECR
- [ ] add dynamo to terraform
- [ ] add api gateway to terraform
- [ ] enable cloudwatch logs for all apps