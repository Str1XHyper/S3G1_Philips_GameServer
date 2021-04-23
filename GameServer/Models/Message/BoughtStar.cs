using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Message
{
    public class BoughtStar : SocketMessage
    {
        public BoughtStar(string playerId) : base(playerId)
        {
            messageType = MessageType.BOUGHT_STAR;
        }

        public BoughtStar()
        {

        }
    }
}
