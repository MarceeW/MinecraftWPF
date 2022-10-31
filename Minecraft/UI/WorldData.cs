using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.UI
{
    class WorldData
    {
        public string WorldName { get; set; }
        public int WorldSeed { get; set; }
        public string WorldPath { get; set; }
        public DateTime LastPlayed { get; set; }
    }
}
