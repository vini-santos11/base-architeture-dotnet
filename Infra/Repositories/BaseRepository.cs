using System.Linq.Expressions;
using Domain.Interfaces.Repositories;
using Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    protected readonly BaseContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected BaseRepository(BaseContext context)
    {
        Context = context;
        DbSet = Context.Set<TEntity>();
    }
    
    public async Task<TEntity?> GetById(Guid id)
    {
        return await DbSet.FindAsync(id);
    }
    
    public async Task<IEnumerable<TEntity>> GetAll()
    {
        return await DbSet.ToListAsync();
    }
    
    public async Task Add(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        await Context.SaveChangesAsync();
    }
    
    public async Task Update(TEntity entity)
    {
        DbSet.Update(entity);
        await Context.SaveChangesAsync();
    }
    
    public virtual async Task Remove(Guid id)
    {
        var entity = await GetById(id) ?? throw new Exception($"{nameof(TEntity)} not found");
        DbSet.Remove(entity);
        await Context.SaveChangesAsync();
    }
    
    public IQueryable<TEntity?> GetByExpression(Expression<Func<TEntity, bool>> expression)
    {
        return DbSet.Where(expression).AsNoTracking();
    }
    
    public TEntity? GetElementByExpression(Expression<Func<TEntity, bool>> expression)
    {
        return DbSet.AsNoTracking().FirstOrDefault(expression);
    }
}