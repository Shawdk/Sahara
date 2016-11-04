using Sahara.Base.Game.Players;
using Sahara.Core.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Sahara.Core.Sockets
{
    class SocketManager
    {
        private readonly Socket _serverSocket;
        private readonly LogManager _logManager;

        public SocketManager()
        {
            var stopwatch = Stopwatch.StartNew();

            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _logManager = Sahara.GetServer().GetLogManager();

            Listen();

            stopwatch.Stop();
            _logManager.Log("Loaded Socket Manager [" + stopwatch.ElapsedMilliseconds + "ms]", LogType.Information);
        }

        private void Listen()
        {
            try
            {
                _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 3000));
                _serverSocket.Listen(50);
                _serverSocket.BeginAccept(OnAcceptConnection, _serverSocket);
            }
            catch (SocketException socketException)
            {
                if (_logManager != null)
                {
                    var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    _logManager.Log("Error in " + method + ": " + socketException.Message, LogType.Error);
                    _logManager.Log(socketException.StackTrace, LogType.Error);
                }
            }
        }

        private void OnAcceptConnection(IAsyncResult asyncResult)
        {
            try
            {
                if (_serverSocket == null)
                {
                    return;
                }

                var server = (Socket)asyncResult.AsyncState;
                var client = server.EndAccept(asyncResult);

                var newPlayer = new Player(client);

                if (!Sahara.GetServer().GetGameManager().GetPlayerManager().TryAddPlayer(newPlayer))
                {
                    _logManager.Log("Error trying to add player.", LogType.Error);
                }
            }
            catch (SocketException socketException)
            {
                var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logManager.Log("Error in " + method + ": " + socketException.Message, LogType.Error);
                _logManager.Log(socketException.StackTrace, LogType.Error);
            }
            finally
            {
                _serverSocket?.BeginAccept(OnAcceptConnection, _serverSocket);
            }
        }
    }
}
