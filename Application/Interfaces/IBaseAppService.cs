using System.Linq.Expressions;
using Application.Commands;
using Application.Queries;

namespace Application.Interfaces;

public interface IBaseAppService<TC, TQ>
    where TC : BaseCommand
    where TQ : BaseQuery
{
    Task<TQ> GetById(Guid id);
    Task<IEnumerable<TQ>> GetAll();
    Task Add(TC command);
    Task Update(TC command);
    Task Remove(Guid id);
}