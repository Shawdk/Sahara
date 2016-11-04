using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Base.Game.Moderation
{
    class ModerationPresetActionMessage
    {
        private readonly int _id;
        private readonly int _parentId;
        private readonly string _caption;
        private readonly string _messageContent;
        private readonly int _muteTime;
        private readonly int _banTime;
        private readonly int _ipBanTime;
        private readonly int _tradeLockTime;
        private readonly string _noticeMessage;

        public ModerationPresetActionMessage(int id, int parentId, string caption, string messageContent, int muteTime, int banTime, int ipBanTime, int tradeLockTime, string noticeMessage)
        {
            _id = id;
            _parentId = parentId;
            _caption = caption;
            _messageContent = messageContent;
            _muteTime = muteTime;
            _banTime = banTime;
            _ipBanTime = ipBanTime;
            _tradeLockTime = tradeLockTime;
            _noticeMessage = noticeMessage;
        }

        public int Id => _id;
        public int ParentId => _parentId;
        public string Caption => _caption;
        public string MessageContent => _messageContent;
        public int MuteTime => _muteTime;
        public int BanTime => _banTime;
        public int IpBanTime => _ipBanTime;
        public int TradeLockTime => _tradeLockTime;
        public string NoticeMessage => _noticeMessage;
    }
}
