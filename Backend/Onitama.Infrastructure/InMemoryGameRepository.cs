using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Infrastructure.Util;

namespace Onitama.Infrastructure
{
    internal class InMemoryGameRepository : IGameRepository
    {
        private readonly ExpiringDictionary<Guid, IGame> _gameDictionary;

        public InMemoryGameRepository()
        {
            _gameDictionary = new ExpiringDictionary<Guid, IGame>(TimeSpan.FromHours(5));
        }

        public void Add(IGame newGame)
        {
            _gameDictionary.AddOrReplace(newGame.Id, newGame);
        }

        public IGame GetById(Guid id)
        {
            if (_gameDictionary.TryGetValue(id, out IGame game))
            {
                return game;
            }
            throw new DataNotFoundException();
        }
    }
}