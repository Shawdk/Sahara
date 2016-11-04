using System.Collections.Concurrent;

namespace Sahara.Base.Game.Players
{
    internal class PlayerManager
    {
        private readonly ConcurrentDictionary<int, Player> _playersConnected;

        public PlayerManager()
        {
            _playersConnected = new ConcurrentDictionary<int, Player>();
        }

        public bool TryAddPlayer(Player player)
        {
            if (!_playersConnected.TryAdd(_playersConnected.Count, player))
            {
                return false;
            }

            player.StartPacketProcessing();
            return true;
        }
    }
}

