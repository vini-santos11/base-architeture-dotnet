using Application.Models;
using Models.Commands.Role;
using Models.Queries.Role;

namespace Application.Interfaces;

public interface IRoleAppService : IBaseAppService<CreateRoleCommand, RoleQuery>
{
    Task<Response<bool>> AddRole(CreateRoleCommand command);
    Task<Response<bool>> UpdateRole(Guid id, CreateRoleCommand command);
}