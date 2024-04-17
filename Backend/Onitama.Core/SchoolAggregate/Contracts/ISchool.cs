using Onitama.Core.Util.Contracts;

namespace Onitama.Core.SchoolAggregate.Contracts;


/// <summary>
/// Represents a school (one master and 4 students)
/// </summary>
public interface ISchool
{
    /// <summary>
    /// The master of the school
    /// </summary>
    IPawn Master { get; }

    /// <summary>
    /// The students of the school (4 students)
    /// </summary>
    IPawn[] Students { get; }

    /// <summary>
    /// All the pawns of the school (one master and 4 students)
    /// </summary>
    IPawn[] AllPawns { get; }

    /// <summary>
    /// The position on the play mat of the temple arch of the school
    /// </summary>
    ICoordinate TempleArchPosition { get; set; }

    /// <summary>
    /// Retrieves a pawn (student or master) by its unique identifier.
    /// Returns null if the pawn is not found.
    /// </summary>
    /// <param name="pawnId">The unique identifier of the pawn</param>
    IPawn GetPawn(Guid pawnId);
}