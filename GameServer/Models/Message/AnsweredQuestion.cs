using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Message
{
    public class AnsweredQuestion : SocketMessage
    {
        public string answer { get; set; }

        public AnsweredQuestion()
        {
            messageType = MessageType.ANSWERED_QUESTION;
        }

        public AnsweredQuestion(string playerId, string answer) : base(playerId)
        {
            messageType = MessageType.ANSWERED_QUESTION;
            this.answer = answer;
        }
    }
}
