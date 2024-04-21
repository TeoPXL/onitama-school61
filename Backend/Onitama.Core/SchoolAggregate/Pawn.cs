using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.SchoolAggregate;

/// <inheritdoc cref="IPawn"/>
internal class Pawn : IPawn

{
    public Guid Id { get; }
    public Guid OwnerId { get; }

    public PawnType Type { get; }

    public ICoordinate Position { get; set; }

    public Pawn(Guid id, Guid ownerId, PawnType type)
    {
        Id = id;
        OwnerId = ownerId;
        Type = type;
    }

}