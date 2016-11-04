using Sahara.Core.Packets.Outgoing;

namespace Sahara.Core.Packets.Server.Packets.Handshake
{
    class SetUniqueIdMessageComposer : OutgoingPacket
    {
        public SetUniqueIdMessageComposer(string uniqueId)
            : base(OutgoingHeaders.SetUniqueIdMessageComposer)
        {
            base.WriteString(uniqueId);
        }
    }
}
