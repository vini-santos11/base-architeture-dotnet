using Application.Commands.Role;
using Application.Queries.Role;

namespace Application.Interfaces;

public interface IRoleAppService : IBaseAppService<CreateRoleCommand, RoleQuery>
{
    Task AddRole(CreateRoleCommand command);
    Task UpdateRole(Guid id, CreateRoleCommand command);
}