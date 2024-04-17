using AutoMapper;
using Onitama.Core.SchoolAggregate.Contracts;

namespace Onitama.Api.Models.Output;

public class SchoolModel
{
    public PawnModel[] AllPawns { get; set; }
    public CoordinateModel TempleArchPosition { get; set; }
    
    private class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ISchool, SchoolModel>();
        }
    }
}