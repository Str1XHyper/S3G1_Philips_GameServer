using System;
using System.Collections.Generic;
using System.Text;

    public class EndGameResponse : SocketResponse
    {
        public EndGameResponse (string playerId) : base(playerId)
        {
            responseType = ResponseType.END_GAME;
        }
    }
