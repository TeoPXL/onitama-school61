using AutoMapper;
using Onitama.Core.PlayerAggregate.Contracts;

namespace Onitama.Api.Models.Output;

public class PlayerModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public string Direction { get; set; }

    public SchoolModel School { get; set; }

    public MoveCardModel[] MoveCards { get; set; }

    private class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IPlayer, PlayerModel>();
        }
    }
}