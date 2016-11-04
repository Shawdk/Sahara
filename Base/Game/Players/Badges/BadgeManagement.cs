using Sahara.Base.Game.Badges;
using Sahara.Core.Net.Messages.Outgoing.Packets.Handshake;
using System.Collections.Generic;

namespace Sahara.Base.Game.Players.Badges
{
    internal class BadgeManagement
    {
        private readonly Player _player;
        private readonly Dictionary<string, Badge> _playerBadges;

        public BadgeManagement(Player player)
        {
            _player = player;
            _playerBadges = new Dictionary<string, Badge>();
        }

        public int BadgeCount => _playerBadges.Count;
        public Dictionary<string, Badge> Badges => _playerBadges;

        public void LoadBadges()
        {

        }

        public bool HasBadge(string badgeCode)
        {
            return _playerBadges.ContainsKey(badgeCode);
        }

        public void GiveBadge(string badgeCode, bool addToDatabase)
        {
            if (_playerBadges.ContainsKey(badgeCode))
            {
                return;
            }

            BadgeDefinition badgeDefinition;

            if (!Sahara.GetServer().GetGameManager().GetBadgeManager().TryGetBadge(badgeCode.ToUpper(), out badgeDefinition) || badgeDefinition.PermissionRequired.Length > 0 && !_player.GetPlayerData().GetPermissionManagement().HasPermission(badgeDefinition.PermissionRequired))
            {
                return;
            }

            _playerBadges.Add(badgeCode, new Badge(badgeCode, 0));

            if (addToDatabase)
            {
                using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
                {
                    mysqlConnection.OpenConnection();
                    mysqlConnection.AddParameter("id", _player.GetPlayerData().PlayerId);
                    mysqlConnection.AddParameter("badge", badgeCode);
                    mysqlConnection.SetQuery("INSERT INTO user_badges `user_id`, `badge_id`, `badge_slot` VALUES (@id, @badge, 0)");
                    mysqlConnection.RunQuery();
                    mysqlConnection.CloseConnection();
                }
            }

            if (_player == null)
            {
                return;
            }

            _player.SendMessage(new BadgesMessageComposer(_player));
            _player.SendMessage(new FurniListNotificationMessageComposer(1, 4));
        }
    }
}
