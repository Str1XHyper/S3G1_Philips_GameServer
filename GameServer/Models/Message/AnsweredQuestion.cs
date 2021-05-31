using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Message
{
    public class AnsweredQuestion : SocketMessage
    {
        public string answer;

        public AnsweredQuestion()
        {

        }

        public AnsweredQuestion(string playerId, string answer) : base(playerId)
        {
            messageType = MessageType.ANSWERED_QUESTION;
            this.answer = answer;
        }
    }
}
