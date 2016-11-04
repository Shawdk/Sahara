using Sahara.Core.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sahara.Core.Config
{
    internal class ConfigManager
    {
        private readonly Dictionary<string, string> _configItems;

        public ConfigManager(string configFile)
        {
            _configItems = new Dictionary<string, string>();

            var logManager = Sahara.GetServer().GetLogManager();

            try
            {
                if (File.Exists(configFile))
                {
                    var stopwatch = Stopwatch.StartNew();

                    _configItems = File.ReadLines(configFile)
                    .Where(IsConfigurationLine)
                    .Select(line => line.Split('='))
                    .ToDictionary(line => line[0], line => line[1]);

                    stopwatch.Stop();
                    logManager.Log("Loaded Config Data [" + stopwatch.ElapsedMilliseconds + "ms]", LogType.Information);
                }
                else
                {
                    _configItems.Add("database.host", "localhost");
                    _configItems.Add("database.username", "root");
                    _configItems.Add("database.password", "");
                    _configItems.Add("database.name", "database");
                    _configItems.Add("database.port", "3306");
                    _configItems.Add("database.max_connections", "10000");

                    logManager.Log("Using config defaults", LogType.Information);
                }
            }
            catch (Exception exception)
            {
                var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                logManager.Log($"Error in {method}: {exception.Message}", LogType.Error);
                logManager.Log(exception.StackTrace, LogType.Error);
            }
        }

        private static bool IsConfigurationLine(string line)
        {
            return !line.StartsWith("#") && line.Contains("=");
        }

        public string GetConfigElement(string key)
        {
            string value;

            if (!_configItems.TryGetValue(key, out value))
            {
                throw new KeyNotFoundException("Unable to find key " + key + " in config items dictionary.");
            }

            return value;
        }
    }
}
