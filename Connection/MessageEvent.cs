using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection
{
    public class MessageEventArgs : EventArgs
    {
        public string Message { get; set; }
        public int RefId { get; set; }

        public MessageEventArgs(int reference, string message)
        {
            RefId = reference;
            Message = message;
        }
    }

    public delegate void MessageEventHandler(object send, MessageEventArgs e);


}
