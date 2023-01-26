## Conclusions

### Products Commands:
- [ ] CreateProduct  
- [ ] ChangeCategory  
- [ ] ChangePrice (split this into multiple with additional metadata for eg promo price)  
- [ ] ChangeDescription  
- [ ] UpdateMetadata - add, remove  
- [ ] MarkAsObsolete  

### Categories Queries - Aurora
- [ ] ByName  
- [ ] AllChildren  
- [ ] ById  
- [x] MainCategories - all without parent  

### GetProducts Queries: - OpenSearch
- [ ] GetPagedByCategoryWithOrder  
- [ ] GetPriceHistory - lowest price from 30 last days :D and charts  
- [ ] FullTextSearch - name and description  
- [ ] FilterByPrice  
- [ ] FilterByMetadata  

- [ ] moving data to archives - EFS Infrequent Access  
- [ ] logs - CloudWatch  
- [ ] alarms for errors with mail notification - CloudWatch  