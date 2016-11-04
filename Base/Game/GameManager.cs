using Sahara.Base.Game.Achievements;
using Sahara.Base.Game.Badges;
using Sahara.Base.Game.Moderation;
using Sahara.Base.Game.Permissions;
using Sahara.Base.Game.Players;
using Sahara.Base.Game.Rooms;
using Sahara.Base.Game.Subscriptions;

namespace Sahara.Base.Game
{
    internal class GameManager
    {
        private readonly PlayerManager _playerManager;
        private readonly BadgeManager _badgeManager;
        private readonly RoomManager _roomManager;
        private readonly AchievementManager _achievementManager;
        private readonly PermissionManager _permissionManager;
        private readonly SubscriptionManager _subscriptionManager;
        private readonly ModerationManager _moderationManager;

        public GameManager()
        {
            _playerManager = new PlayerManager();
            _badgeManager = new BadgeManager();
            _roomManager = new RoomManager();
            _achievementManager = new AchievementManager();
            _permissionManager = new PermissionManager();
            _subscriptionManager = new SubscriptionManager();
            _moderationManager = new ModerationManager();
        }

        public PlayerManager GetPlayerManager()
        {
            return _playerManager;
        }

        public BadgeManager GetBadgeManager()
        {
            return _badgeManager;
        }

        public RoomManager GetRoomManager()
        {
            return _roomManager;
        }

        public AchievementManager GetAchievementManager()
        {
            return _achievementManager;
        }

        public PermissionManager GetPermissionManager()
        {
            return _permissionManager;
        }

        public SubscriptionManager GetSubscriptionManager()
        {
            return _subscriptionManager;
        }

        public ModerationManager GetModerationManager()
        {
            return _moderationManager;
        }
    }
}

