using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Connection;

using Newtonsoft.Json;

using static System.Console;

namespace Server
{
    class JSONMessage
    {
        public string Name;
        public string Message;
    }
    class MUD
    {

        public void Start()
        {
            // pass port number
            Connection.WebSocketServer wss = new Connection.WebSocketServer();
            wss.Start("http://localhost:8080/MUD/");

            wss.LostConnection += new MessageEventHandler(this.OnClosedConnection);
            wss.NewConnection += new MessageEventHandler(this.OnNewConnection);
            wss.ClientMessageReceived += new MessageEventHandler(this.OnNewMessage);


            JSONMessage message = new JSONMessage();
            message.Message = "Hey";
            message.Name = "Server";
            //Console.WriteLine(msg);
            //BasicInfo binfo = JsonConvert.DeserializeObject<BasicInfo>(msg);
            string json = JsonConvert.SerializeObject(message);
            while (true)
            {
                wss.SendAll(json);
                Thread.Sleep(500);
            }
        }

        private void OnNewConnection(object send, MessageEventArgs e)
        {
            WriteLine("NewConnection: " + e.Message);
        }
        private void OnNewMessage(object send, MessageEventArgs e)
        {
            WriteLine("New Message: " + e.Message);
        }
        private void OnClosedConnection(object send, MessageEventArgs e)
        {
            WriteLine("Closed Connection: " + e.Message);
        }

    }
}
