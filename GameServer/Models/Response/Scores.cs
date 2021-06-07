using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Response
{
    [Serializable]
    public class Scores : SocketResponse
    {
        public ScoreResponse[] scoreResponses { get; set; }

        public Scores(ScoreResponse[] scoreResponses, string playerId) : base(playerId)
        {
            this.scoreResponses = scoreResponses;
            responseType = ResponseType.SCORE;
        }
    }
}
