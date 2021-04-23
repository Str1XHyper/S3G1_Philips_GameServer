using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MovePlayerResponse : SocketResponse
{
    public int movementAmount { get; set; }
    public MovePlayerResponse(string playerId) : base(playerId)
    {
        responseType = ResponseType.MOVE_PLAYER;
    }

    public MovePlayerResponse(string playerId, int movementAmount) : this(playerId)
    {
        responseType = ResponseType.MOVE_PLAYER;
        this.movementAmount = movementAmount;
    }
}
