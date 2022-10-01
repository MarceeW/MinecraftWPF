using System;

namespace Minecraft.Terrain
{
    internal enum BlockType
    {
        Air, Grass, Stone, Dirt, Leaves, OakTrunk, Glass, Sand, Water, Bedrock, Cobblestone, WoodPlank
    }
    internal static class BlockData
    {
        public static bool IsBlockSolid(BlockType? type)
        {
            return type != BlockType.Water && type != BlockType.Air && type != null;
        }
        public static bool IsBolckTransparent(BlockType? type)
        {
            return type == BlockType.Water ||
                   type == BlockType.Glass ||
                   type == BlockType.Air;
        }
        public static BlockType GetBlockTypeByName(string name)
        {
            foreach(var type in (BlockType[])Enum.GetValues(typeof(BlockType)))
            {
                if (type.ToString() == name)
                    return type;
            }
            throw new Exception($"Blocktype '{name}' was not found");
        }
    }
}