using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Message
{
    public class PlayerJoinMessage : SocketMessage
    {
        public PlayerJoinMessage(string playerId) : base(playerId)
        {
            messageType = MessageType.PLAYER_JOIN;
        }

        public PlayerJoinMessage()
        {
            messageType = MessageType.PLAYER_JOIN;
        }
    }
}