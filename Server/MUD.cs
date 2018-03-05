using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class MUD
    {
        public void Start()
        {
            // pass port number
            Connection.WebSocketServer wss = new Connection.WebSocketServer();
            wss.Start("http://localhost:8080/MUD/");
            Console.WriteLine("Hit a key to exit.");
            Console.ReadKey();
        }
    }
}
