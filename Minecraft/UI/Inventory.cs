using Minecraft.Terrain;
using System;

namespace Minecraft.UI
{
    public class Inventory
    {
        public int Rows { get; }
        public int Columns { get; } = 16;

        public BlockType[,] Blocks;
        public Inventory()
        {
            var blocks = Enum.GetValues(typeof(BlockType)) as BlockType[];

            Rows = blocks.Length / Columns + 1;

            Blocks = new BlockType[Rows, Columns];
            
            for (int y = 0; y < Rows; y++)
                for (int x = 0; x < Columns; x++)
                {
                        if (y * Columns + x < blocks.Length - 1)
                            Blocks[y, x] = blocks[y * Columns + x + 1];        
                }   
        }
    }
}
