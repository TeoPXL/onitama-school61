using AutoMapper;
using Onitama.Core;
using Onitama.Core.UserAggregate;

namespace Onitama.Api.Models.Output;

public class UserModel
{
    public Guid Id { get; set; }

    public string Email { get; set; }
    public string WarriorName { get; set; }

    private class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserModel>();
        }
    }
}