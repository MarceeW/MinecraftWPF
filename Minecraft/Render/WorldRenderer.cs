using Minecraft.Graphics;
using Minecraft.Terrain;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Minecraft.Render
{
    internal class WorldRenderer
    {
        public static int RenderDistance = 16;
        private PriorityQueue<Vector2,float> renderQueue;
        private Camera camera;
        public Shader Shader { get; }
        private World world;

        public WorldRenderer(World world,Camera camera)
        {
            this.world = world;
            this.camera = camera;

            renderQueue = new PriorityQueue<Vector2,float>();
            Shader = new Shader(@"..\..\..\Graphics\Shaders\Block\blockVert.glsl", @"..\..\..\Graphics\Shaders\Block\blockFrag.glsl");
        }
        public void AddToQueue(Vector2 toRender)
        {
            renderQueue.Enqueue(toRender, (toRender - camera.Position.Xz / Chunk.Size).Length);
        }
        public void RenderWorld()
        {
            AtlasTexturesData.Atlas.Use();
            world.OrderByPlayerPosition(camera.Position.Xz - camera.Front.Xz * Chunk.Size);

            foreach (var chunk in world.Chunks.Values)
            {
                chunk.Mesh.Render(Shader);
            }
            CreateMeshesInQueue();
        }
        public void CreateMeshesInQueue()
        {
            if (renderQueue.Count > 0)
            {
                var chunk = renderQueue.Dequeue();
                    ChunkMesh.CreateMesh(world, chunk);
            }             
        }
    }
}
