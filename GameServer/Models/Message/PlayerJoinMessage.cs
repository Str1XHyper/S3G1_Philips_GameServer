using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Message
{
    public class PlayerJoinMessage : SocketMessage
    {
        public string LessonID { get; set; }
        public string Username { get; set; }
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