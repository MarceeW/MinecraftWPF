using Minecraft.Terrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.UI
{
    internal class Hotbar
    {
        public int SelectedItemIndex { get; private set; }
        public BlockType[] Content { get; set; } = new BlockType[maxItems];
        private const int maxItems = 9;
        public Hotbar()
        {
        }
        public void UpdateSelectedIndex(int delta)
        {
            SelectedItemIndex += delta;

            if (SelectedItemIndex < 0)
                SelectedItemIndex = maxItems - 1;

            else if (SelectedItemIndex == maxItems)
                SelectedItemIndex = 0;
        }
    }
}
