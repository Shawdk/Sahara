using Plus.Communication.Encryption.Crypto.Prng;
using Sahara.Base.Game.Permissions;
using Sahara.Core.Logging;
using Sahara.Core.Net.Messages.Outgoing.Packets.Handshake;
using Sahara.Core.Packets.Server;
using System;
using System.Net.Sockets;
using System.Text;
using Sahara.Base.Game.Subscriptions;

namespace Sahara.Base.Game.Players
{
    internal class Player : IDisposable
    {
        private readonly Socket _playerSocket;
        private readonly PlayerPacketHandler _playerPacketHandler;
        private readonly byte[] _buffer;
        private readonly LogManager _logManager;
        private PlayerData _playerData;
        private bool _disconnectionCalled;
        private string _playerMachineId;
        private ARC4 _arc4;

        public Player(Socket playerSocket)
        {
            _playerSocket = playerSocket;
            _playerPacketHandler = new PlayerPacketHandler(this);
            _buffer = new byte[8000];
            _logManager = Sahara.GetServer().GetLogManager();
        }

        public void StartPacketProcessing()
        {
            try
            {
                _playerSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, IncomingPacket, _playerSocket);
            }
            catch (SocketException)
            {
                Dispose();
            }
        }

        public void OnAuthentication(string authenticationTicket)
        {
            AuthTicket = authenticationTicket;

            PlayerData playerData;

            if (!PlayerLoader.TryGetData(this, out playerData))
            {
                Dispose();
                return;
            }

            _playerData = playerData;

            //todo: code ban check

            _playerData.InitializeData();

            SendMessage(new AuthenticationOkMessageComposer());
            SendMessage(new AvatarEffectsMessageComposer(_playerData.GetEffectManagement().GetAllEffects));
            SendMessage(new NavigatorSettingsMessageComposer(_playerData.HomeRoom));
            SendMessage(new FavouriteRoomsMessageComposer(_playerData.FavouriteRoomIds));
            SendMessage(new FigureSetIdsMessageComposer(_playerData.GetClothingManagement().ClothingParts));
            SendMessage(new UserRightsMessageComposer(_playerData.Rank));
            SendMessage(new AvailabilityStatusMessageComposer());
            SendMessage(new AchievementScoreMessageComposer(_playerData.AchievementScore));
            SendMessage(new BuildersClubMembershipMessageComposer());
            SendMessage(new CfhTopicsInitMessageComposer());
            SendMessage(new BadgeDefinitionsMessageComposer(Sahara.GetServer().GetGameManager().GetAchievementManager().Achievements));
            SendMessage(new SoundSettingsMessageComposer(_playerData.ClientVolumes, _playerData.ChatPreference, _playerData.AllowMessengerInvites, _playerData.FocusPreference, (int)_playerData.GetMessenger().MessengerBarState));

            if (!string.IsNullOrEmpty(_playerMachineId) && _playerData.MachineId != _playerMachineId)
            {
                using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
                {
                    mysqlConnection.OpenConnection();
                    mysqlConnection.SetQuery("UPDATE `users` SET `machine_id` = @machineId WHERE `id` = @id LIMIT 1");
                    mysqlConnection.AddParameter("machineId", _playerMachineId);
                    mysqlConnection.AddParameter("id", _playerData.PlayerId);
                    mysqlConnection.RunQuery();
                    mysqlConnection.CloseConnection();
                }

                _playerData.MachineId = _playerMachineId;
            }

            PermissionGroup permissionGroup;

            if (Sahara.GetServer().GetGameManager().GetPermissionManager().TryGetPermissionGroup(_playerData.Rank, out permissionGroup))
            {
                if (!string.IsNullOrEmpty(permissionGroup.PermissionBadge) && !_playerData.GetBadgeManagement().HasBadge(permissionGroup.PermissionBadge))
                {
                    _playerData.GetBadgeManagement().GiveBadge(permissionGroup.PermissionBadge, true);
                }
            }

            SubscriptionInformation subscriptionInformation = null;

            if (Sahara.GetServer().GetGameManager().GetSubscriptionManager().TryGetSubscriptionInformation(_playerData.VipRank, out subscriptionInformation))
            {
                if (!string.IsNullOrEmpty(subscriptionInformation.Badge) && !_playerData.GetBadgeManagement().HasBadge(subscriptionInformation.Badge))
                {
                    _playerData.GetBadgeManagement().GiveBadge(subscriptionInformation.Badge, true);
                }
            }

            if (_playerData.GetPermissionManagement().HasPermission("mod_tickets"))
            {
            }
        }

        private void IncomingPacket(IAsyncResult iAr)
        {
            try
            {
                var bytesReceived = _playerSocket.EndReceive(iAr);

                if (bytesReceived == 0)
                {
                    Dispose();
                    return;
                }

                var packet = new byte[bytesReceived];
                Array.Copy(_buffer, packet, bytesReceived);
                _playerPacketHandler.ProcessPacketData(packet);
            }
            catch
            {
                Dispose();
            }
            finally
            {
                try
                {
                    _playerSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, IncomingPacket, _playerSocket);
                }
                catch
                {
                    Dispose();
                }
            }
        }

        public ARC4 Arc4
        {
            get { return _arc4; }
            set { _arc4 = value; }
        }

        public string MachineId
        {
            get { return _playerMachineId; }
            set { _playerMachineId = value; }
        }

        public string AuthTicket { get; private set; }

        public void SendString(string data)
        {
            SendData(Encoding.UTF8.GetBytes(data));
        }

        private void SendData(byte[] data)
        {
            try
            {
                _playerSocket.BeginSend(data, 0, data.Length, 0, OnSend, null);
            }
            catch (SocketException socketException)
            {
                var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logManager.Log("Error in " + method + ": " + socketException.Message, LogType.Error);
                _logManager.Log(socketException.StackTrace, LogType.Error);

                Dispose();
            }
        }

        public void SendMessage(IOutgoingPacket outgoingPacket)
        {
            var packetBytes = outgoingPacket.GetBytes();
            SendData(packetBytes);
        }

        private void OnSend(IAsyncResult asyncResult)
        {
            try
            {
                if (_playerSocket == null)
                {
                    return;
                }

                _playerSocket.EndSend(asyncResult);
            }
            catch (SocketException)
            {
                Dispose();
            }
        }

        public PlayerData GetPlayerData()
        {
            return _playerData;
        }

        public void Dispose()
        {
            try
            {
                if (_disconnectionCalled)
                {
                    return;
                }

                _disconnectionCalled = true;

                if (_playerSocket == null || !_playerSocket.Connected)
                {
                    return;
                }

                _playerSocket.Shutdown(SocketShutdown.Both);
                _playerSocket.Close();
                _playerSocket.Dispose();
            }
            catch (Exception exception)
            {
                var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                _logManager.Log(exception.StackTrace, LogType.Error);
            }
        }
    }
}
