using Sahara.Base.Game.Achievements;
using Sahara.Base.Game.Players.Badges;
using Sahara.Base.Game.Players.Clothing;
using Sahara.Base.Game.Players.Effects;
using Sahara.Base.Game.Players.Inventory;
using Sahara.Base.Game.Players.Messenger;
using Sahara.Base.Game.Players.Permissions;
using Sahara.Base.Game.Players.Relationships;
using Sahara.Base.Game.Rooms;
using Sahara.Core.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace Sahara.Base.Game.Players
{
    internal class PlayerData
    {
        private readonly Player _player;
        private readonly int _playerId;
        private readonly string _username;
        private readonly string _motto;
        private readonly string _look;
        private readonly int _homeRoom;
        private readonly int _rank;
        private readonly int _achievementScore;
        private readonly List<int> _clientVolumes;
        private readonly bool _chatPreference;
        private readonly bool _allowMessengerInvites;
        private readonly bool _focusPreference;
        private readonly int _vipRank;
        private string _machineId;
        private readonly PermissionManagement _permissionManagement;
        private bool _initializedData;
        private readonly ConcurrentDictionary<string, PlayerAchievement> _playerAchievements;
        private readonly List<int> _favouriteRoomIds;
        private readonly List<int> _mutedUsers;
        private readonly BadgeManagement _badgeManagement;
        private readonly InventoryManagement _inventoryManagement;
        private readonly Dictionary<int, int> _playerQuests;
        private readonly Dictionary<int, MessengerBuddy> _playerBuddies;
        private PlayerMessenger _playerMessenger;
        private readonly List<RoomInformation> _playersRooms;
        private readonly Dictionary<int, PlayerRelationship> _playerRelationships;
        private readonly EffectManagement _effectManagement;
        private readonly ClothingManagement _clothingManagement;
        private readonly LogManager _logManager;
        private readonly PlayerProcessor _processor;

        public PlayerData(Player player, int playerId, string username, string motto, string look, int homeRoom, int rank, int achievementScore, string clientVolume, bool chatPreference, bool allowMessengerInvites, bool focusPreference, int vipRank)
        {
            _player = player;
            _playerId = playerId;
            _username = username;
            _motto = motto;
            _look = look;
            _homeRoom = homeRoom;
            _rank = rank;
            _achievementScore = achievementScore;
            _clientVolumes = new List<int>();
            _chatPreference = chatPreference;
            _allowMessengerInvites = allowMessengerInvites;
            _focusPreference = focusPreference;
            _vipRank = vipRank;
            _permissionManagement = new PermissionManagement(_player);
            _playerAchievements = new ConcurrentDictionary<string, PlayerAchievement>();
            _favouriteRoomIds = new List<int>();
            _mutedUsers = new List<int>();
            _badgeManagement = new BadgeManagement(_player);
            _inventoryManagement = new InventoryManagement(_player);
            _playerQuests = new Dictionary<int, int>();
            _playerBuddies = new Dictionary<int, MessengerBuddy>();
            _playersRooms = new List<RoomInformation>();
            _playerRelationships = new Dictionary<int, PlayerRelationship>();
            _effectManagement = new EffectManagement(_player);
            _clothingManagement = new ClothingManagement(_player);
            _logManager = Sahara.GetServer().GetLogManager();
            _processor = new PlayerProcessor(_player);

            foreach (var volumeString in clientVolume.Split(','))
            {
                if (string.IsNullOrEmpty(volumeString))
                {
                    continue;
                }

                var volumeValue = 0;
                _clientVolumes.Add(int.TryParse(volumeString, out volumeValue) ? int.Parse(volumeString) : 100);
            }
        }

        public int PlayerId => _playerId;
        public string Username => _username;
        public string Motto => _motto;
        public string Look => _look;
        public int HomeRoom => _homeRoom;
        public int Rank => _rank;
        public int AchievementScore => _achievementScore;
        public IEnumerable<int> ClientVolumes => _clientVolumes;
        public List<int> FavouriteRoomIds => _favouriteRoomIds;
        public bool ChatPreference => _chatPreference;
        public bool AllowMessengerInvites => _allowMessengerInvites;
        public bool FocusPreference => _focusPreference;
        public int VipRank => _vipRank;
        public List<RoomInformation> PlayersRooms => _playersRooms;

        public string MachineId
        {
            get { return _machineId; }
            set { _machineId = value; }
        }

        public void InitializeData()
        {
            if (_initializedData)
            {
                return;
            }

            LoadAchievements();
            LoadFavouriteRooms();
            LoadIgnores();
            LoadQuests();
            LoadMessenger();
            LoadRooms();
            LoadRelationships();

            _permissionManagement.AddRanges();
            _effectManagement.LoadEffects();
            _clothingManagement.LoadClothingParts();

            _initializedData = true;
        }

        private void LoadAchievements()
        {
            using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
            {
                try
                {
                    mysqlConnection.OpenConnection();

                    if (!_playerAchievements.IsEmpty)
                    {
                        return;
                    }

                    mysqlConnection.SetQuery("SELECT `group`,`level`,`progress` FROM `user_achievements` WHERE `userid` = @userid");
                    mysqlConnection.AddParameter("userid", _playerId);

                    var achievementsTable = mysqlConnection.GetTable();

                    if (achievementsTable == null)
                    {
                        return;
                    }

                    foreach (DataRow achievemenRow in achievementsTable.Rows)
                    {
                        _playerAchievements.TryAdd(Convert.ToString(achievemenRow["group"]),
                            new PlayerAchievement(Convert.ToString(achievemenRow["group"]),
                                Convert.ToInt32(achievemenRow["level"]), Convert.ToInt32(achievemenRow["progress"])));
                    }
                }
                catch (Exception exception)
                {
                    var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                    _logManager.Log(exception.StackTrace, LogType.Error);
                }
                finally
                {
                    mysqlConnection.CloseConnection();
                }
            }
        }

        private void LoadFavouriteRooms()
        {
            using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
            {
                try
                {
                    mysqlConnection.OpenConnection();

                    if (_favouriteRoomIds.Count >= 1)
                    {
                        return;
                    }

                    mysqlConnection.SetQuery("SELECT `room_id` FROM `user_favorites` WHERE `user_id` = @userid");
                    mysqlConnection.AddParameter("userid", _playerId);
                    var favouriteRoomsTable = mysqlConnection.GetTable();

                    if (favouriteRoomsTable == null)
                    {
                        return;
                    }

                    foreach (DataRow favouriteRoom in favouriteRoomsTable.Rows)
                    {
                        _favouriteRoomIds.Add(Convert.ToInt32(favouriteRoom["room_id"]));
                    }
                }
                catch (Exception exception)
                {
                    var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                    _logManager.Log(exception.StackTrace, LogType.Error);
                }
                finally
                {
                    mysqlConnection.CloseConnection();
                }
            }
        }

        private void LoadIgnores()
        {
            using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
            {
                try
                {
                    mysqlConnection.OpenConnection();

                    if (_mutedUsers.Count >= 1)
                    {
                        return;
                    }

                    mysqlConnection.AddParameter("userId", _playerId);
                    mysqlConnection.SetQuery("SELECT `ignore_id` FROM `user_ignores` WHERE `user_id` = @userId");
                    var mutedUsersTable = mysqlConnection.GetTable();

                    if (mutedUsersTable == null)
                    {
                        return;
                    }

                    foreach (DataRow mutedUser in mutedUsersTable.Rows)
                    {
                        _mutedUsers.Add(Convert.ToInt32(mutedUser["ignore_id"]));
                    }
                }
                catch (Exception exception)
                {
                    var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                    _logManager.Log(exception.StackTrace, LogType.Error);
                }
                finally
                {
                    mysqlConnection.CloseConnection();
                }
            }
        }

        private void LoadQuests()
        {
            using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
            {
                mysqlConnection.OpenConnection();

                try
                {
                    if (_playerQuests.Count >= 1)
                    {
                        return;
                    }

                    mysqlConnection.AddParameter("userId", _playerId);
                    mysqlConnection.SetQuery("SELECT `quest_id`,`progress` FROM `user_quests` WHERE `user_id` = @userId");
                    var questsTable = mysqlConnection.GetTable();

                    if (questsTable == null)
                    {
                        return;
                    }

                    foreach (DataRow quest in questsTable.Rows)
                    {
                        var questId = Convert.ToInt32(quest["quest_id"]);

                        if (_playerQuests.ContainsKey(questId))
                        {
                            _playerQuests.Remove(questId);
                        }

                        _playerQuests.Add(questId, Convert.ToInt32(quest["progress"]));
                    }
                }
                catch (Exception exception)
                {
                    var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                    _logManager.Log(exception.StackTrace, LogType.Error);
                }
                finally
                {
                    mysqlConnection.CloseConnection();
                }
            }
        }

        private void LoadMessenger()
        {
            using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
            {
                mysqlConnection.OpenConnection();

                try
                {
                    if (_playerMessenger != null)
                    {
                        return;
                    }

                    mysqlConnection.AddParameter("userTwoId", _playerId);
                    mysqlConnection.AddParameter("userOneId", _playerId);
                    mysqlConnection.SetQuery(@"SELECT users.id,users.username,users.motto,users.look,users.last_online,users.hide_inroom,users.hide_online FROM users JOIN messenger_friendships ON users.id = messenger_friendships.user_one_id WHERE messenger_friendships.user_two_id = @userTwoId UNION ALL SELECT users.id,users.username,users.motto,users.look,users.last_online,users.hide_inroom,users.hide_online FROM users JOIN messenger_friendships ON users.id = messenger_friendships.user_two_id WHERE messenger_friendships.user_one_id = @userOneId");
                    var friendsTable = mysqlConnection.GetTable();

                    if (friendsTable != null)
                    {
                        foreach (DataRow friend in friendsTable.Rows)
                        {
                            var friendUserId = Convert.ToInt32(friend["id"]);

                            if (friendUserId == _playerId)
                            {
                                continue;
                            }

                            if (_playerBuddies.ContainsKey(friendUserId))
                            {
                                continue;
                            }

                            var friendUsername = Convert.ToString(friend["username"]);
                            var friendClothing = Convert.ToString(friend["look"]);
                            var friendMotto = Convert.ToString(friend["motto"]);
                            var friendLastOnline = Convert.ToInt32(friend["last_online"]);
                            var friendMessengerState = Convert.ToInt32(friend["hide_online"]).ToString() == "1" ? MessengerState.Offline : MessengerState.Online;
                            var friendHideInRoom = Convert.ToInt32(friend["hide_inroom"]).ToString() == "1";

                            _playerBuddies.Add(friendUserId,
                                new MessengerBuddy(friendUserId, friendUsername, friendClothing, friendMotto,
                                    friendLastOnline, friendMessengerState, friendHideInRoom));
                        }
                    }

                    mysqlConnection.SetQuery("SELECT messenger_requests.from_id,messenger_requests.to_id,users.username FROM users JOIN messenger_requests ON users.id = messenger_requests.from_id WHERE messenger_requests.to_id = " + _playerId);
                    var requestsTable = mysqlConnection.GetTable();

                    if (requestsTable != null)
                    {
                        var requests = new Dictionary<int, MessengerBuddyRequest>();

                        foreach (DataRow dRow in requestsTable.Rows)
                        {
                            var receiverId = Convert.ToInt32(dRow["from_id"]);
                            var senderId = Convert.ToInt32(dRow["to_id"]);
                            var requestUsername = Convert.ToString(dRow["username"]);

                            if (receiverId != _playerId)
                            {
                                if (!requests.ContainsKey(receiverId))
                                    requests.Add(receiverId, new MessengerBuddyRequest(_playerId, receiverId, requestUsername));
                            }
                            else
                            {
                                if (!requests.ContainsKey(senderId))
                                    requests.Add(senderId, new MessengerBuddyRequest(_playerId, senderId, requestUsername));
                            }
                        }
                    }

                    _playerMessenger = new PlayerMessenger(_player, null, _playerBuddies, 0);
                }
                catch (Exception exception)
                {
                    var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                    _logManager.Log(exception.StackTrace, LogType.Error);
                }
                finally
                {
                    mysqlConnection.CloseConnection();
                }
            }
        }

        private void LoadRooms()
        {
            using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
            {
                mysqlConnection.OpenConnection();

                try
                {
                    mysqlConnection.AddParameter("userId", _playerId);
                    mysqlConnection.SetQuery("SELECT * FROM `rooms` WHERE `owner` = @userId LIMIT 200");
                    var roomsTable = mysqlConnection.GetTable();

                    if (roomsTable == null)
                    {
                        return;
                    }

                    foreach (DataRow playerRoom in roomsTable.Rows)
                    {
                        RoomInformation roomInformation;

                        if (Sahara.GetServer().GetGameManager().GetRoomManager().TryGetRoomInformation(Convert.ToInt32(playerRoom["id"]), out roomInformation))
                        {
                            _playersRooms.Add(roomInformation);
                        }
                    }
                }
                catch (Exception exception)
                {
                    var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                    _logManager.Log(exception.StackTrace, LogType.Error);
                }
                finally
                {
                    mysqlConnection.CloseConnection();
                }
            }
        }

        private void LoadRelationships()
        {
            using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
            {
                mysqlConnection.OpenConnection();

                try
                {
                    mysqlConnection.AddParameter("userId", _playerId);
                    mysqlConnection.SetQuery(
                        "SELECT `id`, `target`, `type` FROM `user_relationships` WHERE `user_id` = @userId");
                    var relationshipsTable = mysqlConnection.GetTable();

                    if (relationshipsTable == null)
                    {
                        return;
                    }

                    foreach (DataRow relationship in relationshipsTable.Rows)
                    {
                        if (_playerRelationships.ContainsKey(Convert.ToInt32(relationship["id"])))
                        {
                            continue;
                        }

                        _playerRelationships.Add(Convert.ToInt32(relationship["target"]),
                            new PlayerRelationship(Convert.ToInt32(relationship["id"]),
                                Convert.ToInt32(relationship["target"]),
                                (PlayerRelationshipType)Convert.ToInt32(relationship["type"].ToString())));
                    }
                }
                catch (Exception exception)
                {
                    var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                    _logManager.Log(exception.StackTrace, LogType.Error);
                }
                finally
                {
                    mysqlConnection.CloseConnection();
                }
            }
        }

        public PermissionManagement GetPermissionManagement()
        {
            return _permissionManagement;
        }

        public EffectManagement GetEffectManagement()
        {
            return _effectManagement;
        }

        public ClothingManagement GetClothingManagement()
        {
            return _clothingManagement;
        }

        public PlayerMessenger GetMessenger()
        {
            return _playerMessenger;
        }

        public BadgeManagement GetBadgeManagement()
        {
            return _badgeManagement;
        }
    }
}