using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sahara.Core.Logging;

namespace Sahara.Base.Game.Players
{
    internal class PlayerProcessor
    {
        private readonly Player _player;
        private readonly Timer _processTimer;
        private bool _timerProcessing;
        private bool _timerLagging;
        private readonly bool _timerEnabled;
        private readonly int _timerInterval;
        private readonly LogManager _logManager;
        private readonly AutoResetEvent _resetEvent;

        public PlayerProcessor(Player player)
        {
            _player = player;
            _timerInterval = 60000;
            _logManager = Sahara.GetServer().GetLogManager();
            _resetEvent = new AutoResetEvent(true);
            _processTimer = new Timer(OnProcess, null, _timerInterval, _timerInterval);
        }

        private void OnProcess(object timerObject)
        {
            if (!_timerEnabled)
            {
                return;
            }

            try
            {
                if (_timerProcessing)
                {
                    _logManager.Log("PlayerProcessor timer is lagging", LogType.Information);
                    _timerLagging = true;
                }

                _timerProcessing = true;
                _resetEvent.Reset();

                // run your code c;

                _timerProcessing = false;
                _timerLagging = false;
                _resetEvent.Set();
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
