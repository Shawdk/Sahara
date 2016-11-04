using Sahara.Core.Logging;
using System;
using System.Diagnostics;
using System.Threading;

namespace Sahara.Core.Server
{
    class ServerStatusUpdater : IDisposable
    {
        private Timer _serverStatusUpdateTimer;
        private DateTime _lastUpdate;
        private readonly int _secondsInterval;

        public ServerStatusUpdater()
        {
            var stopwatch = Stopwatch.StartNew();

            _secondsInterval = 1;
            _serverStatusUpdateTimer = new Timer(Update, null, _secondsInterval, _secondsInterval);

            stopwatch.Stop();
            Sahara.GetServer().GetLogManager().Log("Loaded Server Updater [" + stopwatch.ElapsedMilliseconds + "ms]", LogType.Information);
        }

        private void Update(object obj)
        {
            if ((DateTime.Now - _lastUpdate).TotalSeconds <= _secondsInterval)
            {
                return;
            }

            var serverUptime = DateTime.Now - Sahara.GetServer().StartedTime;

            var days = serverUptime.Days + " day" + (serverUptime.Days != 1 ? "s" : "") + ", ";
            var hours = serverUptime.Hours + " hour" + (serverUptime.Hours != 1 ? "s" : "") + ", and ";
            var minutes = serverUptime.Minutes + " minute" + (serverUptime.Minutes != 1 ? "s" : "");
            var uptimeString = days + hours + minutes;

            Console.Title = Sahara.GetServer().GetServerInformation().ServerName + " - Uptime: " + uptimeString;
            _lastUpdate = DateTime.Now;
        }

        public void Dispose()
        {
            if (_serverStatusUpdateTimer == null)
            {
                return;
            }

            _serverStatusUpdateTimer.Dispose();
            _serverStatusUpdateTimer = null;
        }
    }
}
