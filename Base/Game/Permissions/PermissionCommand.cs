using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Base.Game.Permissions
{
    internal class PermissionCommand
    {
        private readonly string _commandName;
        private readonly int _groupId;
        private readonly int _subscriptionId;

        public PermissionCommand(string commandName, int groupId, int subscriptionId)
        {
            _commandName = commandName;
            _groupId = groupId;
            _subscriptionId = subscriptionId;
        }

        public string CommandName => _commandName;
        public int GroupId => _groupId;
        public int SubscriptionId => _subscriptionId;
    }
}
