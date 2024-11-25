using Application.Commands;
using Application.Interfaces;
using Application.Queries;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Services;

namespace Application.Services;

public abstract class BaseAppService<TC, TQ, TEntity, TS> : IBaseAppService<TC, TQ>
    where TC : BaseCommand
    where TQ : BaseQuery
    where TEntity : class
    where TS : IBaseService<TEntity>
{
    protected readonly IMapper Mapper;
    protected readonly TS Service;
    protected BaseAppService(IMapper mapper, TS service)
    {
        Mapper = mapper;
        Service = service;
    }
    
    public virtual async Task<TQ> GetById(Guid id)
    {
        return Mapper.Map<TQ>(await Service.GetById(id));
    }
    
    public virtual async Task<IEnumerable<TQ>> GetAll()
    {
        return Mapper.Map<IEnumerable<TQ>>(await Service.GetAll());
    }
    
    public virtual async Task Add(TC command)
    {
        await Service.Add(Mapper.Map<TEntity>(command));
    }
    
    public virtual async Task Update(TC command)
    {
        await Service.Update(Mapper.Map<TEntity>(command));
    }
    
    public virtual async Task Remove(Guid id)
    {
        await Service.Remove(id);
    }
}