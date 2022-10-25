using Minecraft.Graphics;
using OpenTK.Mathematics;

namespace Minecraft.Terrain
{
    internal interface IChunk
    {
        byte[,,] Blocks { get; }
        ChunkMesh Mesh { get; set; }
        Vector2 Position { get; }
        int[,] TopBlockPositions { get; }

        void AddBlock(Vector3 pos, BlockType block, bool overWrite);
        BlockType? GetBlock(Vector3 pos);
        bool IsBlockPosInChunk(Vector3 pos);
        void RemoveBlock(Vector3 pos);
    }
}