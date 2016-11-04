using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sahara.Base.Game.Players;

namespace Sahara.Core.Packets.Client
{
    internal interface IPacket
    {
        void Parse(Player player, IncomingPacket IncomingPacket);
    }
}
