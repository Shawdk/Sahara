using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Base.Game.Players.Messenger
{
    internal class MessengerBuddyRequest
    {
        private int _toUser;
        private int _fromUser;
        private string _username;

        public MessengerBuddyRequest(int toUser, int fromUser, string username)
        {
            _toUser = toUser;
            _fromUser = fromUser;
            _username = username;
        }
    }
}
