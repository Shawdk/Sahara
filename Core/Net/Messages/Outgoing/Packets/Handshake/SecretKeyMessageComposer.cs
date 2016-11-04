using Sahara.Core.Packets.Outgoing;

namespace Sahara.Core.Packets.Server.Packets.Handshake
{
    class SecretKeyMessageComposer : OutgoingPacket
    {
        public SecretKeyMessageComposer(string publicKey)
            : base(OutgoingHeaders.SecretKeyMessageComposer)
        {
            base.WriteString(publicKey);
        }
    }
}
