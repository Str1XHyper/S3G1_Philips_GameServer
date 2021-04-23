using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Models.Message
{
    public enum MovementDirection
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }
    public class DirectionChosen : SocketMessage
    {
        public MovementDirection direction { get; set; }

        public DirectionChosen(string playerId, MovementDirection direction) : base(playerId)
        {
            this.direction = direction;

            messageType = MessageType.DIRECTION_CHOSEN;
        }

        public DirectionChosen()
        {

            messageType = MessageType.DIRECTION_CHOSEN;
        }
    }
}
