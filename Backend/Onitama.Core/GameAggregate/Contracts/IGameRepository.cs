namespace Onitama.Core.GameAggregate.Contracts;

public interface IGameRepository
{
    void Add(IGame newGame);
    IGame GetById(Guid id);
    List<IGame> GetAll();
}