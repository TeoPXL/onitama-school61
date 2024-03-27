using AutoMapper;
using Onitama.Core.TableAggregate;
using Onitama.Core.TableAggregate.Contracts;

namespace Onitama.Api.Models.Output
{
    public class TableModel
    {
        public Guid Id { get; set; }
        public TablePreferences Preferences { get; set; } = new TablePreferences();
        public Guid OwnerPlayerId { get; set; }
        public List<PlayerModel> SeatedPlayers { get; set; } = new List<PlayerModel>();
        public bool HasAvailableSeat { get; set; }
        public Guid GameId { get; set; }

        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ITable, TableModel>();
            }
        }
    }
}
