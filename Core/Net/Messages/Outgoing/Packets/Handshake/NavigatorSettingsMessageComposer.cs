using Sahara.Core.Packets.Outgoing;
using Sahara.Core.Packets.Server;

namespace Sahara.Core.Net.Messages.Outgoing.Packets.Handshake
{
    internal class NavigatorSettingsMessageComposer : OutgoingPacket
    {
        public NavigatorSettingsMessageComposer(int homeRoom)
            : base(OutgoingHeaders.NavigatorSettingsMessageComposer)
        {
            base.WriteInteger(homeRoom);
            base.WriteInteger(homeRoom);
        }
    }
}
