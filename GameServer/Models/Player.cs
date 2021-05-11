using System;
using System.Collections.Generic;
using System.Text;

public class Player
{
    private string playerID;
    private string sessionID;
    private string username;
    private int points;
    private int stars;

    public Player(string playerID, string sessionId, string username)
    {
        this.playerID = playerID;
        points = 0;
        stars = 0;
        sessionID = sessionId;
        this.username = username;
    }

    public string PlayerID { get => playerID; }
    public int Points { get => points; }
    public int Stars { get => stars; }
    public string SessionID { get => sessionID;}
    public string Username { get => username; }

    public void AddPoints(int amount)
    {
        points += amount;
    }

    public void SubtractPoints(int amount)
    {
        points -= amount;
        if (points < 0)
        {
            points = 0;
        }
    }

    public void AddStar(int amount)
    {
        stars += amount;
    }
}