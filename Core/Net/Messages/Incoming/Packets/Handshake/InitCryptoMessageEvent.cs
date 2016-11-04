using Plus.Communication.Encryption;
using Sahara.Base.Game.Players;
using Sahara.Core.Packets.Server.Packets.Handshake;

namespace Sahara.Core.Packets.Client.Packets.Handshake
{
    internal class InitCryptoMessageEvent : IPacket
    {
        public void Parse(Player player, IncomingPacket IncomingPacket)
        {
            player.SendMessage(new InitCryptoMessageComposer(HabboEncryptionV2.GetRsaDiffieHellmanPrimeKey(), HabboEncryptionV2.GetRsaDiffieHellmanGeneratorKey()));
        }
    }
}
