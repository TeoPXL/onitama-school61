using AutoMapper;
using Onitama.Core.GameAggregate.Contracts;

namespace Onitama.Api.Models.Output;

public class MoveModel
{
    public MoveCardModel Card { get; set; }
    public string Direction { get; set; }
    public PawnModel Pawn { get; set; }
    public CoordinateModel To { get; set; }

    private class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IMove, MoveModel>();
        }
    }
}