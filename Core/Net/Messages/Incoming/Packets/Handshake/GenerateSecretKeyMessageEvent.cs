using Plus.Communication.Encryption;
using Plus.Communication.Encryption.Crypto.Prng;
using Sahara.Base.Game.Players;
using Sahara.Core.Packets.Server.Packets.Handshake;

namespace Sahara.Core.Packets.Client.Packets.Handshake
{
    internal class GenerateSecretKeyMessageEvent : IPacket
    {
        public void Parse(Player player, IncomingPacket IncomingPacket)
        {
            var cipherPublickey = IncomingPacket.GetString();
            var sharedKey = HabboEncryptionV2.CalculateDiffieHellmanSharedKey(cipherPublickey);

            if (sharedKey == 0)
            {
                return;
            }

            player.Arc4 = new ARC4(sharedKey.getBytes());
            player.SendMessage(new SecretKeyMessageComposer(HabboEncryptionV2.GetRsaDiffieHellmanPublicKey()));
        }
    }
}
