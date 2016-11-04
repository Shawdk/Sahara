using System.Collections.Generic;

namespace Sahara.Base.Game.Achievements
{
    internal class PlayerAchievement
    {
        private readonly string _achievementGroup;
        private int _achievementLevel;
        private int _achievementProgress;

        public PlayerAchievement(string achievementGroup, int achievementLevel, int achievementProgress)
        {
            _achievementGroup = achievementGroup;
            _achievementLevel = achievementLevel;
            _achievementProgress = achievementProgress;
        }
    }
}
