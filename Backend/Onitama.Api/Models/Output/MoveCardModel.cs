using AutoMapper;
using Onitama.Core.MoveCardAggregate.Contracts;

namespace Onitama.Api.Models.Output;

public class MoveCardModel
{
    public string Name { get; set; }
    public MoveCardGridCellType[,] Grid { get; set; }
    public string StampColor { get; set; }

    private class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IMoveCard, MoveCardModel>();
        }
    }
}