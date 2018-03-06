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

        public WebSocket socket = null;
        public int refId = -1;

        public string name = "";

        public bool InUse = false;

        public event MessageEventHandler MessageReceived;
        protected virtual void OnMessage(MessageEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }

        public Connection(int id)
        {
            refId = id;
            ClearBuffer();
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
                    if (receiveResult.MessageType != WebSocketMessageType.Text)
                    {
                        socket.Abort();
                        CloseSocketRequest("Cannot accept binary frames.");
                        await socket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Cannot accept binary frame", CancellationToken.None);
                        KillSocket("Couldnt accept binary frame");
                        return;
                    }

                    sb.Append(Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count));

                } while (!receiveResult.EndOfMessage);

                string msg = sb.ToString();
                //Console.WriteLine(msg);
                //BasicInfo binfo = JsonConvert.DeserializeObject<BasicInfo>(msg);
                //string json = JsonConvert.SerializeObject(binfo);

                OnMessage(new MessageEventArgs(refId, msg, MessageType.MESSAGERECEIVED));

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
            if (!this.InUse)
                return;

            if (socket != null)
            {
                socket.Dispose();
            }

            OnMessage(new MessageEventArgs(refId, SpecialMessage, MessageType.SOCKETCLOSED));

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

