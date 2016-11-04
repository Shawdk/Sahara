using Sahara.Base.Game.Achievements;
using Sahara.Core.Packets.Outgoing;
using Sahara.Core.Packets.Server;
using System.Collections.Generic;

namespace Sahara.Core.Net.Messages.Outgoing.Packets.Handshake
{
    internal class BadgeDefinitionsMessageComposer : OutgoingPacket
    {
        public BadgeDefinitionsMessageComposer(Dictionary<string, Achievement> achievements) : base(OutgoingHeaders.BadgeDefinitionsMessageComposer)
        {
            base.WriteInteger(achievements.Count);

            foreach (var achievement in achievements.Values)
            {
                base.WriteString(achievement.AchievementGroupName.Replace("ACH_", ""));
                base.WriteInteger(achievement.AchievementLevels.Count);

                foreach (var achievementLevel in achievement.AchievementLevels.Values)
                {
                    base.WriteInteger(achievementLevel.LevelId);
                    base.WriteInteger(achievementLevel.LevelRequirement);
                }
            }
        }
    }
}
