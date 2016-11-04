using Sahara.Base.Game.Items;
using Sahara.Base.Game.Players.Inventory.Bots;
using Sahara.Base.Game.Rooms.AI.Pets;
using System.Collections.Concurrent;

namespace Sahara.Base.Game.Players.Inventory
{
    internal class InventoryManagement
    {
        private readonly Player _player;
        private readonly ConcurrentDictionary<int, Bot> _playerBots;
        private readonly ConcurrentDictionary<int, Pet> _playerPets;
        private readonly ConcurrentDictionary<int, GameItem> _playerGameItems;

        public InventoryManagement(Player player)
        {
            _player = player;
            _playerBots = new ConcurrentDictionary<int, Bot>();
            _playerPets = new ConcurrentDictionary<int, Pet>();
            _playerGameItems = new ConcurrentDictionary<int, GameItem>();
        }

        public void Initialize(bool clearBefore = false)
        {
            if (clearBefore)
            {
                if (_playerBots.Count > 1)
                {
                    _playerBots.Clear();
                }

                if (_playerPets.Count > 1)
                {
                    _playerPets.Clear();
                }

                if (_playerGameItems.Count > 1)
                {
                    _playerGameItems.Clear();
                }
            }


        }
    }
}
