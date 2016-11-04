using Plus.Communication.Encryption;
using Plus.Communication.Encryption.Keys;
using Sahara.Base.Game;
using Sahara.Base.Utility;
using Sahara.Core.Config;
using Sahara.Core.Database;
using Sahara.Core.Logging;
using Sahara.Core.Packets;
using Sahara.Core.Server;
using Sahara.Core.Sockets;
using System;
using System.Diagnostics;

namespace Sahara
{
    internal class SaharaServer
    {
        private ServerStatusUpdater _serverStatusUpdater;
        private LogManager _logManager;
        private ServerInformation _serverInformation;
        private ConfigManager _configManager;
        private DatabaseManager _mysqlManager;
        private SocketManager _socketManager;
        private GameManager _gameManager;
        private PacketManager _packetManager;
        private Utility _utility;

        public void Load()
        {
            _logManager = new LogManager();
            _serverInformation = new ServerInformation();

            foreach (var consoleOutputString in _serverInformation.ConsoleLogo)
            {
                Console.WriteLine(consoleOutputString);
            }

            _logManager.Log("Loading " + _serverInformation.ServerName + "...", LogType.Information);

            var stopwatch = Stopwatch.StartNew();

            _configManager = new ConfigManager("Extra/Other/config.ini");

            var databaseStopwatch = Stopwatch.StartNew();

            _mysqlManager = new DatabaseManager(new DatabaseSettings(
                _configManager.GetConfigElement("database.host"),
                _configManager.GetConfigElement("database.username"),
                _configManager.GetConfigElement("database.password"),
                _configManager.GetConfigElement("database.name"),
                int.Parse(_configManager.GetConfigElement("database.port")),
                int.Parse(_configManager.GetConfigElement("database.max_connections"))));

            if (!_mysqlManager.WorkingConnection())
            {
                _logManager.Log("Unable to connect to the MySQL server.", LogType.Error);
                return;
            }

            databaseStopwatch.Stop();
            _logManager.Log("Loaded MySQL [" + databaseStopwatch.ElapsedMilliseconds + "ms]", LogType.Information);

            _socketManager = new SocketManager();
            _gameManager = new GameManager();
            _packetManager = new PacketManager();
            _utility = new Utility();

            StartedTime = DateTime.Now;
            _serverStatusUpdater = new ServerStatusUpdater();

            HabboEncryptionV2.Initialize(new RSAKeys());

            stopwatch.Stop();

            _logManager.Log("Finished Loading! [" + stopwatch.ElapsedMilliseconds + "ms]", LogType.Warning);

            Console.ForegroundColor = ConsoleColor.Black;
        }

        public LogManager GetLogManager()
        {
            return _logManager;
        }

        public ServerInformation GetServerInformation()
        {
            return _serverInformation;
        }

        public GameManager GetGameManager()
        {
            return _gameManager;
        }

        public PacketManager GetPacketManager()
        {
            return _packetManager;
        }

        public Utility GetUtility()
        {
            return _utility;
        }

        public DateTime StartedTime { get; private set; }

        public void Dispose()
        {
            try
            {
                _serverStatusUpdater.Dispose();
            }
            catch (Exception exception)
            {
                if (_logManager != null)
                {
                    _logManager.Log("Error in disposing SaharaServer: " + exception.Message, LogType.Error);
                    _logManager.Log(exception.StackTrace, LogType.Error);
                }
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        public DatabaseManager GetMySql()
        {
            return _mysqlManager;
        }
    }
}
