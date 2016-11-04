using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Base.Game.Players.Messenger
{
    internal class PlayerMessenger
    {
        private readonly MessengerState _playerMessengerState;
        private readonly Player _player;
        private readonly Dictionary<int, MessengerBuddyRequest> _playerBuddyRequests;
        private readonly Dictionary<int, MessengerBuddy> _playerBuddies;
        private readonly MessengerBarState _messengerBarState;

        public PlayerMessenger(Player player, Dictionary<int, MessengerBuddyRequest> playerBuddyRequests, Dictionary<int, MessengerBuddy> playerBuddies, int messengerBarState)
        {
            _player = player;
            _playerBuddyRequests = playerBuddyRequests;
            _playerBuddies = playerBuddies;
            _messengerBarState = (MessengerBarState) messengerBarState;
        }

        public MessengerBarState MessengerBarState => _messengerBarState;
    }
}
