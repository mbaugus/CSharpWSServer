using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class DbContextFile: DbContext
    {
        public DbContextFile() : base()
        {

        }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Npc> Npcs { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Item> Items { get; set; }
    }
}
