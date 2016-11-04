using Sahara.Base.Game.Players;
using Sahara.Core.Logging;
using Sahara.Core.Packets.Client;
using Sahara.Core.Packets.Client.Packets.Handshake;
using System.Collections.Generic;
using Sahara.Core.Net.Messages.Incoming.Packets.Catalog;

namespace Sahara.Core.Packets
{
    class PacketManager
    {
        private readonly Dictionary<int, IPacket> _IncomingPackets;
        private readonly LogManager _logManager;

        public PacketManager()
        {
            _IncomingPackets = new Dictionary<int, IPacket>();
            _logManager = Sahara.GetServer().GetLogManager();

            AddPackets();
        }

        public void ProcessPacket(Player player, IncomingPacket IncomingPacket)
        {
            IPacket packet;

            if (_IncomingPackets.TryGetValue(IncomingPacket.PacketHeader, out packet))
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    _logManager.Log("Handled Packet: " + IncomingPacket.PacketHeader, LogType.Information);
                }

                packet.Parse(player, IncomingPacket);
                return;
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                _logManager.Log("Unhandled Packet: " + IncomingPacket.PacketHeader, LogType.Warning);
            }
        }

        private void AddPackets()
        {
            _IncomingPackets.Add(IncomingHeaders.GetClientVersionMessageEvent, new GetClientVersionMessageEvent());
            _IncomingPackets.Add(IncomingHeaders.InitCryptoMessageEvent, new InitCryptoMessageEvent());
            _IncomingPackets.Add(IncomingHeaders.GenerateSecretKeyMessageEvent, new GenerateSecretKeyMessageEvent());
            _IncomingPackets.Add(IncomingHeaders.ClientVariablesMessageEvent, new ClientVariablesMessageEvent());
            _IncomingPackets.Add(IncomingHeaders.SsoTicketMessageEvent, new SsoTicketMessageEvent());
            _IncomingPackets.Add(IncomingHeaders.UniqueIdMessageEvent, new UniqueIdMessageEvent());
            _IncomingPackets.Add(IncomingHeaders.GetCatalogRoomPromotionMessageEvent, new GetCatalogRoomPromotionMessageEvent());
        }
    }
}
