using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Base.Game.Achievements
{
    public class Achievement
    {
        private readonly int _achievementId;
        private readonly string _achievementCategory;
        private readonly string _achievementGroupName;
        private readonly int _achievementGameId;
        private readonly Dictionary<int, AchievementLevel> _achievementLevels;

        public Achievement(int achievementId, string achievementCategory, string achievementGroupName, int achievementGameId)
        {
            _achievementId = achievementId;
            _achievementCategory = achievementCategory;
            _achievementGroupName = achievementGroupName;
            _achievementGameId = achievementGameId;
            _achievementLevels = new Dictionary<int, AchievementLevel>();
        }

        public void AddNewLevel(AchievementLevel newAchievementLevel)
        {
            _achievementLevels.Add(newAchievementLevel.LevelId, newAchievementLevel);
        }

        public int AchievementId => _achievementId;
        public string AchievementCategory => _achievementCategory;
        public string AchievementGroupName => _achievementGroupName;
        public int AchievementGameId => _achievementGameId;
        public Dictionary<int, AchievementLevel> AchievementLevels => _achievementLevels;
    }
}
