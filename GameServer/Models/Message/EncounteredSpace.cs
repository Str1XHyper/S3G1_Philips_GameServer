using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Message
{
    public enum SpaceType
    {
        CHOOSE_DIRECTION,
        GAIN_POINTS,
        LOSE_POINTS,
        START,
        PORTAL,
        BANK
    }
    [Serializable]
    public class EncounteredSpace : SocketMessage
    {
        public SpaceType spaceType { get; set; }

        public EncounteredSpace(string playerId, SpaceType spaceType) : base(playerId)
        {
            this.spaceType = spaceType;
            messageType = MessageType.ENCOUNTERED_SPACE;
        }

        public EncounteredSpace()
        {
            messageType = MessageType.ENCOUNTERED_SPACE;
        }
    }
}
