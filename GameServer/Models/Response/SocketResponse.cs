using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public enum ResponseType
{
    START_GAME,
    START_TURN,
    MOVE_PLAYER,
    QUESTION,
    SCORE,
    PLAYER_JOIN,
    DIRECTION_CHOSEN,
}

[Serializable]
public abstract class SocketResponse
{
    public string playerId { get; set; }
    public ResponseType responseType { get; set; }

    protected SocketResponse(string playerId)
    {
        this.playerId = playerId;
    }
}
