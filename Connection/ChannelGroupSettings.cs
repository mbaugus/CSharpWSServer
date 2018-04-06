using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection
{
    public class ChannelGroupSettings
    {
        public List<Channel> Channels { get; set; }
        
        public ChannelGroupSettings()
        {
            Channels = new List<Channel>();
        }
    }
}
