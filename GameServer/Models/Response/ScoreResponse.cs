using System;
using System.Collections.Generic;
using System.Text;

public class ScoreResponse : SocketResponse
{
    public int Points { get; set; }
    public int Stars { get; set; }
    public ScoreResponse(string playerId, int points, int stars) : base(playerId)
    {
        responseType = ResponseType.SCORE;
        Points = points;
        Stars = stars;
    }

    public ScoreResponse(Player player) : base(player.PlayerID)
    {
        responseType = ResponseType.SCORE;
        Points = player.Points;
        Stars = player.Stars;
    }
}
