using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Models.Message
{
    public class PassedStart : SocketMessage
    {
        public PassedStart(string playerId) : base(playerId)
        {
            messageType = MessageType.PASSED_START;
        }

        public PassedStart()
        {
            messageType = MessageType.PASSED_START;
        }
    }
}
