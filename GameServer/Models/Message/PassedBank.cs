using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Message
{
    public class PassedBank : SocketMessage
    {
        public PassedBank(string playerId) : base(playerId)
        {
            messageType = MessageType.PASSED_BANK;
        }

        public PassedBank()
        {
            messageType = MessageType.PASSED_BANK;
        }
    }
}
