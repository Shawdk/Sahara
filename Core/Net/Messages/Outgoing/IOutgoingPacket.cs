using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core.Packets.Server
{
    internal interface IOutgoingPacket
    {
        byte[] GetBytes();
    }
}
