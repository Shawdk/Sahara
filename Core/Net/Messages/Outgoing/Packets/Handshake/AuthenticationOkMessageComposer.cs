using Sahara.Core.Packets.Outgoing;
using Sahara.Core.Packets.Server;

namespace Sahara.Core.Net.Messages.Outgoing.Packets.Handshake
{
    internal class AuthenticationOkMessageComposer : OutgoingPacket
    {
        public AuthenticationOkMessageComposer()
            : base(OutgoingHeaders.AuthenticationOkMessageComposer)
        {
        }
    }
}
