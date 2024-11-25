using System.Linq.Expressions;

namespace Domain.Interfaces.Repositories;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetById(Guid id);
    Task<IEnumerable<TEntity>> GetAll();
    Task Add(TEntity entity);
    Task Update(TEntity entity);
    Task Remove(Guid id);
    IQueryable<TEntity?> GetByExpression(Expression<Func<TEntity, bool>> expression);
    TEntity? GetElementByExpression(Expression<Func<TEntity, bool>> expression);
}