using System;
using System.Collections.Concurrent;
using Sahara.Core.Logging;

namespace Sahara.Base.Game.Players
{
    internal static class PlayerLoader
    {
        private static readonly ConcurrentDictionary<int, PlayerData> CachedPlayerData = new ConcurrentDictionary<int, PlayerData>();

        public static bool TryGetDataById(int playerId, out PlayerData playerData)
        {
            if (CachedPlayerData.ContainsKey(playerId))
            {
                return CachedPlayerData.TryGetValue(playerId, out playerData);
            }

            playerData = null;
            return false;
        }

        public static bool TryGetData(Player player, out PlayerData playerData)
        {
            try
            {
                using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
                {
                    mysqlConnection.OpenConnection();

                    mysqlConnection.SetQuery("SELECT * FROM `users` WHERE `id` = '1' LIMIT 1");
                    //mysqlConnection.AddParameter("ticket", 1);

                    var playerDataRow = mysqlConnection.GetRow();

                    if (playerDataRow == null)
                    {
                        Sahara.GetServer().GetLogManager().Log(player.AuthTicket + "", LogType.Warning);
                        playerData = null;
                        return false;
                    }

                    var p = new PlayerData(player, 1, "", "", "", 0, 0, 0, "100,100,100", true, true, true, 1);

                    playerData = p;

                    if (CachedPlayerData.ContainsKey(Convert.ToInt32(playerDataRow["id"])))
                    {
                        PlayerData tempData = null;
                        CachedPlayerData.TryRemove(Convert.ToInt32(playerDataRow["id"]), out tempData);
                        tempData = null;
                    }

                    CachedPlayerData.TryAdd(Convert.ToInt32(playerDataRow["id"]), playerData);

                    mysqlConnection.CloseConnection();
                }

                return true;
            }
            catch (Exception exception)
            {
                var logManager = Sahara.GetServer().GetLogManager();
                var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                logManager.Log($"Error in {method}: {exception.Message}", LogType.Warning);
                logManager.Log(exception.StackTrace, LogType.Warning);

                playerData = null;
                return false;
            }
        }
    }
}
