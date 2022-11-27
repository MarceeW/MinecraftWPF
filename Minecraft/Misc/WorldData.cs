using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace Minecraft.Misc
{
    public class WorldData : ObservableObject
    {
        public string WorldName { get; set; }
        public int WorldSeed { get; set; }
        public string WorldPath { get; set; }
        public DateTime LastPlayed { get; set; }
    }
}
