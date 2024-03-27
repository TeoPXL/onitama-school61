using AutoMapper;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util.Contracts;

namespace Onitama.Api.Models.Output;

public class CoordinateModel
{
    public int Row { get; set; }
    public int Column { get; set; }

    private class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ICoordinate, CoordinateModel>();
        }
    }
}