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
        public Guid RefId { get; set; }
        public MessageType MsgType { get; set; }
        public string Nickname { get; set; }
        public string ChannelName { get; set; }
        public MessageEventArgs(Guid reference, string message, MessageType msgtype, string channelname, string nickname = "")
        {
            RefId = reference;
            Message = message;
            MsgType = msgtype;
            Nickname = nickname;
            ChannelName = channelname;
        }
    }

    public delegate void MessageEventHandler(object send, MessageEventArgs e);


}
