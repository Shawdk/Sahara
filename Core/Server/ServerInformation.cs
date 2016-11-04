
namespace Sahara.Core.Server
{
    internal class ServerInformation
    {
        public ServerInformation()
        {
            ServerName = "Sahara Emulator";
            ServerVersion = "1.0 BETA";
            ServerDeveloper = "Liam Savage, Rahmeer";

            ConsoleLogo = new[] { @"   _____       _                     ",
                @"  / ____|     | |                    ",
                @" | (___   __ _| |__   __ _ _ __ __ _      " + ServerName + "",
                @"  \___ \ / _` | '_ \ / _` | '__/ _` |     " + ServerVersion + "",
                @"  ____) | (_| | | | | (_| | | | (_| |     ",
                @" |_____/ \__,_|_| |_|\__,_|_|  \__,_|     Developers: " + ServerDeveloper + "",
                @"",
                @""};
        }

        public string ServerName { get; }

        public string ServerVersion { get; }

        public string ServerDeveloper { get; }

        public string[] ConsoleLogo { get; }
    }
}
