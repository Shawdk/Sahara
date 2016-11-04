using Sahara.Core.Packets.Outgoing;
using Sahara.Core.Packets.Server;

namespace Sahara.Core.Net.Messages.Outgoing.Packets.Handshake
{
    internal class FurniListNotificationMessageComposer : OutgoingPacket
    {
        public FurniListNotificationMessageComposer(int id, int type)
            : base(OutgoingHeaders.FurniListNotificationMessageComposer)
        {
            base.WriteInteger(1);
            base.WriteInteger(type);
            base.WriteInteger(1);
            base.WriteInteger(id);
        }
    }
}
