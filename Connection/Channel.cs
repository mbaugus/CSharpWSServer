using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Connection
{
    public enum ChannelPriority
    {
        LOW, NORMAL, HIGH
    }
    public enum ChannelMessageTypes
    {
        BINARY, TEXT, JSON
    }
    public class Channel
    {
        public Channel(Channel copyfrom = null)
        {
            if (copyfrom == null)
            {
                Chunksizes = 1024;
                Priority = ChannelPriority.NORMAL;
                MessageType = ChannelMessageTypes.TEXT;
                Name = "";
            }
            else
            {
                Chunksizes = copyfrom.Chunksizes;
                Name = copyfrom.Name;
                Priority = copyfrom.Priority;
                MessageType = copyfrom.MessageType;
            }

            Filestreamer = null;
        }

        public Channel(int chunksize, string channelname, ChannelPriority priority, ChannelMessageTypes messagetype)
        {
            Chunksizes = chunksize;
            Name = channelname;
            Priority = priority;
            MessageType = messagetype;
        }

        public FileStream Filestreamer { get; set; }
        public int Chunksizes { get; set; }
        public string Name { get; set; }
        public ChannelPriority Priority { get; set; }
        public ChannelMessageTypes MessageType { get; set; }

        public SocketMessage outgoing = new SocketMessage();
        public SocketMessage incoming = new SocketMessage();

        bool MessageInProgress { get; set; }

        Queue<string> OutgoingQueue = new Queue<string>();
    }
}
