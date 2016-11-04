using Sahara.Core.Logging;
using Sahara.Core.Packets.Client;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Sahara.Base.Game.Players
{
    internal class PlayerPacketHandler
    {
        private readonly Player _player;
        private readonly LogManager _logManager;
        private bool _decryptedData;
        private byte[] _dataReceived;
        private bool _halfDataRecieved;

        public PlayerPacketHandler(Player player)
        {
            _player = player;
            _logManager = Sahara.GetServer().GetLogManager();
        }

        public void ProcessPacketData(byte[] data)
        {
            try
            {
                if (data[0] == 60)
                {
                    const string crossDomainPolicy = "<?xml version=\"1.0\"?>\r\n"
                                                     +
                                                     "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n"
                                                     + "<cross-domain-policy>\r\n"
                                                     +
                                                     "<site-control permitted-cross-domain-policies=\"master-only\"/>\r\n"
                                                     + "<allow-access-from domain=\"*\" to-ports=\"*\" />\r\n"
                                                     + "</cross-domain-policy>\x0";

                    _player.SendString(crossDomainPolicy);

                }
                else if (data[0] != 67)
                {
                }

                ////////

                if (_player.Arc4 != null && !_decryptedData)
                {
                    _player.Arc4.Decrypt(ref data);
                    _decryptedData = true;
                }

                if (_halfDataRecieved)
                {
                    var fullDataRcv = new byte[_dataReceived.Length + data.Length];
                    Buffer.BlockCopy(_dataReceived, 0, fullDataRcv, 0, _dataReceived.Length);
                    Buffer.BlockCopy(data, 0, fullDataRcv, _dataReceived.Length, data.Length);

                    _halfDataRecieved = false;
                    ProcessPacketData(fullDataRcv);
                    return;
                }

                if (data[0] == 60)
                {
                    const string crossDomainPolicy = "<?xml version=\"1.0\"?>\r\n"
                                                     +
                                                     "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n"
                                                     + "<cross-domain-policy>\r\n"
                                                     +
                                                     "<site-control permitted-cross-domain-policies=\"master-only\"/>\r\n"
                                                     + "<allow-access-from domain=\"*\" to-ports=\"*\" />\r\n"
                                                     + "</cross-domain-policy>\x0";

                    _player.SendString(crossDomainPolicy);
                    
                }
                else if (data[0] != 67)
                {
                    using (var reader = new BinaryReader(new MemoryStream(data)))
                    {
                        if (data.Length < 4)
                            return;

                        var messageLength = Utility.Utility.DecodeInt32(reader.ReadBytes(4));

                        if ((reader.BaseStream.Length - 4) < messageLength)
                        {
                            _dataReceived = data;
                            _halfDataRecieved = true;
                            return;
                        }

                        if (messageLength < 0 || messageLength > 5120)
                            return;

                        var packet = reader.ReadBytes(messageLength);

                        using (var binaryReader = new BinaryReader(new MemoryStream(packet)))
                        {
                            var packetHeader = Utility.Utility.DecodeInt16(binaryReader.ReadBytes(2));

                            var packetContentBytes = new byte[packet.Length - 2];
                            Buffer.BlockCopy(packet, 2, packetContentBytes, 0, packet.Length - 2);

                            var IncomingPacket = new IncomingPacket(packetHeader, packetContentBytes);
                            Sahara.GetServer().GetPacketManager().ProcessPacket(_player, IncomingPacket);

                            _decryptedData = false;
                        }

                        if (reader.BaseStream.Length - 4 <= messageLength)
                        {
                            return;
                        }

                        var extra = new byte[reader.BaseStream.Length - reader.BaseStream.Position];
                        Buffer.BlockCopy(data, (int)reader.BaseStream.Position, extra, 0, (int)(reader.BaseStream.Length - reader.BaseStream.Position));

                        _decryptedData = true;
                        ProcessPacketData(extra);
                    }
                }
            }
            catch (Exception exception)
            {
                var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                _logManager.Log(exception.StackTrace, LogType.Error);

                _player.Dispose();
            }
        }
    }
}
