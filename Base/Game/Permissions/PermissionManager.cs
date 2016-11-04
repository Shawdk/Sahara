using Sahara.Core.Database;
using Sahara.Core.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security;
using Sahara.Base.Game.Players;

namespace Sahara.Base.Game.Permissions
{
    internal class PermissionManager
    {
        private readonly Dictionary<int, Permission> _permissions;
        private readonly Dictionary<string, PermissionCommand> _permissionCommands;
        private readonly Dictionary<int, PermissionGroup> _permissionGroups;
        private readonly Dictionary<int, List<string>> _permissionGroupRights;
        private readonly Dictionary<int, List<string>> _permissionSubscriptionRights;
        private readonly LogManager _logManager;

        public PermissionManager()
        {
            _permissions = new Dictionary<int, Permission>();
            _permissionCommands = new Dictionary<string, PermissionCommand>();
            _permissionGroups = new Dictionary<int, PermissionGroup>();
            _permissionGroupRights = new Dictionary<int, List<string>>();
            _permissionSubscriptionRights = new Dictionary<int, List<string>>();
            _logManager = Sahara.GetServer().GetLogManager();

            InitializePermissions();
        }

        private void InitializePermissions()
        {
            using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
            {
                mysqlConnection.OpenConnection();

                LoadPermissions(mysqlConnection);
                LoadCommandPermissions(mysqlConnection);
                LoadPermissionGroups(mysqlConnection);
                LoadPermissionRights(mysqlConnection);
                LoadPermissionSubscriptions(mysqlConnection);

                mysqlConnection.CloseConnection();
            }
        }

        private void LoadPermissions(DatabaseConnection mysqlConnection)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                mysqlConnection.SetQuery("SELECT * FROM `permissions`");
                var permissionTable = mysqlConnection.GetTable();

                if (permissionTable != null)
                {
                    foreach (DataRow permissionRow in permissionTable.Rows)
                    {
                        _permissions.Add(Convert.ToInt32(permissionRow["id"]), new Permission(Convert.ToInt32(permissionRow["id"]), Convert.ToString(permissionRow["permission"])));
                    }
                }

                stopwatch.Stop();
                _logManager.Log($"Loaded {_permissions.Count} permissions [{stopwatch.ElapsedMilliseconds}ms]", LogType.Information);
            }
            catch (Exception exception)
            {
                var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                _logManager.Log(exception.StackTrace, LogType.Error);
            }
        }

        private void LoadCommandPermissions(DatabaseConnection mysqlConnection)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                mysqlConnection.SetQuery("SELECT * FROM `permissions_commands`");
                var permissionTable = mysqlConnection.GetTable();

                if (permissionTable != null)
                {
                    foreach (DataRow permissionRow in permissionTable.Rows)
                    {
                        _permissionCommands.Add(Convert.ToString(permissionRow["command"]), new PermissionCommand(Convert.ToString(permissionRow["command"]), Convert.ToInt32(permissionRow["group_id"]), Convert.ToInt32(permissionRow["subscription_id"])));
                    }
                }

                stopwatch.Stop();
                _logManager.Log($"Loaded {_permissionCommands.Count} command permissions [{stopwatch.ElapsedMilliseconds}ms]", LogType.Information);
            }
            catch (Exception exception)
            {
                var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                _logManager.Log(exception.StackTrace, LogType.Error);
            }
        }

        private void LoadPermissionGroups(DatabaseConnection mysqlConnection)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                mysqlConnection.SetQuery("SELECT * FROM `permissions_groups`");
                var permissionGroupsTable = mysqlConnection.GetTable();

                if (permissionGroupsTable != null)
                {
                    foreach (DataRow permissionRow in permissionGroupsTable.Rows)
                    {
                        _permissionGroups.Add(Convert.ToInt32(permissionRow["id"]), new PermissionGroup(Convert.ToString(permissionRow["name"]), Convert.ToString(permissionRow["badge_code"])));
                    }
                }

                stopwatch.Stop();
                _logManager.Log($"Loaded {_permissionGroups.Count} permission groups [{stopwatch.ElapsedMilliseconds}ms]", LogType.Information);
            }
            catch (Exception exception)
            {
                var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                _logManager.Log(exception.StackTrace, LogType.Error);
            }
        }

        private void LoadPermissionRights(DatabaseConnection mysqlConnection)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                mysqlConnection.SetQuery("SELECT * FROM `permissions_rights`");
                var permissionRightsTable = mysqlConnection.GetTable();

                if (permissionRightsTable != null)
                {
                    foreach (DataRow permissionRow in permissionRightsTable.Rows)
                    {
                        var groupId = Convert.ToInt32(permissionRow["group_id"]);

                        if (!_permissionGroups.ContainsKey(groupId))
                        {
                            continue;
                        }

                        Permission newPermission = null;
                        var permissionId = Convert.ToInt32(permissionRow["permission_id"]);

                        if (!_permissions.TryGetValue(permissionId, out newPermission))
                        {
                            continue;
                        }

                        if (_permissionGroupRights.ContainsKey(groupId))
                        {
                            _permissionGroupRights[groupId].Add(newPermission.PermissionName);
                        }
                        else
                        {
                            var permissionRightSets = new List<string> { newPermission.PermissionName };
                            _permissionGroupRights.Add(groupId, permissionRightSets);
                        }
                    }
                }
                
                stopwatch.Stop();
                _logManager.Log($"Loaded {_permissionGroupRights.Count} permission group rights [{stopwatch.ElapsedMilliseconds}ms]", LogType.Information);
            }
            catch (Exception exception)
            {
                var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                _logManager.Log(exception.StackTrace, LogType.Error);
            }
        }

        private void LoadPermissionSubscriptions(DatabaseConnection mysqlConnection)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                mysqlConnection.SetQuery("SELECT * FROM `permissions_subscriptions`");
                var permissionRightsTable = mysqlConnection.GetTable();

                if (permissionRightsTable != null)
                {
                    foreach (DataRow permissionRow in permissionRightsTable.Rows)
                    {
                        Permission newPermission = null;
                        var permissionId = Convert.ToInt32(permissionRow["permission_id"]);

                        if (!_permissions.TryGetValue(permissionId, out newPermission))
                        {
                            continue;
                        }

                        var subscriptionId = Convert.ToInt32(permissionRow["subscription_id"]);

                        if (_permissionSubscriptionRights.ContainsKey(subscriptionId))
                        {
                            _permissionSubscriptionRights[subscriptionId].Add(newPermission.PermissionName);
                        }
                        else
                        {
                            var permissionRightSets = new List<string> { newPermission.PermissionName };
                            _permissionSubscriptionRights.Add(subscriptionId, permissionRightSets);
                        }
                    }
                }

                stopwatch.Stop();
                _logManager.Log($"Loaded {_permissionSubscriptionRights.Count} permission subscription rights [{stopwatch.ElapsedMilliseconds}ms]", LogType.Information);
            }
            catch (Exception exception)
            {
                var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                _logManager.Log(exception.StackTrace, LogType.Error);
            }
        }

        public bool TryGetPermissionGroup(int id, out PermissionGroup permissionGroup)
        {
            return _permissionGroups.TryGetValue(id, out permissionGroup);
        }

        public IEnumerable<string> GetPermissionsForPlayer(PlayerData playerData)
        {
            var permissionSets = new List<string>();

            List<string> permissionRights = null;

            if (_permissionGroupRights.TryGetValue(playerData.Rank, out permissionRights))
            {
                permissionSets.AddRange(permissionRights);
            }

            List<string> subscriptionRights = null;

            if (_permissionSubscriptionRights.TryGetValue(playerData.VipRank, out subscriptionRights))
            {
                permissionSets.AddRange(subscriptionRights);
            }

            return permissionSets;
        }

        public IEnumerable<string> GetCommandsForPlayer(PlayerData playerData)
        {
            return _permissionCommands.Where(x => playerData.Rank >= x.Value.GroupId && playerData.VipRank >= x.Value.SubscriptionId).Select(x => x.Key).ToList();
        }
    }
}
