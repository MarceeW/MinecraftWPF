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

        private Dictionary<Vector2, List<Block>> blocksWaitingForChunk;

        public World()
        {
            Chunks = new Dictionary<Vector2, Chunk>();
            blocksWaitingForChunk = new Dictionary<Vector2, List<Block>>();
        }
        public Chunk? GetChunk(Vector3 pos,out Vector2 chunkPos)
        {
            int chunkPosX = (int)(pos.X / Chunk.Size);
            int chunkPosZ = (int)(pos.Z / Chunk.Size);

            if (pos.X < 0 && (int)pos.X % Chunk.Size != 0)
                chunkPosX--;
            if (pos.Z < 0 && (int)pos.Z % Chunk.Size != 0)
                chunkPosZ--;

            chunkPos = new Vector2(chunkPosX, chunkPosZ);

            return Chunks.GetValueOrDefault(new Vector2(chunkPosX, chunkPosZ));
        }
        public BlockType? GetBlock(Vector3 pos)
        {
            var chunk = GetChunk(pos,out Vector2 chunkPos);

            if(chunk != null)
            {
                return chunk.GetBlock(pos);
            }

            return null;
        }
        public void AddChunk(Vector2 pos,Chunk chunk)
        {  
            if(Chunks.TryAdd(pos, chunk))
            {
                if (blocksWaitingForChunk.ContainsKey(pos))
                {
                    foreach (var block in blocksWaitingForChunk[pos])
                    {
                        AddBlock(block.Position, block.Type);
                    }
                    blocksWaitingForChunk.Remove(pos);
                }
            }
        }
        public void RemoveBlock(Vector3 pos)
        {
            var chunk = GetChunk(pos, out Vector2 chunkPos);

            if (chunk != null)
            {
                chunk.RemoveBlock(pos);
                ChunkMesh.CreateMesh(this, chunk.Position);
            }
        }
        public void AddBlock(Vector3 pos,BlockType block)
        {
            var chunk = GetChunk(pos,out Vector2 chunkPos);

            if (chunk != null)
            {
                chunk.AddBlock(pos, block, true);
                ChunkMesh.CreateMesh(this, chunk.Position);
            }
        }
        public void AddEntity(Vector3 position,EntityType entityType, Chunk chunk)
        {
            var entity = EntityData.Entities[(int)entityType];

            foreach(var block in entity.Blocks)
            {
                if (chunk.IsBlockPosInChunk(block.Position + position))
                    chunk.AddBlock(block.Position + position, block.Type, false);
                else
                {
                    var neighborChunk = GetChunk(block.Position, out Vector2 chunkPos);
                
                    if(neighborChunk != null)
                    {
                        neighborChunk.AddBlock(block.Position + position, block.Type, false);
                    }
                    //else
                    //{
                    //    if (!blocksWaitingForChunk.ContainsKey(chunkPos))
                    //    {
                    //        blocksWaitingForChunk.Add(chunkPos, new List<Block>());
                    //    }
                    //    blocksWaitingForChunk[chunkPos].Add(new Block(block.Position + position,block.Type));
                    //} 
                }
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
