using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Message
{
    public enum MessageType
    {
        DICE_THROW,
        ENCOUNTERED_SPACE,
        TURN_END, 
        DIRECTION_CHOSEN, 
        PASSED_BANK, 
        PASSED_START, 
        BOUGHT_STAR,
        ANSWERED_QUESTION,
        PLAYER_JOIN,
        START_GAME
    }
    public class SocketMessage
    {
        public string playerId { get; set; }
        public MessageType messageType { get; set; }

        public SocketMessage()
        {
        }

        protected SocketMessage(string playerId)
        {
            this.playerId = playerId;
        }
    }
}
