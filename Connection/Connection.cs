using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;

namespace Connection
{
    public class Connection
    {
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] receiveBuffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
        public WebSocket socket = null;
        public int refId = -1;
        public bool InUse = false;

        public event MessageEventHandler MessageReceived;

        // The protected OnAlarm method raises the event by invoking
        // the delegates. The sender is always this, the current instance
        // of the class.
        protected virtual void OnMessage(MessageEventArgs e)
        {
            MessageEventHandler handler = MessageReceived;
            if (handler != null)
            {
                // Invokes the delegates.
                handler(this, e);
            }
        }

        public Connection(int id)
        {
            refId = id;
        }

        public async Task ReceiveAsync()
        {
            while (true)
            {
                try
                {
                    if (socket.State != WebSocketState.Open)
                    {
                        await KillSocket();
                        return;
                    }

                    WebSocketReceiveResult receiveResult = null;

                    do
                    {
                        receiveResult = await socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                        if (receiveResult.MessageType == WebSocketMessageType.Close)
                        {
                            await KillSocket();
                            return;
                        }
                        if (receiveResult.MessageType != WebSocketMessageType.Text)
                        {
                            // send cancel message, then kill.
                            await socket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Cannot accept binary frame", CancellationToken.None);
                            await KillSocket();
                        }

                        sb.Append(receiveBuffer);

                    } while (!receiveResult.EndOfMessage);

                    OnMessage(new MessageEventArgs(refId, sb.ToString()));
                    ClearBuffer();

                    /*
                    await socket.SendAsync(
                            new ArraySegment<byte>(receiveBuffer,0,receiveResult.Count),
                            WebSocketMessageType.Text,
                            true, CancellationToken.None);
                    */
                }
                catch (Exception e)
                {
                    // Just log any exceptions to the console. Pretty much any exception that occurs when calling `SendAsync`/`ReceiveAsync`/`CloseAsync` is unrecoverable in that it will abort the connection and leave the `WebSocket` instance in an unusable state.
                    Console.WriteLine("Exception: {0}", e);
                }
                finally
                {
                    // Clean up by disposing the WebSocket once it is closed/aborted.
                    if (socket != null)
                        socket.Dispose();
                }
            }
        }


        public async Task KillSocket()
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            if (socket != null)
                socket.Dispose();
        }

        private void ClearBuffer()
        {
            Array.Clear(receiveBuffer, 0, BufferSize);
        }
    }
}

