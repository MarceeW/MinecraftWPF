using Minecraft.Graphics;
using OpenTK.Mathematics;
using System;

namespace Minecraft.Terrain
{
    [Serializable]
    internal class Chunk : IChunk
    {
        public const int Size = 16;
        public const int Height = 256;
        public Vector2 Position { get; private set; }
        public ChunkMesh Mesh
        {
            get
            {
                return mesh;
            }
            set
            {
                mesh = value;
            }
        }
        public byte[,,] Blocks { get; private set; }
        public int[,] TopBlockPositions { get; private set; }
        [NonSerialized]
        private ChunkMesh mesh;
        public Chunk(Vector2 pos)
        {
            Position = pos;
            Blocks = new byte[Size, Height, Size];
            TopBlockPositions = new int[Size, Size];

            mesh = new ChunkMesh();
        }
        public void AddBlock(Vector3 pos, BlockType block, bool overWrite)
        {
            if (pos.Y >= Height)
                return;

            int x = (int)pos.X % Size;
            int y = (int)pos.Y;
            int z = (int)pos.Z % Size;

            if (x < 0)
                x += Size;

            if (z < 0)
                z += Size;

            if (y > TopBlockPositions[x, z])
                TopBlockPositions[x, z] = y;

            if (Blocks[x, y, z] == 0 || overWrite)
            {
                Blocks[x, y, z] = (byte)block;

                if (y > 0 && Blocks[x, y - 1, z] == (byte)BlockType.GrassBlock && !BlockData.IsVegetationBlock(block))
                    Blocks[x, y - 1, z] = (byte)BlockType.Dirt;
            }
        }
        public void RemoveBlock(Vector3 pos)
        {
            if (pos.Y <= 0)
                return;

            int x = (int)pos.X % Size;
            int y = (int)pos.Y;
            int z = (int)pos.Z % Size;

            if (x < 0)
                x += Size;

            if (z < 0)
                z += Size;

            Vector3 aboveBlockPos = new Vector3(pos.X, pos.Y + 1, pos.Z);
            var block = GetBlock(aboveBlockPos);

            if (block != null && !BlockData.IsBlockSolid(block) && !BlockData.IsVegetationBlock(block))
            {
                Blocks[x, y, z] = (byte)block;
            }
            else if (BlockData.IsVegetationBlock(block))
            {
                Blocks[x, y, z] = 0;
                RemoveBlock(aboveBlockPos);
            }
            else
                Blocks[x, y, z] = 0;

            if (TopBlockPositions[x, z] == y)
            {
                while (y > 0 && Blocks[x, y, z] == 0)
                    y--;

                TopBlockPositions[x, z] = y;
            }
        }
        public BlockType? GetBlock(Vector3 pos)
        {
            if (pos.Y >= 0 && pos.Y < Height)
            {
                int x = (int)pos.X;
                int y = (int)pos.Y;
                int z = (int)pos.Z;

                if (pos.X - x < 0)
                    x--;
                if (pos.Z - z < 0)
                    z--;

                x %= Size;
                z %= Size;

                if (x < 0)
                    x += Size;

                if (z < 0)
                    z += Size;

                return (BlockType)Blocks[x, y, z];
            }
            return null;
        }
        public bool IsBlockPosInChunk(Vector3 pos)
        {
            int bx = (int)Position.X * Size;
            int bz = (int)Position.Y * Size;

            int tx = (int)(Position.X + 1) * Size;
            int tz = (int)(Position.Y + 1) * Size;

            return pos.X >= bx && pos.X < tx && pos.Z >= bz && pos.Z < tz;
        }
    }
}