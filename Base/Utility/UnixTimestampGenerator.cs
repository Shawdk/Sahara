using System;

namespace Sahara.Base.Utility
{
    internal static class UnixTimestampGenerator
    {
        public static double GetNow()
        {
            var ts = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0));
            return ts.TotalSeconds;
        }
    }
}
