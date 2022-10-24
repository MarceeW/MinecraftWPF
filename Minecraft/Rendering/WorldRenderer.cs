using Assimp.Unmanaged;
using Minecraft.Game;
using Minecraft.Graphics;
using Minecraft.Terrain;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Chunk = Minecraft.Terrain.Chunk;

namespace Minecraft.Render
{
    internal class WorldRenderer
    {
        public static BlockType? CurrentTarget { get; private set; }
        public static int RenderDistance = 8;
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
            Shader.SetDouble("renderDistance", RenderDistance);
        }
        public void AddToQueue(Vector2 toRender)
        {
            renderQueue.Enqueue(toRender, (toRender - camera.Position.Xz / Chunk.Size).Length);
        }
        public void RenderWorld()
        {
            AtlasTexturesData.Atlas.Use();

            foreach (var chunk in world.Chunks.Values)
                if(IsChunkInRange(chunk))
                    chunk.Mesh.RenderSolidMesh(Shader);

            foreach (var chunk in world.Chunks.Values)
                if (IsChunkInRange(chunk))
                    chunk.Mesh.RenderTransparentMesh(Shader);

            RenderSelectedBlockFrame();

            //LineRenderer.WireWrame(camera.Position - new Vector3(0.5f), new Vector3(0.0f));
            //LineRenderer.Axes(camera.Position - new Vector3(0.5f));

            CreateMeshesInQueue(0);
        }
        public void CreateMeshesInQueue(float delta)
        {
            if (renderQueue.Count > 0)
            {
                var chunk = renderQueue.Dequeue();
                    ChunkMesh.CreateMesh(world, chunk);
            }             
        }
        private bool IsChunkInRange(in Chunk chunk)
        {
            float xDistance = Math.Abs(camera.Position.X - chunk.Position.X * Chunk.Size);
            float zDistance = Math.Abs(camera.Position.Z - chunk.Position.Y * Chunk.Size);

            return xDistance <= RenderDistance * Chunk.Size && zDistance <= RenderDistance * Chunk.Size;
        }
        private void RenderSelectedBlockFrame()
        {
            var blockHitPos = Ray.Cast(camera, world, out bool hit, out FaceDirection hitFace);

            CurrentTarget = world.GetBlock(blockHitPos);

            if (hit)
            {
                LineRenderer.WireWrame(blockHitPos, new Vector3(0.0f));
            }
        }
    }
}
