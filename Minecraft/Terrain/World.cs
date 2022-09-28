using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Linq;

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
        public void AddChunk(Vector2 pos,Chunk chunk)
        {
            Chunks.TryAdd(pos, chunk);
        }
        public void PlaceEntity(EntityType entity,Vector2 pos)
        {
            //var entityBlocks = Entity.EntityPositions[entity];
            //
            //int x = (int)pos.X % Chunk.ChunkSize;
            //int z = (int)pos.Y % Chunk.ChunkSize;
            //int y = 0;
            //
            //while (selectedChunk.Blocks.ContainsKey(new Vector3(x, y, z)))
            //    y++;
            //
            //foreach(var block in entity)
            //    selectedChunk.Blocks.Add(new Vector3(x, y, z),new Block(true,blockType));
        }
        public void PlaceBlock(Vector3 pos)
        {
            int selectedChunkX = (int)(pos.X / Chunk.Size);
            int selectedChunkZ = (int)(pos.Y / Chunk.Size);

            var selectedChunk = Chunks[new Vector2(selectedChunkX, selectedChunkZ)];

            int x = (int)pos.X % Chunk.Size;
            int y = (int)pos.Y;
            int z = (int)pos.Z % Chunk.Size;
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
