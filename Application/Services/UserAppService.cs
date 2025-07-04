using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Services;
using Models.Commands.User;
using Models.Queries.User;

namespace Application.Services;

public class UserAppService : BaseAppService<CreateUserCommand, UserQuery, User, IUserService>, IUserAppService
{
    public UserAppService(IMapper mapper, IUserService service) : base(mapper, service)
    {
        
    }
}