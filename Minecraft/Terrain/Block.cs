using OpenTK.Mathematics;

namespace Minecraft.Terrain
{
    internal struct Block
    {
        public Block(Vector3 position, BlockType type)
        {
            Position = position;
            Type = type;
        }

        public Vector3 Position { get; }
        public BlockType Type { get; }

    }
}
