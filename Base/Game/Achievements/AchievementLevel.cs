using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Base.Game.Achievements
{
    public class AchievementLevel
    {
        private readonly int _levelId;
        private readonly int _levelRequirement;
        private readonly int _levelRewardPixels;
        private readonly int _levelRewardPoints;

        public AchievementLevel(int levelId, int levelRequirement, int levelRewardPixels, int levelRewardPoints)
        {
            _levelId = levelId;
            _levelRequirement = levelRequirement;
            _levelRewardPixels = levelRewardPixels;
            _levelRewardPoints = levelRewardPoints;
        }

        public int LevelId => _levelId;
        public int LevelRequirement => _levelRequirement;
        public int LevelRewardPixels => _levelRewardPixels;
        public int LevelRewardPoints => _levelRewardPoints;
    }
}
