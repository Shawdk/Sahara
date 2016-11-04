using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sahara.Core.Logging;

namespace Sahara.Base.Game.Achievements
{
    internal class AchievementManager
    {
        private Dictionary<string, Achievement> _achievements;

        public AchievementManager()
        {
            var stopwatch = Stopwatch.StartNew();

            this._achievements = new Dictionary<string, Achievement>();
            AchievementLoader.TryGetAllAchievements(out _achievements);

            stopwatch.Stop();
            Sahara.GetServer().GetLogManager().Log("Loaded Achievement Manager [" + stopwatch.ElapsedMilliseconds + "ms]", LogType.Information);
        }

        public Dictionary<string, Achievement> Achievements => _achievements;
    }
}
