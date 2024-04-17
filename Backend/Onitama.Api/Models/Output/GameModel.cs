using AutoMapper;
using Onitama.Core.GameAggregate.Contracts;

namespace Onitama.Api.Models.Output;

public class GameModel
{
    public Guid Id { get; set; }
    public PlayMatModel PlayMat { get; set; }

    public MoveCardModel ExtraMoveCard { get; set; }
    public PlayerModel[] Players { get; set; }

    public Guid PlayerToPlayId { get; set; }

    public Guid WinnerPlayerId { get; set; }

    public string WinnerMethod { get; set; }

    private class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IGame, GameModel>();
        }
    }
}