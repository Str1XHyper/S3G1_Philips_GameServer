using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Message
{
    public class StartGame : SocketMessage
    {
        public StartGame(string playerId) : base(playerId)
        {
            messageType = MessageType.START_GAME;
        }
        public StartGame()
        {
            messageType = MessageType.START_GAME;
        }
    }
}
