using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SimpleApp.Entities;

namespace Commands.Persistence;

public class InMemoryDb
{
    private readonly IDictionary<Guid, Product> _products = new ConcurrentDictionary<Guid, Product>();

    public ReadOnlyDictionary<Guid, Product> Products => _products.AsReadOnly();
    
    public void Add(Product product) => _products.Add(product.Id, product);

    public void Delete(Guid id) => _products.Remove(id);

    public bool Update(Product product)
    {
        if (!_products.ContainsKey(product.Id)) return false;
        _products[product.Id] = product;
        return true;
    }
    
    // For tests only
    public void Clear() => _products.Clear();
}