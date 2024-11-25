using Application.Commands.User;
using Application.Interfaces;
using Application.Queries.User;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Services;

namespace Application.Services;

public class UserAppService : BaseAppService<CreateUserCommand, UserQuery, User, IUserService>, IUserAppService
{
    public UserAppService(IMapper mapper, IUserService service) : base(mapper, service)
    {
        
    }
}