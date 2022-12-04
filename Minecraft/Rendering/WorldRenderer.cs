using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Game;
using Minecraft.Graphics;
using Minecraft.Terrain;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Chunk = Minecraft.Terrain.Chunk;

namespace Minecraft.Render
{
    internal class WorldRenderer : ObservableObject, IDisposable
    {
        public static BlockType? CurrentTarget { get; private set; }
        public Action<int>? RenderDistanceChanged;
        public static Vector2 RenderFrustumCenter;
        public int RenderDistance { get => renderDistance; set { SetProperty(ref renderDistance, value); if(meshGenerator != null) meshGenerator.RenderDistance = renderDistance; OnRenderSizeChanged(); } }
        private int renderDistance;

        private ICamera camera;
        public Shader Shader { get; }
        private IWorld world;
        private MeshGenerator meshGenerator;

        private Queue<ChunkMeshRawData> finishedChunkMeshes;
        public WorldRenderer()
        {
            camera = Ioc.Default.GetService<ICamera>();

            Shader = new Shader(@"..\..\..\Graphics\Shaders\Block\blockVert.glsl", @"..\..\..\Graphics\Shaders\Block\blockFrag.glsl");
            Shader.SetDouble("renderDistance", renderDistance);

            finishedChunkMeshes = new Queue<ChunkMeshRawData>();
        }
        public void OnRenderSizeChanged()
        {
            RenderDistanceChanged?.Invoke(renderDistance);
            Shader.SetDouble("renderDistance", renderDistance);
        }
        public void SetWorld(IWorld world)
        {
            this.world = world;
            meshGenerator = new MeshGenerator(world, finishedChunkMeshes, renderDistance);
        }

        public void RenderWorld()
        {
            AtlasTexturesData.Atlas.Use();

            var frontNorm = camera.Front.Xz;
            frontNorm.Normalize();

            RenderFrustumCenter = frontNorm * (renderDistance - (camera.Position.Y / (2 * Chunk.Size))) * Chunk.Size + camera.Position.Xz;

            if (frontNorm.X > 0)
                RenderFrustumCenter.X -= Chunk.Size;
            if (frontNorm.Y > 0)
                RenderFrustumCenter.Y -= Chunk.Size;

            int chunksRendered = 0;

            foreach (var chunk in world.Chunks.Values)
                if (IsChunkInRange(chunk.Position, RenderFrustumCenter, renderDistance))
                {
                    chunk.Mesh.RenderSolidMesh(Shader);
                    chunksRendered++;
                }

            foreach (var chunk in world.Chunks.Values)
                if (IsChunkInRange(chunk.Position, RenderFrustumCenter, renderDistance))
                    chunk.Mesh.RenderTransparentMesh(Shader);

            RenderSelectedBlockFrame();
            LoadFinishedChunksToGPU();
        }
        public static bool IsChunkInRange(in Vector2 chunkPos, Vector2 rangeCenter,int renderDistance)
        {
            float xDistance = Math.Abs(rangeCenter.X - chunkPos.X * Chunk.Size);
            float zDistance = Math.Abs(rangeCenter.Y - chunkPos.Y * Chunk.Size);

            return xDistance <= renderDistance * Chunk.Size && zDistance <= renderDistance * Chunk.Size;

        }
        public void Dispose()
        {
            meshGenerator.ShouldRun = false;
            meshGenerator = null;
        }
        private void LoadFinishedChunksToGPU()
        {
            while(finishedChunkMeshes.Count > 0)
            {
                var rawData = finishedChunkMeshes.Dequeue();
                rawData.Owner.Mesh.LoadToGPU(rawData);
            }
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
