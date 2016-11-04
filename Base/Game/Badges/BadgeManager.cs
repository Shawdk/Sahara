using Sahara.Core.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Sahara.Base.Game.Badges
{
    internal class BadgeManager
    {
        private readonly Dictionary<string, BadgeDefinition> _badges;

        public BadgeManager()
        {
            _badges = new Dictionary<string, BadgeDefinition>();

            Initialize();
        }

        private void Initialize()
        {
            try
            {
                var logManager = Sahara.GetServer().GetLogManager();
                var stopwatch = Stopwatch.StartNew();

                using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
                {
                    mysqlConnection.OpenConnection();
                    mysqlConnection.SetQuery("SELECT * FROM `badge_definitions`");
                    var badgeTable = mysqlConnection.GetTable();

                    foreach (DataRow badge in badgeTable.Rows)
                    {
                        var badgeCode = Convert.ToString(badge["code"]).ToUpper();

                        if (!_badges.ContainsKey(badgeCode))
                        {
                            _badges.Add(badgeCode, new BadgeDefinition(badgeCode, Convert.ToString(badge["required_right"])));
                        }
                    }

                    mysqlConnection.CloseConnection();
                }

                stopwatch.Stop();
                logManager.Log("Loaded Badge Manager [" + stopwatch.ElapsedMilliseconds + "ms]", LogType.Information);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public bool TryGetBadge(string badgeCode, out BadgeDefinition badgeDefinition)
        {
            return this._badges.TryGetValue(badgeCode.ToUpper(), out badgeDefinition);
        }
    }
}
