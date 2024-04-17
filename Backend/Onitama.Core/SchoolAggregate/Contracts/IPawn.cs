using Onitama.Core.Util.Contracts;

namespace Onitama.Core.SchoolAggregate.Contracts;


/// <summary>
/// Represents a pawn in the game (e.g. a student or a master)
/// </summary>
public interface IPawn
{
    /// <summary>
    /// The unique identifier of the pawn
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// The unique identifier of the player that owns the pawn
    /// </summary>
    Guid OwnerId { get; }

    /// <summary>
    /// The type of the pawn (student, master)
    /// </summary>
    PawnType Type { get; }

    /// <summary>
    /// The position of the pawn on the play mat.
    /// Is null when the pawn is not on the mat (e.g. when it was taken by an enemy pawn)
    /// </summary>
    ICoordinate Position { get; set; }
}