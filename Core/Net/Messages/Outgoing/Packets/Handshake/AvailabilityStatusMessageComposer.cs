using Sahara.Core.Packets.Outgoing;
using Sahara.Core.Packets.Server;

namespace Sahara.Core.Net.Messages.Outgoing.Packets.Handshake
{
    internal class AvailabilityStatusMessageComposer : OutgoingPacket
    {
        public AvailabilityStatusMessageComposer() : base(OutgoingHeaders.AvailabilityStatusMessageComposer)
        {
            base.WriteBoolean(true);
            base.WriteBoolean(false);
        }
    }
}
