using AutoMapper;
using Domain.Entities;
using Models.Commands.User;
using Models.Queries.User;

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