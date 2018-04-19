using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection
{
    public class SocketMessage
    {
        // message 1 of 20
        // 
        // ushort msg1, of up to 65,000. 1 byte channel number,

        // header
        // [2 bytes = message # of , 2 bytes = totalmessages, 1 byte = channel number] 5 bytes total.
        // This class is built for use with a web socket protocol, and because WS guarantees a 
        // complete message on receive you can simply check the message size to get total bytes.
         // it is redunant to send this information.
         //
        public byte[] buffer { get; set; }
        public static int offset = sizeof(ushort) + sizeof(ushort) + sizeof(byte);
        int buffersize;
        public int ChannelNumber { get; set; }
        public ushort TotalMessages { get; set; }
        public ushort MessageNumber { get; set; }
        public int MessageLength { get; set; }

        public bool ValidHeader { get; private set; }

        public SocketMessage(int byteChunkSize)
        {
            buffersize = byteChunkSize;
            buffer = new byte[buffersize];
            ValidHeader = false;
            MessageNumber = 0;
            ChannelNumber = 0;
            TotalMessages = 0;
            MessageLength = 0;
        }

        public ArraySegment<byte> GetMessage()
        {
            return new ArraySegment<byte>(buffer, offset, MessageLength - offset);
        }

        public void Encode()
        {

        }

        public bool Decode()
        {
            MessageNumber = BitConverter.ToUInt16(buffer, 0);
            TotalMessages = BitConverter.ToUInt16(buffer, sizeof(ushort));
            ChannelNumber = buffer[4];

            Console.WriteLine($"Decode: MsgNumber {MessageNumber} TotalMessages {TotalMessages} ChanneNumber {ChannelNumber}");

            if(MessageNumber > 1 && MessageNumber < 65000 && TotalMessages > 0 && TotalMessages < 65000
                && MessageNumber <= TotalMessages && ChannelNumber > 0 && ChannelNumber <= 16 && MessageLength > offset)
            {
                ValidHeader = true;
                return true;
            }
            ValidHeader = false;
            return false;
        }

        public void Reset()
        {
            Array.Clear(buffer, 0, buffersize);
            MessageNumber = 0;
            ChannelNumber = 0;
            TotalMessages = 0;
            MessageLength = 0;
            ValidHeader = false;
        }
    }
}
