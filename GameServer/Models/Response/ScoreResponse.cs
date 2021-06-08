using System;
using System.Collections.Generic;
using System.Text;

public class ScoreResponse
{
    public int Points { get; set; }
    public int Stars { get; set; }
    public string PlayerID { get; set; }
    public string Username { get; set; }

    public ScoreResponse(string playerId, int points, int stars, string username)
    {
        Points = points;
        Stars = stars;
        PlayerID = playerId;
        Username = username;
    }

    public ScoreResponse(Player player)
    {
        PlayerID = player.PlayerID;
        Points = player.Points;
        Stars = player.Stars;
        Username = player.Username;
    }
}
