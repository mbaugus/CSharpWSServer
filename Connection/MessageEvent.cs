using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection
{
    public enum MessageType
    {
        MESSAGERECEIVED,
        MESSAGESENT,
        SOCKETCLOSED,
        NEWCONNECTION
    };

    public class MessageEventArgs : EventArgs
    {
        public string Message { get; set; }
        public int RefId { get; set; }
        public MessageType MsgType { get; set; }

        public MessageEventArgs(int reference, string message, MessageType msgtype)
        {
            RefId = reference;
            Message = message;
            MsgType = msgtype;
        }
    }

    public delegate void MessageEventHandler(object send, MessageEventArgs e);


}
