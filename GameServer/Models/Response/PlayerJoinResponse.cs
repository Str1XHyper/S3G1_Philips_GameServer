using System;
using System.Collections.Generic;
using System.Text;

public class PlayerJoinResponse : SocketResponse
{
    public List<Player> players { get; set; }

    public PlayerJoinResponse(string playerId, List<Player> _players) : base(playerId)
    {
        players = _players;
        responseType = ResponseType.PLAYER_JOIN;
    }
}
