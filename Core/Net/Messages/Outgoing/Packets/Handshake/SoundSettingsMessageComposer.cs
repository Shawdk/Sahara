using Sahara.Core.Packets.Outgoing;
using Sahara.Core.Packets.Server;
using System.Collections.Generic;

namespace Sahara.Core.Net.Messages.Outgoing.Packets.Handshake
{
    internal class SoundSettingsMessageComposer : OutgoingPacket
    {
        public SoundSettingsMessageComposer(IEnumerable<int> clientVolumeSettings, bool chatPreference, bool invitationStatus, bool focusPreference, int friendshipBarState) : base(OutgoingHeaders.SoundSettingsMessageComposer)
        {
            foreach (var value in clientVolumeSettings)
            {
                base.WriteInteger(value);
            }

            base.WriteBoolean(chatPreference);
            base.WriteBoolean(invitationStatus);
            base.WriteBoolean(focusPreference);
            base.WriteInteger(friendshipBarState);
            base.WriteInteger(0);
            base.WriteInteger(0);
        }
    }
}
