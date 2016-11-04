using Sahara.Core.Packets.Outgoing;
using Sahara.Core.Packets.Server;
using System.Collections.Generic;

namespace Sahara.Core.Net.Messages.Outgoing.Packets.Handshake
{
    internal class FavouriteRoomsMessageComposer : OutgoingPacket
    {
        public FavouriteRoomsMessageComposer(IReadOnlyCollection<int> favouriteRooms)
            : base(OutgoingHeaders.FavouriteRoomsMessageComposer)
        {
            base.WriteInteger(50);
            base.WriteInteger(favouriteRooms.Count);

            foreach (var roomId in favouriteRooms)
            {
                base.WriteInteger(roomId);
            }
        }
    }
}
