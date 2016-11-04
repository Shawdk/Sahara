using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HabboEncryption;

namespace Sahara.Core.Packets.Client
{
    internal class IncomingPacket : IIncomingPacket
    {
        private readonly byte[] _packetData;
        private int _pointer;

        public IncomingPacket(int packetHeader, byte[] packetData)
        {
            PacketHeader = packetHeader;
            _packetData = packetData ?? new byte[0];
        }

        public int PacketHeader { get; }

        private int RemainingLength => _packetData.Length - _pointer;

        private byte[] ReadBytes(int bytes)
        {
            if (bytes > RemainingLength)
                bytes = RemainingLength;

            var data = new byte[bytes];

            for (var i = 0; i < bytes; i++)
                data[i] = _packetData[_pointer++];

            return data;
        }

        private byte[] ReadFixedValue()
        {
            var len = HabboEncoding.DecodeInt16(ReadBytes(2));
            return ReadBytes(len);
        }

        public string GetString()
        {
            return Encoding.Default.GetString(ReadFixedValue());
        }
    }
}
