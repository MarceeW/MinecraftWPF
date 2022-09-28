using Minecraft.Graphics;
using OpenTK.Mathematics;

namespace Minecraft.Terrain
{
    internal class Chunk
    {
        public const int Size = 16;
        public Vector2 Position { get; private set; }
        public ChunkMesh Mesh { get; }
        public byte[,,] Blocks { get; private set; }
        public int[,] TopBlockPositions { get; private set; }
        public Chunk(Vector2 pos)
        {
            Position = pos;
            Blocks = new byte[Size, 256, Size];
            TopBlockPositions = new int[Size, Size];

            Mesh = new ChunkMesh();
        }
        public void AddBlock(Vector3 pos,BlockType type)
        {
            int x = (int)pos.X % Size;
            int y = (int)pos.Y;
            int z = (int)pos.Z % Size;

            if (x < 0)
                x += Size;

            if (z < 0)
                z += Size;

            if (y > TopBlockPositions[x, z])
                TopBlockPositions[x, z] = y;

            Blocks[x, y, z] = (byte)type;
        }
        public short GetBlock(int x,int y,int z)
        {
            short block = -1;

            if (y >= 0 || y < 256)
            {
                int xs = x % Size;
                int zs = z % Size;

                if (xs < 0)
                    xs += Size;

                if (zs < 0)
                    zs += Size;

                return Blocks[xs, y, zs];
            }

            return block;
        }
        public short GetBlock(Vector3 pos)
        {
            short block = -1;

            if(pos.Y >= 0 && pos.Y < 256)
            {
                int x = (int)pos.X % Size;
                int z = (int)pos.Z % Size;

                if (x < 0)
                    x += Size;

                if (z < 0)
                    z += Size;

                return Blocks[x, (int)pos.Y, z];
            }
            return block;
        }
        public bool IsBlockInChunk(Vector3 pos)
        {
            int bx = (int)Position.X * Size;
            int bz = (int)Position.Y * Size;

            int tx = (int)(Position.X + 1) * Size;
            int tz = (int)(Position.Y + 1) * Size;

            return pos.X >= bx && pos.X < tx && pos.Z >= bz && pos.Z < tz;
        }
    }
}