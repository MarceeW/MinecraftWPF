using Minecraft.Graphics;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace Minecraft.Terrain
{
    internal class World
    {
        public Dictionary<Vector2, Chunk> Chunks { get; private set; }
        public WorldGenerator? WorldGenerator { get; set; }

        public World()
        {
            Chunks = new Dictionary<Vector2, Chunk>();
        }
        public Chunk? GetChunk(Vector3 pos)
        {
            int chunkPosX = (int)(pos.X / Chunk.Size);
            int chunkPosZ = (int)(pos.Z / Chunk.Size);

            if (pos.X < 0 && (int)pos.X % Chunk.Size != 0)
                chunkPosX--;
            if (pos.Z < 0 && (int)pos.Z % Chunk.Size != 0)
                chunkPosZ--;

            return Chunks.GetValueOrDefault(new Vector2(chunkPosX, chunkPosZ));
        }
        public BlockType? GetBlock(Vector3 pos)
        {

            var chunk = GetChunk(pos);

            if(chunk != null)
            {
                return chunk.GetBlock(pos);
            }

            return null;
        }
        public void AddChunk(Vector2 pos,Chunk chunk)
        {
            Chunks.TryAdd(pos, chunk);
        }
        public void RemoveBlock(Vector3 pos)
        {
            var chunk = GetChunk(pos);

            if (chunk != null)
            {
                chunk.RemoveBlock(pos);
                ChunkMesh.CreateMesh(this, chunk.Position);
            }
        }
        public void AddBlock(Vector3 pos,BlockType block)
        {
            var chunk = GetChunk(pos);

            if (chunk != null)
            {
                chunk.AddBlock(pos, block);
                ChunkMesh.CreateMesh(this, chunk.Position);
            }

        }
        public void OrderByPlayerPosition(Vector2 position)
        {
            position /= Chunk.Size;
            try
            {
                Chunks = Chunks.OrderByDescending(x => (x.Key - position).Length).ToDictionary(x => x.Key, y => y.Value);
            }
            catch
            {
                return;
            }
        }
    }
}
