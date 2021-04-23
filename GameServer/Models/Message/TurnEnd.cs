using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Models.Message
{
    [Serializable]
    public class TurnEnd : SocketMessage
    {
        public TurnEnd(string playerId) : base(playerId)
        {
            messageType = MessageType.TURN_END;
        }
    }
}
