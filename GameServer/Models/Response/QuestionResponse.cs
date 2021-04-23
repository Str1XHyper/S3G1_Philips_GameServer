using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class QuestionResponse : SocketResponse
{
    public Question question;
    public QuestionResponse(string playerId) : base(playerId)
    {
        responseType = ResponseType.QUESTION;
    }
}
