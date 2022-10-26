using Minecraft.Logic;
using OpenTK.Mathematics;
using System;

namespace Minecraft.Terrain
{
    internal interface IWorldGenerator
    {
        event Action<Vector2>? ChunkAdded;
        int RenderDistance { get; set; }

        void AddGeneratedChunksToWorld(float delta);
        void ExpandWorld(Direction dir, Vector2 position);
        void GenerateChunksToQueue();
        BlockType GetBlockAtHeight(int y, int depth = 0);
        int GetHeightAtPosition(Vector2 pos);
        void InitWorld();
    }
}