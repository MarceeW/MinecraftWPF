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
        public void AddBlock(Vector3 pos,BlockType block,bool overWrite)
        {
            if (pos.Y >= 256)
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

            if(Blocks[x, y, z] == 0 || overWrite)
                Blocks[x, y, z] = (byte)block;
        }
        public void RemoveBlock(Vector3 pos)
        {
            if (pos.Y <= 1)
                return;

            int x = (int)pos.X % Size;
            int y = (int)pos.Y;
            int z = (int)pos.Z % Size;

            if (x < 0)
                x += Size;

            if (z < 0)
                z += Size;

            var block = GetBlock(new Vector3(pos.X,pos.Y + 1,pos.Z));

            if (block != null && !BlockData.IsBlockSolid(block))
            {
                Blocks[x, y, z] = (byte)block;
            }
            else
                Blocks[x, y, z] = 0;

            if (TopBlockPositions[x,z] == y)
            {
                while (y > 0 && Blocks[x, y, z] > 0)
                    y--;

                TopBlockPositions[x, z] = y;
            }
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
        public BlockType? GetBlock(Vector3 pos)
        {
            if(pos.Y >= 0 && pos.Y < 256)
            {
                int x = (int)pos.X % Size;
                int z = (int)pos.Z % Size;

                if (x < 0)
                    x += Size;
                
                if (z < 0)
                    z += Size;

                return (BlockType)Blocks[x, (int)pos.Y, z];
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