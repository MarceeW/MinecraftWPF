using Minecraft.Game;
using Minecraft.Graphics;
using Minecraft.Terrain;
using OpenTK.Mathematics;
using System.Collections.Generic;
using Chunk = Minecraft.Terrain.Chunk;

namespace Minecraft.Render
{
    internal class WorldRenderer
    {
        public static int RenderDistance = 12;
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

            foreach (var chunk in world.Chunks.Values)
                chunk.Mesh.RenderSolidMesh(Shader);

            foreach (var chunk in world.Chunks.Values)
                chunk.Mesh.RenderTransparentMesh(Shader);

            RenderSelectedBlockFrame();
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
        private void RenderSelectedBlockFrame()
        {
            var blockHitPos = Ray.Cast(camera, world, out bool hit, out FaceDirection hitFace);

            if (hit)
            {
                WireFrame.Render(blockHitPos, hitFace, new Vector3(0.0f));
            }
        }
    }
}
