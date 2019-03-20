# Convert Linq Expression to Dynamic Query String

I have created this Linq Expression helper as a tool that should enable me to use the Linq filtering with Entity Framework in the Onion Architecture.

## Problem

Problem with using the Entity Framework in Onion Architecture is when we want simple filtering below the Repository Layer (eg. Service Layer). Entity Framework (Repository Layer) is working with the Entity objects, Service Layer works with the Domain Models (POCO's). Mapping will be handled in the Repository Layer, but we can't easily use the Linq for filtering in the Service Layer, because it needs to be created with the Entity objects. Consider the example below.

```csharp
 protected virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null)
 {
    return dbContext.Set<TEntity>().Where(filter).ToList();
 }
```

That is a valid method. We are passing a Lambda Expression as a filter, and that expression is built from the object of a TEntity type. The problem here is that we can't call this method from the Service Layer because Service Layer don't have access to the TEntity class, it is part of the Data Access Layer. Now consider the example below.

```csharp
 protected virtual IEnumerable<TEntity> Get(Expression<Func<TModel, bool>> filter = null)
 {
    return dbContext.Set<TEntity>().Where(filter).ToList();
 }
```

This is not a valid method because our dbContext can't work with the TModel, it is initialized with the TEntity. But this kind of method could be called from the Service Layer.

## Solution

There is a nice NuGet package called System.Linq.Dynamic that can compose Linq queries at runtime. It has a SQL-like syntax, please check the documentation for the more info about that. My helper converts a Lambda Expression to the SQL-like syntax the System.Linq.Dynamic is using, so it enables us to do this:

```csharp
using System.Linq.Dynamic;
using DynamicExpression.Core;

 protected virtual IEnumerable<TEntity> Get(Expression<Func<TModel, bool>> filter = null)
 {
    return dbContext.Set<TEntity>().Where(DynamicExpressionHandler.GetDynamicQueryString(filter.Body)).ToList();
 }
```

## Important Note
In the example above I have said that the Get method could be called from the Service Layer. That is not entirely true, I didn't want to complicate the examples. The method is returning a TEntity so it can't be called from the Service Layer, that method is part of the Generic Repository, and will be called from a specific repository. Below is an example of how could that be called:

#### CompanyRepository.cs
```csharp
private readonly IGenericRepository<Company, ICompanyPoco> genericRepository;

public virtual IEnumerable<ICompanyPoco> Get(Expression<Func<ICompanyPoco, bool>> filter = null)
{
    var entities = genericRepository.Get(filter);
    return mapper.Map<IEnumerable<ICompanyPoco>>(entities);
}
```

#### CompanyService.cs
```csharp
private readonly ICompanyRepository companyRepository;

public virtual IEnumerable GetCompanies(Expression<Func<ICompanyPoco, bool>> filter = null)
{
    return companyRepository.Get(filter);
}
```

## Limitations
Currently the helper is not supporting the Navigation Properties (or any complex types).
Can do:  
```csharp
company.Where(p => p.Id == Guid.Empty);
```

Can't do:  
```csharp
company.Where(p => p.User.Id == Guid.Empty);
```

