using Sahara.Base.Utility;
using Sahara.Core.Database;
using Sahara.Core.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Sahara.Base.Game.Moderation
{
    internal class ModerationManager
    {
        private int _moderationTicketCount;
        private readonly List<string> _playerPresets;
        private readonly List<string> _roomPresets;
        private readonly Dictionary<string, ModerationBan> _moderationBans;
        private readonly Dictionary<int, string> _playerActionPresetCategories;
        private readonly Dictionary<int, List<ModerationPresetActionMessage>> _playerActionPresetMessages;
        private readonly ConcurrentDictionary<int, ModerationTicket> _moderationTickets;
        private readonly LogManager _logManager;

        public ModerationManager()
        {
            _moderationTicketCount = 1;
            _playerPresets = new List<string>();
            _roomPresets = new List<string>();
            _moderationBans = new Dictionary<string, ModerationBan>();
            _playerActionPresetCategories = new Dictionary<int, string>();
            _playerActionPresetMessages = new Dictionary<int, List<ModerationPresetActionMessage>>();
            _moderationTickets = new ConcurrentDictionary<int, ModerationTicket>();
            _logManager = new LogManager();

            LoadModeration();
        }

        private void LoadModeration()
        {
            using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
            {
                mysqlConnection.OpenConnection();

                LoadModerationPresets(mysqlConnection);
                LoadModerationCategorys(mysqlConnection);
                LoadModerationMessages(mysqlConnection);
                LoadModerationBans(mysqlConnection);

                mysqlConnection.CloseConnection();
            }
        }

        private void LoadModerationPresets(DatabaseConnection mysqlConnection)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                mysqlConnection.SetQuery("SELECT * FROM `moderation_presets`");
                var moderationPresetsTable = mysqlConnection.GetTable();

                if (moderationPresetsTable != null)
                {
                    foreach (DataRow presetRow in moderationPresetsTable.Rows)
                    {
                        var presetType = Convert.ToString(presetRow["type"]).ToLower();
                        var message = Convert.ToString(presetRow["message"]);

                        switch (presetType)
                        {
                            case "user":
                                _playerPresets.Add(message);
                                break;
                            default:
                                _roomPresets.Add(message);
                                break;
                        }
                    }
                }

                stopwatch.Stop();
                _logManager.Log(
                    $"Loaded {_playerPresets.Count + _roomPresets.Count} moderation presets [{stopwatch.ElapsedMilliseconds}ms]",
                    LogType.Information);
            }
            catch (Exception exception)
            {
                var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                _logManager.Log(exception.StackTrace, LogType.Error);
            }
        }

        private void LoadModerationCategorys(DatabaseConnection mysqlConnection)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                mysqlConnection.SetQuery("SELECT * FROM `moderation_preset_action_categories`");
                var moderationCategorysTable = mysqlConnection.GetTable();

                if (moderationCategorysTable != null)
                {
                    foreach (DataRow presetRow in moderationCategorysTable.Rows)
                    {
                        _playerActionPresetCategories.Add(Convert.ToInt32(presetRow["id"]), Convert.ToString(presetRow["caption"]));
                    }
                }

                stopwatch.Stop();
                _logManager.Log($"Loaded {_playerActionPresetCategories.Count} moderation categorys [{stopwatch.ElapsedMilliseconds}ms]", LogType.Information);
            }
            catch (Exception exception)
            {
                var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                _logManager.Log(exception.StackTrace, LogType.Error);
            }
        }

        private void LoadModerationMessages(DatabaseConnection mysqlConnection)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                mysqlConnection.SetQuery("SELECT * FROM `moderation_preset_action_messages`");
                var moderationMessagesTable = mysqlConnection.GetTable();

                if (moderationMessagesTable != null)
                {
                    foreach (DataRow presetRow in moderationMessagesTable.Rows)
                    {
                        var parentId = Convert.ToInt32(presetRow["parent_id"]);

                        if (!_playerActionPresetMessages.ContainsKey(parentId))
                        {
                            _playerActionPresetMessages.Add(parentId, new List<ModerationPresetActionMessage>());
                        }

                        _playerActionPresetMessages[parentId].Add(new ModerationPresetActionMessage(Convert.ToInt32(presetRow["id"]), Convert.ToInt32(presetRow["parent_id"]), Convert.ToString(presetRow["caption"]), Convert.ToString(presetRow["message_text"]), Convert.ToInt32(presetRow["mute_hours"]), Convert.ToInt32(presetRow["ban_hours"]), Convert.ToInt32(presetRow["ip_ban_hours"]), Convert.ToInt32(presetRow["trade_lock_days"]), Convert.ToString(presetRow["notice"])));
                    }
                }

                stopwatch.Stop();
                _logManager.Log($"Loaded {_playerActionPresetMessages.Count} moderation messages [{stopwatch.ElapsedMilliseconds}ms]", LogType.Information);
            }
            catch (Exception exception)
            {
                var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                _logManager.Log(exception.StackTrace, LogType.Error);
            }
        }

        private void LoadModerationBans(DatabaseConnection mysqlConnection)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                mysqlConnection.SetQuery("SELECT `bantype`, `value`, `reason`, `expire` FROM `bans` WHERE `bantype` = 'machine' OR `bantype` = 'user'");
                var moderationBanTable = mysqlConnection.GetTable();

                if (moderationBanTable != null)
                {
                    foreach (DataRow banRow in moderationBanTable.Rows)
                    {
                        var banValue = Convert.ToString(banRow["value"]);
                        var banReason = Convert.ToString(banRow["reason"]);
                        var banExpirationDate = (double)banRow["expire"];
                        var banType = Convert.ToString(banRow["bantype"]);
                        var banTypeValue = (banType == "ip" ? ModerationBanType.ByIp : banType == "machine" ? ModerationBanType.ByMachine : ModerationBanType.ByUsername);
                        var ban = new ModerationBan(banTypeValue, banValue, banReason, banExpirationDate);

                        if (banExpirationDate <= UnixTimestampGenerator.GetNow())
                        {
                            mysqlConnection.AddParameter("Key", banValue);
                            mysqlConnection.SetQuery("DELETE FROM `bans` WHERE `bantype` = '" + banType + "' AND `value` = @Key LIMIT 1");
                            mysqlConnection.RunQuery();
                        }
                        else if (!_moderationBans.ContainsKey(banValue))
                        {
                            _moderationBans.Add(banValue, ban);
                        }
                    }
                }

                stopwatch.Stop();
                _logManager.Log($"Loaded {_moderationBans.Count} moderation bans [{stopwatch.ElapsedMilliseconds}ms]", LogType.Information);
            }
            catch (Exception exception)
            {
                var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                _logManager.Log(exception.StackTrace, LogType.Error);
            }
        }

        public void AddBan(string moderatorUsername, ModerationBanType banType, string banValue, string banReason, double expireTimestamp)
        {
            var type = (banType == ModerationBanType.ByIp ? "ip" : banType == ModerationBanType.ByMachine ? "machine" : "user");

            using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
            {
                mysqlConnection.OpenConnection();
                mysqlConnection.AddParameter("banType", type);
                mysqlConnection.AddParameter("banValue", banValue);
                mysqlConnection.AddParameter("reason", banReason);
                mysqlConnection.AddParameter("expireTimestamp", expireTimestamp);
                mysqlConnection.AddParameter("moderatorUsername", moderatorUsername);
                mysqlConnection.AddParameter("addedTimestamp", UnixTimestampGenerator.GetNow());
                mysqlConnection.SetQuery("REPLACE INTO `bans` (`bantype`, `value`, `reason`, `expire`, `added_by`, `added_date`) VALUES (@banType, @banValue, @reason, @expireTimestamp, @moderatorUsername, @addedTimestamp)");
                mysqlConnection.RunQuery();
                mysqlConnection.CloseConnection();
            }

            if (banType != ModerationBanType.ByMachine && banType != ModerationBanType.ByUsername)
            {
                return;
            }

            if (!_moderationBans.ContainsKey(banValue))
            {
                _moderationBans.Add(banValue, new ModerationBan(banType, banValue, banReason, expireTimestamp));
            }
        }

        public ICollection<string> PlayerMessagePresets => _playerPresets;
        public ICollection<string> RoomMessagePresets => _roomPresets;
        public ICollection<ModerationTicket> GetTickets => _moderationTickets.Values;

        public Dictionary<string, List<ModerationPresetActionMessage>> PlayerActionPresets
        {
            get
            {
                var resultDictionary = new Dictionary<string, List<ModerationPresetActionMessage>>();

                foreach (var categoryKeyValuePair in this._playerActionPresetCategories.ToList())
                {
                    resultDictionary.Add(categoryKeyValuePair.Value, new List<ModerationPresetActionMessage>());

                    if (!this._playerActionPresetCategories.ContainsKey(categoryKeyValuePair.Key))
                    {
                        continue;
                    }

                    foreach (var data in this._playerActionPresetMessages[categoryKeyValuePair.Key])
                    {
                        resultDictionary[categoryKeyValuePair.Value].Add(data);
                    }
                }

                return resultDictionary;
            }
        }

        public bool TryAddModerationTicket(ModerationTicket moderationTicket)
        {
            moderationTicket.TicketId = _moderationTicketCount++;
            return _moderationTickets.TryAdd(moderationTicket.TicketId, moderationTicket);
        }

        public bool TryGetModerationTicket(int ticketId, out ModerationTicket ticket)
        {
            return _moderationTickets.TryGetValue(ticketId, out ticket);
        }

        private bool BanValueFound(string banValue, out ModerationBan moderationBan)
        {
            if (!_moderationBans.TryGetValue(banValue, out moderationBan))
            {
                moderationBan = null;
                return false;
            }

            if (!moderationBan.HasExpired)
            {
                return true;
            }

            using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
            {
                mysqlConnection.OpenConnection();
                mysqlConnection.AddParameter("type", moderationBan.BanType == ModerationBanType.ByIp ? "ip" : moderationBan.BanType == ModerationBanType.ByMachine ? "machine" : "username");
                mysqlConnection.AddParameter("value", banValue);
                mysqlConnection.SetQuery("DELETE FROM `bans` WHERE `bantype` = @type AND `value` = @value LIMIT 1");
                mysqlConnection.RunQuery();
                mysqlConnection.CloseConnection();
            }

            if (_moderationBans.ContainsKey(banValue))
            {
                _moderationBans.Remove(banValue);
            }

            return false;
        }

        public bool MachineBanFound(string machineId)
        {
            ModerationBan machineBan;

            if (!BanValueFound(machineId, out machineBan))
            {
                return true;
            }

            using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
            {
                mysqlConnection.OpenConnection();
                mysqlConnection.AddParameter("value", machineId);
                mysqlConnection.SetQuery("SELECT * FROM `bans` WHERE `bantype` = 'machine' AND `value` = @value LIMIT 1");
                var banRow = mysqlConnection.GetRow();

                if (banRow != null)
                {
                    return true;
                }
                
                mysqlConnection.CloseConnection();

                RemoveBan(machineId);
                return false;
            }
        }

        public bool UsernameBanFound(string username)
        {
            ModerationBan usernameBan;

            if (!BanValueFound(username, out usernameBan))
            {
                return true;
            }

            using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
            {
                mysqlConnection.AddParameter("value", username);
                mysqlConnection.SetQuery("SELECT * FROM `bans` WHERE `bantype` = 'user' AND `value` = @value LIMIT 1");
                var banRow = mysqlConnection.GetRow();

                if (banRow != null)
                {
                    return true;
                }

                RemoveBan(username);
                return false;
            }
        }

        private void RemoveBan(string banValue)
        {
            _moderationBans.Remove(banValue);
        }
    }
}



