using Sahara.Core.Packets.Outgoing;

namespace Sahara.Core.Packets.Server.Packets.Handshake
{
    class InitCryptoMessageComposer : OutgoingPacket
    {
        public InitCryptoMessageComposer(string primeString, string generatorString)
            : base(OutgoingHeaders.InitCryptoMessageComposer)
        {
            base.WriteString(primeString);
            base.WriteString(generatorString);
        }
    }
}
