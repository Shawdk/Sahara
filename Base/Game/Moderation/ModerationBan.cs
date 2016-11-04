using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sahara.Base.Utility;

namespace Sahara.Base.Game.Moderation
{
    internal class ModerationBan
    {
        private readonly ModerationBanType _banType;
        private readonly string _banValue;
        private readonly string _banReason;
        private readonly double _banExpirationDate;

        public ModerationBan(ModerationBanType banType, string banValue, string banReason, double banExpirationDate)
        {
            _banType = banType;
            _banValue = banValue;
            _banReason = banReason;
            _banExpirationDate = banExpirationDate;
        }

        public ModerationBanType BanType => _banType;
        public bool HasExpired => UnixTimestampGenerator.GetNow() >= _banExpirationDate;
    }
}

