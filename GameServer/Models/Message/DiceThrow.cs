using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Models.Message
{
    [Serializable]
    public class DiceThrow : SocketMessage
    {
        public int rolledNumber { get; set; }

        public DiceThrow(string playerId, int rolledNumber) : base(playerId)
        {
            messageType = MessageType.DICE_THROW;
            this.rolledNumber = rolledNumber;
        }
        public DiceThrow()
        {

        }
    }
}
