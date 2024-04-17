using AutoMapper;
using Onitama.Core.SchoolAggregate.Contracts;

namespace Onitama.Api.Models.Output;

public class PawnModel
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public PawnType Type { get; set; }
    public CoordinateModel Position { get; set; }

    private class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IPawn, PawnModel>();
        }
    }
}