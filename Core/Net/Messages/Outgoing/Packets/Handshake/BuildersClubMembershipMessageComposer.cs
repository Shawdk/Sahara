using Sahara.Core.Packets.Outgoing;
using Sahara.Core.Packets.Server;

namespace Sahara.Core.Net.Messages.Outgoing.Packets.Handshake
{
    internal class BuildersClubMembershipMessageComposer : OutgoingPacket
    {
        public BuildersClubMembershipMessageComposer() : base(OutgoingHeaders.BuildersClubMembershipMessageComposer)
        {
            base.WriteInteger(int.MaxValue);
            base.WriteInteger(100);
            base.WriteInteger(0);
            base.WriteInteger(int.MaxValue);
        }
    }
}
