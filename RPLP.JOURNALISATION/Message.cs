using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.JOURNALISATION
{
    public class Message
    {
        public Guid MessageID { get; set; }
        public string TypeOfMessage { get; set; }
        public string Log { get; set; }

        public Message() 
        {

        }

        public Message(Guid p_identifiant, string p_typeMessage, string p_log) 
        {
            MessageID = p_identifiant;
            TypeOfMessage = p_typeMessage;
            Log = p_log;
        }
    }
}
