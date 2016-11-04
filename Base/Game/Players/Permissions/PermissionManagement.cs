using System.Collections.Generic;

namespace Sahara.Base.Game.Players.Permissions
{
    internal class PermissionManagement
    {
        private readonly Player _player;
        private readonly List<string> _commandPermissions;
        private readonly List<string> _permissions;

        public PermissionManagement(Player player)
        {
            _player = player;
            _commandPermissions = new List<string>();
            _permissions = new List<string>();
        }

        public void AddRanges()
        {
            _permissions.AddRange(Sahara.GetServer().GetGameManager().GetPermissionManager().GetPermissionsForPlayer(_player.GetPlayerData()));
            _commandPermissions.AddRange(Sahara.GetServer().GetGameManager().GetPermissionManager().GetCommandsForPlayer(_player.GetPlayerData()));
        }

        public bool HasPermission(string permission)
        {
            return _permissions.Contains(permission);
        }

        public bool HasCommandPermission(string command)
        {
            return _commandPermissions.Contains(command);
        }
    }
}
