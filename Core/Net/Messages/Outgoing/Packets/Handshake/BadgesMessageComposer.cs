using Sahara.Base.Game.Players;
using Sahara.Base.Game.Players.Badges;
using Sahara.Core.Packets.Outgoing;
using Sahara.Core.Packets.Server;
using System.Collections.Generic;

namespace Sahara.Core.Net.Messages.Outgoing.Packets.Handshake
{
    internal class BadgesMessageComposer : OutgoingPacket
    {
        public BadgesMessageComposer(Player player) : base(OutgoingHeaders.BadgesMessageComposer)
        {
            var badgesEquiped = new List<Badge>();

            base.WriteInteger(player.GetPlayerData().GetBadgeManagement().BadgeCount);

            foreach (var badge in player.GetPlayerData().GetBadgeManagement().Badges.Values)
            {
                base.WriteInteger(1);
                base.WriteString(badge.BadgeCode);

                if (badge.BadgeSlotId >= 1)
                {
                    badgesEquiped.Add(badge);
                }
            }

            base.WriteInteger(badgesEquiped.Count);

            foreach (var badge in badgesEquiped)
            {
                base.WriteInteger(badge.BadgeSlotId);
                base.WriteString(badge.BadgeCode);
            }
        }
    }
}
