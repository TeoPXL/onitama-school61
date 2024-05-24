using Onitama.Api.Models.Output;

namespace Onitama.Api.Models.Input;

public class MovePawnModelWotw
{
    public Guid PawnId { get; set; }
    public string MoveCardName { get; set; }
    public CoordinateModel To { get; set; }

    public Guid SpiritId { get; set; }

    public CoordinateModel SpiritTo { get; set; }
}