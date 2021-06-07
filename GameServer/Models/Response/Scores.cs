using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Response
{
    [Serializable]
    public class Scores
    {
        public ScoreResponse[] scoreResponses { get; set; }

        public Scores()
        {

        }

        public Scores(ScoreResponse[] scoreResponses)
        {
            this.scoreResponses = scoreResponses;
        }
    }
}
