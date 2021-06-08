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
        public int currentTileIndex { get; set; }
        public TurnEnd(string playerId, int currentTile) : base(playerId)
        {
            messageType = MessageType.TURN_END;
            currentTileIndex = currentTile;
        }

        public TurnEnd()
        {

        }
    }
}
