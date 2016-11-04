using Sahara.Base.Game.Players;

namespace Sahara.Core.Packets.Client.Packets.Handshake
{
    internal class SsoTicketMessageEvent : IPacket
    {
        public void Parse(Player player, IncomingPacket IncomingPacket)
        {
            if (player?.Arc4 == null)
            {
                return;
            }

            player.OnAuthentication(IncomingPacket.GetString());
        }
    }
}
