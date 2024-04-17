using System.Drawing;
using AutoMapper;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.Util;

namespace Onitama.Api.Util;

public class GeneralMappingProfile : Profile
{
    public GeneralMappingProfile()
    {
        CreateMap<Color, string>().ConvertUsing(c => ColorTranslator.ToHtml(c));
        CreateMap<Direction, string>().ConvertUsing(d => d.ToString());
    }
}