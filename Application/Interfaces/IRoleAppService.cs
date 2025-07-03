using Application.Commands.Role;
using Application.Models;
using Application.Queries.Role;

namespace Application.Interfaces;

public interface IRoleAppService : IBaseAppService<CreateRoleCommand, RoleQuery>
{
    Task<Response<bool>> AddRole(CreateRoleCommand command);
    Task<Response<bool>> UpdateRole(Guid id, CreateRoleCommand command);
}