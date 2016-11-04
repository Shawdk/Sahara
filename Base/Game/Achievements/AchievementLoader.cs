using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Base.Game.Achievements
{
    public class AchievementLoader
    {
        public static bool TryGetAllAchievements(out Dictionary<string, Achievement> achievements)
        {
            try
            {
                achievements = new Dictionary<string, Achievement>();

                using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
                {
                    mysqlConnection.OpenConnection();
                    mysqlConnection.SetQuery("SELECT `id`, `category`, `group_name`, `level`, `reward_pixels`, `reward_points`, `progress_needed`, `game_id` FROM `achievements`");
                    var achievementTable = mysqlConnection.GetTable();

                    if (achievementTable == null)
                    {
                        achievements = null;
                        return false;
                    }

                    foreach (DataRow achievement in achievementTable.Rows)
                    {
                        var achievementId = Convert.ToInt32(achievement["id"]);
                        var achievementCategory = Convert.ToString(achievement["category"]);
                        var achievementGroupName = Convert.ToString(achievement["group_name"]);
                        var achievementLevelId = Convert.ToInt32(achievement["level"]);
                        var achievementLevelPixels = Convert.ToInt32(achievement["reward_pixels"]);
                        var achievementLevelPoints = Convert.ToInt32(achievement["reward_points"]);
                        var progressionRequires = Convert.ToInt32(achievement["progress_needed"]);
                        var achievementGameId = Convert.ToInt32(achievement["game_id"]);

                        var achievementLevel = new AchievementLevel(achievementLevelId, achievementLevelPixels, achievementLevelPoints, progressionRequires);

                        if (!achievements.ContainsKey(achievementGroupName))
                        {
                            var newAchievement = new Achievement(achievementId, achievementCategory, achievementGroupName, achievementGameId);
                            newAchievement.AddNewLevel(achievementLevel);
                            achievements.Add(achievementGroupName, newAchievement);
                        }
                        else
                        {
                            achievements[achievementGroupName].AddNewLevel(achievementLevel);
                        }
                    }

                    mysqlConnection.CloseConnection();
                }

                return true;
            }
            catch (Exception)
            {
                achievements = null;
                return false;
            }
        }
    }
}
