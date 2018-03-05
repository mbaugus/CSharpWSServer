using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.WebSockets;
using System.Net.Http;
using System.Threading;

namespace Connection
{

    public class WebSocketServer
    {
        private int count = 0;
        private static int MaxConnections = 500;
        Connection[] Connections = new Connection[MaxConnections];

        // List<> connections

        public WebSocketServer()
        {
            for (int i = 0; i < MaxConnections; i++)
            {
                Connections[i] = new Connection(i);
                Connections[i].MessageReceived += new MessageEventHandler(this.ReceiveMessageFromSocket);
            }
        }

        private int GetFreeSlot()
        {
            for(int i = 0; i < MaxConnections; i++)
            {
                if (!Connections[i].InUse)
                {
                    return i;
                }
            }
            return -1;
        }
      
        public void ReceiveMessageFromSocket(object send, MessageEventArgs e)
        {
            Console.WriteLine($"Rec'd msg from Socket: {e.RefId}  Msg: {e.Message}");
        }

 
        public async void Start(string listenerPrefix)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(listenerPrefix);
            listener.Start();
            Console.WriteLine("Listening...");

            while (true)
            {
                HttpListenerContext listenerContext = await listener.GetContextAsync();
                if (listenerContext.Request.IsWebSocketRequest)
                {
                    ProcessRequest(listenerContext);
                }
                else
                {
                    listenerContext.Response.StatusCode = 400;
                    listenerContext.Response.Close();
                }
            }
        }

        private async void ProcessRequest(HttpListenerContext listenerContext)
        {
            WebSocketContext wsctx = null;
            try
            {
                wsctx = await listenerContext.AcceptWebSocketAsync(subProtocol: null);
                Interlocked.Increment(ref count);
                Console.WriteLine("Processed: {0}", count);
            }
            catch (Exception e)
            {
                // The upgrade process failed somehow. For simplicity lets assume it was a failure on the part of the server and indicate this using 500.
                listenerContext.Response.StatusCode = 500;
                listenerContext.Response.Close();
                Console.WriteLine("Exception: {0}", e);
                return;
            }
            Console.WriteLine($"Secure:{wsctx.IsSecureConnection}, Authenticated: {wsctx.IsAuthenticated}, IsLocal: {wsctx.IsLocal}, " +
                $"Origin: {wsctx.Origin}, WS Version: {wsctx.SecWebSocketVersion}, User: {wsctx.User}");


            WebSocket webSocket = wsctx.WebSocket;
            
            int freeid = GetFreeSlot();
            Connection c = Connections[freeid];
            c.socket = webSocket;
            c.InUse = true;
            
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            c.ReceiveAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
