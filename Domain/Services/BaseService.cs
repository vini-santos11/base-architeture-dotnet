using System.Linq.Expressions;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Domain.Services;

public abstract class BaseService<TEntity, R> : IBaseService<TEntity> 
    where TEntity : class
    where R : IBaseRepository<TEntity>
{
    protected readonly R Repository;
    protected BaseService(R repository)
    {
        Repository = repository;        
    }
    
    protected virtual void ValidateAdd(TEntity entity)
    {
    }
    
    protected virtual void ValidateUpdate(TEntity entity)
    {
    }
    
    protected virtual void ValidateRemove(Guid id)
    {
    }
    
    public virtual async Task<TEntity?> GetById(Guid id)
    {
        return await Repository.GetById(id);
    }
    
    public virtual async Task<IEnumerable<TEntity>> GetAll()
    {
        return await Repository.GetAll();
    }
    
    public virtual async Task Add(TEntity entity)
    {
        ValidateAdd(entity);
        await Repository.Add(entity);
    }
    
    public virtual async Task Update(TEntity entity)
    {
        ValidateUpdate(entity);
        await Repository.Update(entity);
    }
    
    public virtual async Task Remove(Guid id)
    {
        ValidateRemove(id);
        await Repository.Remove(id);
    }
    
    public virtual IQueryable<TEntity?> GetByExpression(Expression<Func<TEntity, bool>> expression)
    {
        return Repository.GetByExpression(expression);
    }
    
    public virtual TEntity? GetElementByExpression(Expression<Func<TEntity, bool>> expression)
    {
        return Repository.GetElementByExpression(expression);
    }
}