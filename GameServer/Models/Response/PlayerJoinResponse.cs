using System;
using System.Collections.Generic;
using System.Text;

public class PlayerJoinResponse : SocketResponse
{
    public int amountOfPlayers;

    public PlayerJoinResponse(string playerId, int _amountOfPlayers) : base(playerId)
    {
        amountOfPlayers = _amountOfPlayers;
        responseType = ResponseType.PLAYER_JOIN;
    }
}
