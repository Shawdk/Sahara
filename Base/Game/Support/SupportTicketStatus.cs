using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Base.Game.Support
{
    internal enum SupportTicketStatus
    {
        Open = 0,
        Picked = 1,
        Resolved = 2,
        Abusive = 3,
        Invalid = 4,
        Deleted = 5
    }
}
