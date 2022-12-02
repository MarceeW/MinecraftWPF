using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Game;
using Minecraft.Graphics;
using Minecraft.Terrain;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using Chunk = Minecraft.Terrain.Chunk;

namespace Minecraft.Render
{
    internal class WorldRenderer : ObservableObject
    {
        public static BlockType? CurrentTarget { get; private set; }
        public Action<int>? RenderDistanceChanged;
        public int RenderDistance { get => renderDistance; set { SetProperty(ref renderDistance, value); OnRenderSizeChanged(); } }
        private int renderDistance;

        private PriorityQueue<Vector2, float> renderQueue;
        private ICamera camera;
        public Shader Shader { get; }
        private IWorld world;
        public WorldRenderer()
        {
            camera = Ioc.Default.GetService<ICamera>();

            renderQueue = new PriorityQueue<Vector2, float>();
            Shader = new Shader(@"..\..\..\Graphics\Shaders\Block\blockVert.glsl", @"..\..\..\Graphics\Shaders\Block\blockFrag.glsl");
            Shader.SetDouble("renderDistance", renderDistance);
        }
        public void OnRenderSizeChanged()
        {
            RenderDistanceChanged?.Invoke(renderDistance);
            Shader.SetDouble("renderDistance", renderDistance);
        }
        public void SetWorld(IWorld world)
        {
            this.world = world;

            if (world.Chunks.Count > 0)
            {
                foreach (var chunk in world.Chunks)
                {
                    chunk.Value.Mesh = new ChunkMesh();
                    AddToQueue(chunk.Key);
                }
            }
        }
        public void AddToQueue(Vector2 toRender)
        {
            renderQueue.Enqueue(toRender, (toRender - camera.Position.Xz / Chunk.Size).Length);
        }
        public void RenderWorld()
        {
            AtlasTexturesData.Atlas.Use();

            var frontNorm = camera.Front.Xz;
            frontNorm.Normalize();

            Vector2 rangeCenter = frontNorm * (renderDistance - renderDistance < 4 ? 0 : (float)Math.Round(Math.Abs(camera.Front.Y),2) * camera.Position.Y / (2 * Chunk.Size)) * Chunk.Size + camera.Position.Xz;

            if (frontNorm.X > 0)
                rangeCenter.X -= Chunk.Size;
            if (frontNorm.Y > 0)
                rangeCenter.Y -= Chunk.Size;

            int chunksRendered = 0;

            foreach (var chunk in world.Chunks.Values)
                if (IsChunkInRange(chunk.Position, rangeCenter))
                {
                    chunk.Mesh.RenderSolidMesh(Shader);
                    chunksRendered++;
                }

            foreach (var chunk in world.Chunks.Values)
                if (IsChunkInRange(chunk.Position, rangeCenter))
                    chunk.Mesh.RenderTransparentMesh(Shader);

            //Debug.WriteLine(chunksRendered);

            RenderSelectedBlockFrame();

            CreateMeshesInQueue(rangeCenter);
        }
        public void CreateMeshesInQueue(Vector2 rangeCenter)
        {
            if (renderQueue.Count > 0)
            {
                List<Vector2> toPlaceBack = new List<Vector2>();
                
                while(renderQueue.Count > 0)
                {
                    var chunkPos = renderQueue.Dequeue();

                    if(IsChunkInRange(chunkPos, rangeCenter))
                    {
                        ChunkMesh.CreateMesh(world, chunkPos);
                        break;
                    }
                    else
                        toPlaceBack.Add(chunkPos);
                }
                foreach (var chunk in toPlaceBack)
                    AddToQueue(chunk);
            }     

            if (world.ChunksNeedsToBeRegenerated.Count > 0)
                ChunkMesh.CreateMesh(world, world.ChunksNeedsToBeRegenerated.Dequeue());
        }
        private bool IsChunkInRange(in Vector2 chunkPos,Vector2 rangeCenter)
        {
            float xDistance = Math.Abs(rangeCenter.X - chunkPos.X * Chunk.Size);
            float zDistance = Math.Abs(rangeCenter.Y - chunkPos.Y * Chunk.Size);
            
            return xDistance <= renderDistance * Chunk.Size && zDistance <= renderDistance * Chunk.Size;

        }
        private void RenderSelectedBlockFrame()
        {
            var blockHitPos = Ray.Cast(world, out bool hit, out FaceDirection hitFace, out double rayDistance);

            CurrentTarget = world.GetBlock(blockHitPos);

            if (hit)
                LineRenderer.WireWrame(blockHitPos, new Vector3(0.0f));
        }
    }
}
