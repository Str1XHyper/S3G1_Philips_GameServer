using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public class Player
    {
        private string playerID;
        private int points;
        private int stars;

        public Player(string playerID)
        {
            this.playerID = playerID;
            this.points = 0;
            this.stars = 0;
        }

        public string PlayerID { get => playerID;}
        public int Points { get => points; }
        public int Stars { get => stars; }

        public void AddPoints(int amount)
        {
            points += amount;
        }

        public void SubtractPoints(int amount)
        {
            points -= amount;
            if(points < 0)
            {
                points = 0;
            }
        }

        internal void AddStar(int amount)
        {
            stars += amount;
        }
    }
}
