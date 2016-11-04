using Sahara.Core.Packets.Outgoing;
using Sahara.Core.Packets.Server;

namespace Sahara.Core.Net.Messages.Outgoing.Packets.Handshake
{
    internal class AchievementScoreMessageComposer : OutgoingPacket
    {
        public AchievementScoreMessageComposer(int achievementScore) : base(OutgoingHeaders.AchievementScoreMessageComposer)
        {
            base.WriteInteger(achievementScore);
        }
    }
}
