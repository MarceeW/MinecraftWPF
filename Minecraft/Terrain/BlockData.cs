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
                   type == BlockType.LimeStainedGlass ||
                   type == BlockType.OakLeaves||
                   type == BlockType.BirchLeaves ||
                   type == BlockType.AcaciaLeaves ||
                   type == BlockType.SpruceLeaves ||
                   type == BlockType.Air ||
                   type == BlockType.Scaffolding ||
                   
                   IsVegetationBlock(type);
        }
        public static bool IsVegetationBlock(BlockType? type)
        {
            return type == BlockType.Grass ||
                   type == BlockType.SparseGrass ||
                   type == BlockType.Allium ||
                   type == BlockType.Poppy ||
                   type == BlockType.DeadBush ||
                   type == BlockType.OakSapling ||
                   type == BlockType.BirchSapling ||
                   type == BlockType.AcaciaSapling ||
                   type == BlockType.SpruceSapling ||
                   type == BlockType.Torch ||
                   type == BlockType.RedstoneTorch ||
                   type == BlockType.RedMushroom ||
                   type == BlockType.BrownMushroom ||
                   type == BlockType.CobWeb ||
                   type == BlockType.BlueOrchid ||
                   type == BlockType.AzureBluet ||
                   type == BlockType.AmethystCluster ||
                   type == BlockType.Fire;
        }
    }
}