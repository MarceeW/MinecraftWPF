using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.Misc
{
    internal class WorldData : ObservableObject
    {
        public string WorldName { get; set; }
        public int WorldSeed { get; set; }
        public string WorldPath { get; set; }
        public DateTime LastPlayed { get; set; }
    }
}
