using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using Newtonsoft.Json;

namespace Connection
{
    public class BasicInfo
    {
        public string Name = "";
        public string Message = "";
    }

    public class Connection
    {
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] receiveBuffer = new byte[BufferSize];

        //public List<byte> builder = new List<byte>();
        public StringBuilder sb = new StringBuilder();

        //socket is initiated by the Webserver
        public WebSocket socket = null;

        // reference with guid, is faster lookup
        public Guid guid { get; set; }

        // reference with a nickname, can be slower to find
        public string Nickname { get; set; }

        Dictionary<string, Channel> Channels;

        public event MessageEventHandler MessageReceived;
        protected virtual void OnMessage(MessageEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }

        public Connection(ChannelGroupSettings info)
        {
            Channels = new Dictionary<string, Channel>();

            if (info.Channels.Count < 1)
            {
                throw new Exception("You cannot have less than 1 channel in a Connection");
            }
            if ( info.Channels.Count > 16)
            {
                throw new Exception("You cannot have more than 16 channels in a Connection");
            }

            ClearBuffer();

            foreach (var c in info.Channels)
            {
                Channels[c.Name] = new Channel(c);
            }
        }

        public async Task ReceiveAsync()
        {
           // Console.WriteLine($"Recv Async: { socket.State}");

            try
            {
                if (socket.State != WebSocketState.Open)
                {
                    socket.Abort();
                    KillSocket("Socket state != WebSocketState.Open");
                    return;
                }

                WebSocketReceiveResult receiveResult = null;

                do
                {
                    receiveResult = await socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        socket.Abort();
                        CloseSocketRequest("Request to close socket");
                        KillSocket("Request to close socket from client");
                        return;
                    }
                    if (receiveResult.MessageType == WebSocketMessageType.Text)
                    {
                        /*
                        socket.Abort();
                        CloseSocketRequest("Cannot accept binary frames.");
                        await socket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Cannot accept binary frame", CancellationToken.None);
                        KillSocket("Couldnt accept binary frame");
                        return;
                        */
                    }
                    else if(receiveResult.MessageType == WebSocketMessageType.Binary)
                    {

                    }

                    sb.Append(Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count));

                } while (!receiveResult.EndOfMessage);

                string msg = sb.ToString();
                //Console.WriteLine(msg);
                //BasicInfo binfo = JsonConvert.DeserializeObject<BasicInfo>(msg);
                //string json = JsonConvert.SerializeObject(binfo);
                string channel = "";
                //ChannelMessageTypes msgtype = ChannelMessageTypes.TEXT;
                OnMessage(new MessageEventArgs(guid, msg, MessageType.MESSAGERECEIVED, channel));

                ClearBuffer();

                await ReceiveAsync();
            }
            catch (Exception e)
            {
                // Just log any exceptions to the console. Pretty much any exception that occurs when calling `SendAsync`/`ReceiveAsync`/`CloseAsync` is unrecoverable in that it will abort the connection and leave the `WebSocket` instance in an unusable state.
                Console.WriteLine("Exception: {0}", e);
            }
            finally
            {
                // Clean up by disposing the WebSocket once it is closed/aborted.
                KillSocket("Exception thrown");
            }
        }

        public async void SendMessage(string message)
        {
            await socket.SendAsync(
                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(message), 0, Encoding.UTF8.GetByteCount(message)),
                    WebSocketMessageType.Text,
                    true, CancellationToken.None);
        }

        public void KillSocket(string SpecialMessage)
        {
            if (socket != null)
            {
                socket.Dispose();
            }

            OnMessage(new MessageEventArgs(guid, SpecialMessage, MessageType.SOCKETCLOSED, ""));
        }

        private void ClearBuffer()
        {
            Array.Clear(receiveBuffer, 0, BufferSize);
            sb.Clear();
        }

        private async void CloseSocketRequest(string msg)
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, msg, CancellationToken.None);
        }
    }
}

