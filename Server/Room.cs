using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Room
    {
        public int id { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public int NorthRoomArea { get; set; }
        public int NorthRoomNumber { get; set; }
        public int SouthRoomArea { get; set; }
        public int SouthRoomNumber { get; set; }
        public int EastRoomArea { get; set; }
        public int EastRoomNumber { get; set; }
        public int WestRoomArea { get; set; }
        public int WestRoomNumber { get; set; }
        public int NERoomArea { get; set; }
        public int NERoomNumber { get; set; }
        public int NWRoomArea { get; set; }
        public int NWRoomNumber { get; set; }
        public int SERoomArea { get; set; }
        public int SERoomNumber { get; set; }
        public int SWRoomArea { get; set; }
        public int SWRoomNumber { get; set; }


        List<Player> Players;
        List<Npc> Npc;
        List<Item> Items;
    }
}
