using AutoMapper;
using Onitama.Core.PlayMatAggregate.Contracts;

namespace Onitama.Api.Models.Output;

public class PlayMatModel
{
    public PawnModel[,] Grid { get; set; }
    public int Size { get; set; }

    private class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IPlayMat, PlayMatModel>();
        }
    }
}