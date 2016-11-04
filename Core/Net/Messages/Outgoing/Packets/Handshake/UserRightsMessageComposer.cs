using Sahara.Core.Packets.Outgoing;
using Sahara.Core.Packets.Server;

namespace Sahara.Core.Net.Messages.Outgoing.Packets.Handshake
{
    internal class UserRightsMessageComposer : OutgoingPacket
    {
        public UserRightsMessageComposer(int playerRank)
            : base(OutgoingHeaders.UserRightsMessageComposer)
        {
            base.WriteInteger(2);
            base.WriteInteger(playerRank);
            base.WriteBoolean(false);
        }
    }
}
