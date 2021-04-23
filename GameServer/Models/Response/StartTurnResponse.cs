using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StartTurnResponse : SocketResponse
{
    public StartTurnResponse(string playerId) : base(playerId)
    {
        responseType = ResponseType.START_TURN;
    }
}
