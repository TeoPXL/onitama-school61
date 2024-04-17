using Onitama.Api.Models.Output;

namespace Onitama.Api.Models.Input;

public class MovePawnModel
{
    public Guid PawnId { get; set; }
    public string MoveCardName { get; set; }
    public CoordinateModel To { get; set; }
}