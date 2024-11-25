using Application.Commands.User;
using Application.Queries.User;
using AutoMapper;
using Domain.Entities;

namespace Application.AutoMapper;

public class ProfileConfigurationMap : Profile
{
    public ProfileConfigurationMap()
    {
        #region [ User ]
        CreateMap<CreateUserCommand, User>();
        CreateMap<User, UserQuery>();
        #endregion
    }
}