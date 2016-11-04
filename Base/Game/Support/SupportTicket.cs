using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sahara.Base.Game.Players;
using Sahara.Base.Utility;

namespace Sahara.Base.Game.Support
{
    internal class SupportTicket
    {
        private readonly int _ticketId;
        private readonly string _reportedUsername;
        private readonly string _senderUsername;
        private readonly string _message;
        private string _moderatorUsername;
        private int _moderatorId;
        private readonly int _reportedId;
        private readonly int _roomId;
        private readonly string _roomName;
        private readonly int _score;
        private readonly int _senderId;
        private SupportTicketStatus _ticketStatus;
        private readonly int _type;
        private readonly double _timestamp;
        private readonly List<string> _reportedChatMessages;

        public SupportTicket(int ticketId, string reportedUsername, string senderUsername, string message, string moderatorUsername, int moderatorId,  int reportedId, int roomId, string roomName, int score, int senderId, SupportTicketStatus ticketStatus, int type, double timestamp, List<string> reportedChatMessages)
        {
            _ticketId = ticketId;
            _reportedUsername = reportedUsername;
            _senderUsername = senderUsername;
            _message = message;
            _moderatorUsername = moderatorUsername;
            _moderatorId = moderatorId;
            _reportedId = reportedId;
            _roomId = roomId;
            _roomName = roomName;
            _score = score;
            _senderId = senderId;
            _ticketStatus = ticketStatus;
            _type = type;
            _timestamp = timestamp;
            _reportedChatMessages = reportedChatMessages;
        }

        public int TabId
        {
            get
            {
                switch (_ticketStatus)
                {
                    case SupportTicketStatus.Open:
                        return 1;
                    case SupportTicketStatus.Picked:
                        return 2;
                    case SupportTicketStatus.Abusive:
                    case SupportTicketStatus.Invalid:
                    case SupportTicketStatus.Resolved:
                    case SupportTicketStatus.Deleted:
                        return 0;
                }

                return 0;
            }
        }

        public int TicketId => _ticketId;

        public void PickTicket(int moderatorId, bool updateDatabase)
        {
            _ticketStatus = SupportTicketStatus.Picked;
            _moderatorId = moderatorId;

            PlayerData moderatorPlayerData;

            if (PlayerLoader.TryGetDataById(moderatorId, out moderatorPlayerData))
            {
                _moderatorUsername = moderatorPlayerData.Username;
            }

            if (updateDatabase)
            {
                using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
                {
                    mysqlConnection.OpenConnection();
                    mysqlConnection.RunQuery("UPDATE `moderation_tickets` SET `status` = 'picked', moderator_id = " + _moderatorId + ", timestamp = '" + UnixTimestampGenerator.GetNow() + "' WHERE id = " + _ticketId + "");
                    mysqlConnection.CloseConnection();
                }
            }

            moderatorPlayerData = null;
        }

        public void CloseTicket()
        {
            
        }
    }
}
 