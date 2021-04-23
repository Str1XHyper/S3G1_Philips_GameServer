using Models.Message;
using System;
using System.Collections.Generic;
using System.Text;

public class DirectionChosenResponse : SocketResponse
{
    public MovementDirection ChosenDirection { get; set; }
    public DirectionChosenResponse(string playerId, MovementDirection chosenDirection) : base(playerId)
    {
        responseType = ResponseType.DIRECTION_CHOSEN;
        ChosenDirection = chosenDirection;
    }
}