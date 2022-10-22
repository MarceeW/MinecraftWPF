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
        internal BlockType[] Items { get; set; } = new BlockType[MaxItems];
        public const int MaxItems = 9;
        public Hotbar()
        {
            Items[0] = BlockType.GrassBlock;
            Items[1] = BlockType.Cobblestone;
            Items[2] = BlockType.WoodPlank;
            Items[3] = BlockType.Glass;
            Items[4] = BlockType.GreyConcrete;
            Items[5] = BlockType.BlackConcrete;
            Items[6] = BlockType.OakTrunk;
            Items[7] = BlockType.OakLeaves;
            Items[8] = BlockType.Bedrock;
        }
        internal BlockType GetSelectedBlock()
        {
            return Items[SelectedItemIndex];
        }
        public void UpdateSelectedIndex(int delta)
        {
            SelectedItemIndex += delta < 0 ? 1 : -1;

            if (SelectedItemIndex < 0)
                SelectedItemIndex = MaxItems - 1;

            else if (SelectedItemIndex == MaxItems)
                SelectedItemIndex = 0;
        }
    }
}
