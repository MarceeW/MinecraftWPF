using Minecraft.Terrain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.UI
{
    public class Hotbar
    {
        public int SelectedItemIndex { get; private set; }
        internal BlockType[] Content { get; set; } = new BlockType[maxItems];
        private const int maxItems = 9;
        public Hotbar()
        {
            Content[0] = BlockType.GrassBlock;
            Content[1] = BlockType.Sand;
            Content[2] = BlockType.Stone;
            Content[3] = BlockType.Glass;
            Content[4] = BlockType.WoodPlank;
            Content[5] = BlockType.Cobblestone;
            Content[6] = BlockType.OakTrunk;
            Content[7] = BlockType.OakLeaves;
            Content[8] = BlockType.Bedrock;
        }
        internal BlockType GetSelectedBlock()
        {
            return Content[SelectedItemIndex];
        }
        public void UpdateSelectedIndex(int delta)
        {
            SelectedItemIndex += delta < 0 ? 1 : -1;

            if (SelectedItemIndex < 0)
                SelectedItemIndex = maxItems - 1;

            else if (SelectedItemIndex == maxItems)
                SelectedItemIndex = 0;
        }
    }
}
