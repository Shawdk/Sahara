using Sahara.Base.Game.Players.Clothing;
using Sahara.Core.Packets.Outgoing;
using Sahara.Core.Packets.Server;
using System.Collections.Generic;

namespace Sahara.Core.Net.Messages.Outgoing.Packets.Handshake
{
    internal class FigureSetIdsMessageComposer : OutgoingPacket
    {
        public FigureSetIdsMessageComposer(ICollection<ClothingParts> clothingParts) : base(OutgoingHeaders.FigureSetIdsMessageComposer)
        {
            base.WriteInteger(clothingParts.Count);

            foreach (var clothingPart in clothingParts)
            {
                base.WriteInteger(clothingPart.PartId);
            }

            base.WriteInteger(clothingParts.Count);

            foreach (var clothingPart in clothingParts)
            {
                base.WriteString(clothingPart.Parts);
            }
        }
    }
}
