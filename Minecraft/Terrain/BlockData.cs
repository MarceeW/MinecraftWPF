using System;

namespace Minecraft.Terrain
{
    internal static class BlockData
    {
        public static bool IsBlockSolid(BlockType? type)
        {
            return type != BlockType.Water && type != BlockType.Air && type != null && !IsVegetationBlock(type);
        }
        public static bool IsBolckTransparent(BlockType? type)
        {
            return type == BlockType.Water ||
                   type == BlockType.Glass ||
                   type == BlockType.BlueStainedGlass ||
                   type == BlockType.BrownStainedGlass ||
                   type == BlockType.CyanStainedGlass ||
                   type == BlockType.GreenStainedGlass ||
                   type == BlockType.MagentaStainedGlass||
                   type == BlockType.PurpleStainedGlass ||
                   type == BlockType.RedStainedGlass ||
                   type == BlockType.OakLeaves||
                   type == BlockType.BirchLeaves ||
                   type == BlockType.Air ||
                   
                   IsVegetationBlock(type);
        }
        public static bool IsVegetationBlock(BlockType? type)
        {
            return type == BlockType.Grass ||
                   type == BlockType.SparseGrass ||
                   type == BlockType.Allium ||
                   type == BlockType.Poppy ||
                   type == BlockType.DeadBush;
        }
    }
}