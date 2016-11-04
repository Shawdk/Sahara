using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sahara.Base.Game.Players;
using Sahara.Base.Game.Rooms;

namespace Sahara.Base.Game.Moderation
{
    class ModerationTicket
    {
        private int _ticketId;
        private readonly int _ticketType;
        private readonly double _ticketTimestamp;
        private readonly int _ticketCategory;
        private readonly int _ticketPriority;
        private readonly bool _ticketAnswered;
        private readonly PlayerData _ticketSender;
        private readonly PlayerData _ticketTarget;
        private readonly PlayerData _moderator;
        private readonly string _issue;
        private readonly RoomInformation _ticketRoomInformation;

        public ModerationTicket(int ticketId, int ticketType, double ticketTimestamp, int ticketCategory, int ticketPriority, PlayerData ticketSender, PlayerData ticketTarget, string issue, RoomInformation ticketRoomInformation)
        {
            _ticketId = ticketId;
            _ticketType = ticketType;
            _ticketTimestamp = ticketTimestamp;
            _ticketCategory = ticketCategory;
            _ticketPriority = ticketPriority;
            _ticketAnswered = false;
            _ticketSender = ticketSender;
            _ticketTarget = ticketTarget;
            _moderator = null;
            _issue = issue;
            _ticketRoomInformation = ticketRoomInformation;
        }

        public int TicketId
        {
            get { return _ticketId; }
            set { _ticketId = value; }
        }

        public int TicketType => _ticketType;
        public double TicketTimestamp => _ticketTimestamp;
        public int TicketCategory => _ticketCategory;
        public int TicketPriority => _ticketPriority;
        public bool TicketAnswered => _ticketAnswered;
        public PlayerData TicketSender => _ticketSender;
        public PlayerData TicketTarget => _ticketTarget;
        private PlayerData Moderator => _moderator;
        public string Issue => _issue;
        public RoomInformation TicketRoomInformation => _ticketRoomInformation;
        
        public int GetTicketStatus(int id)
        {
            if (Moderator == null)
            {
                return -1;
            }

            if (Moderator.PlayerId == id && _ticketAnswered)
            {
                return 2;
            }

            return 3;
        }
    }
}
