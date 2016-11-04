using Sahara.Base.Game.Players;
using Sahara.Core.Packets.Server.Packets.Handshake;

namespace Sahara.Core.Packets.Client.Packets.Handshake
{
    internal class UniqueIdMessageEvent : IPacket
    {
        public void Parse(Player player, IncomingPacket incomingPacket)
        {
            var useless = incomingPacket.GetString();
            var machineId = incomingPacket.GetString();

            player.MachineId = machineId;
            player.SendMessage(new SetUniqueIdMessageComposer(machineId));
        }
    }
}
