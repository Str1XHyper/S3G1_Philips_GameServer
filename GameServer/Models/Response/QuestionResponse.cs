using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class QuestionResponse : SocketResponse
{
    public Question question { get; set; }
    public QuestionResponse(string playerId, Question question) : base(playerId)
    {
        responseType = ResponseType.QUESTION;
        this.question = question;
    }
}
