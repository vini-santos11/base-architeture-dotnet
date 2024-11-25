using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Interfaces.Services;

public interface IBaseService<TEntity> where TEntity : class
{
    Task<TEntity?> GetById(Guid id);
    Task<IEnumerable<TEntity>> GetAll();
    Task Add(TEntity entity);
    Task Update(TEntity entity);
    Task Remove(Guid id);
    IQueryable<TEntity?> GetByExpression(Expression<Func<TEntity, bool>> expression);
    TEntity? GetElementByExpression(Expression<Func<TEntity, bool>> expression);
}