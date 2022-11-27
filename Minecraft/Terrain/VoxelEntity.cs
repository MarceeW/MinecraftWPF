namespace Minecraft.Terrain
{
    class VoxelEntity
    {
        public VoxelEntity(int width, int height, int depth, Block[] blocks)
        {
            Width = width;
            Height = height;
            Depth = depth;
            Blocks = blocks;
        }

        public int Width { get; }
        public int Height { get; }
        public int Depth { get; }
        public Block[] Blocks { get; }
    }
}
