using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Terrain;
using OpenTK.Mathematics;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Minecraft.Graphics
{
    internal class MeshGenerator
    {
        public bool ShouldRun { get; set; }
        private Thread generatorThread;

        private static ConcurrentQueue<Vector2> GeneratorQueue = new ConcurrentQueue<Vector2>();
        internal int RenderDistance { get; set; }
        private ConcurrentQueue<ChunkMeshRawData> createdMeshes;

        private IWorld world;

        public MeshGenerator(IWorld world, ConcurrentQueue<ChunkMeshRawData> createdMeshes, int renderDistance)
        {
            this.world = world;
            this.createdMeshes = createdMeshes;
            RenderDistance = renderDistance;

            ShouldRun = true;

            if (world.Chunks.Count > 0)
            {
                var cameraPos = Ioc.Default.GetService<ICamera>().Position.Xz / 16;
                PriorityQueue<Vector2, float> renderPriorityQueue = new PriorityQueue<Vector2, float>();

                foreach (var chunk in world.Chunks)
                {
                    chunk.Value.Mesh = new ChunkMesh();
                    renderPriorityQueue.Enqueue(chunk.Key, (chunk.Key - cameraPos).Length);
                }
                while (renderPriorityQueue.Count > 0)
                    AddToQueue(renderPriorityQueue.Dequeue());
            }

            generatorThread = new Thread(GeneratorLoop);
            generatorThread.Priority = ThreadPriority.BelowNormal;
            generatorThread.Start();
        }
        public static bool BlockIsOnBorder(Vector2 chunkPos, Vector3 blockPos)
        {
            float chunkLeftCorner = chunkPos.X * Chunk.Size;
            float chunkRightCorner = (chunkPos.X + 1) * Chunk.Size - 1;

            float chunkBotCorner = chunkPos.Y * Chunk.Size;
            float chunkTopCorner = (chunkPos.Y + 1) * Chunk.Size - 1;

            return blockPos.X == chunkLeftCorner ||
                   blockPos.X == chunkRightCorner ||
                   blockPos.Z == chunkBotCorner ||
                   blockPos.Z == chunkTopCorner;
        }
        public static void AddToQueue(Vector2 toRender)
        {
            GeneratorQueue.Enqueue(toRender);
        }
        private void CreateMeshes()
        {

            if (GeneratorQueue.Count > 0 && world.ChunksNeedsToBeRegenerated.Count == 0)
            {
                if (GeneratorQueue.TryDequeue(out Vector2 chunk))
                    CreateMesh(chunk);
            }

            if (world.ChunksNeedsToBeRegenerated.Count > 0)
                CreateMesh(world.ChunksNeedsToBeRegenerated.Dequeue());
        }
        private void GeneratorLoop()
        {
            while (ShouldRun)
            {
                CreateMeshes();
            }
        }
        private void CreateMesh(Vector2 target)
        {
            var chunk = world.Chunks.GetValueOrDefault(target);

            if (chunk == null)
                return;


            List<float> nVertices = new List<float>();
            List<float> tVertices = new List<float>();
            List<float> vVertices = new List<float>();

            int nFaceCount = 0;
            int tFaceCount = 0;
            int vFaceCount = 0;

            for (int x = 0; x < Chunk.Size; x++)
                for (int z = 0; z < Chunk.Size; z++)
                    for (int y = 0; y <= chunk.TopBlockPositions[x, z]; y++)
                    {
                        Vector3 blockPos = new Vector3(x + Chunk.Size * target.X, y, z + Chunk.Size * target.Y);

                        var block = chunk.GetBlock(blockPos);

                        if (block == BlockType.Air)
                            continue;

                        foreach (var face in FaceDirectionVectors.Vectors)
                        {
                            if (y == 0 && face.Key != FaceDirection.Top)
                                continue;

                            Vector3 neighborPos = blockPos + face.Value;

                            if (BlockIsOnBorder(target, blockPos))
                            {
                                if (chunk.IsBlockPosInChunk(neighborPos))
                                {
                                    var neighborBlock = chunk.GetBlock(neighborPos);

                                    if (neighborBlock == null || (BlockData.IsBolckTransparent(neighborBlock) && !BlockData.IsBolckTransparent(block)))
                                    {
                                        nVertices.AddRange(Face.GetBlockFaceVertices(block, face.Key, blockPos, BlockNeedsShadow(x, y, z, chunk), chunk.TopBlockPositions[x, z] - y));
                                        nFaceCount++;
                                    }
                                    else if (BlockData.IsBolckTransparent(block) && neighborBlock == 0 || BlockData.IsBolckTransparent(block) && BlockData.IsVegetationBlock(neighborBlock) || BlockData.IsBolckTransparent(block) && block != BlockType.Water && block != BlockType.Lava && BlockData.IsBolckTransparent(neighborBlock) || (block == BlockType.Water && face.Key == FaceDirection.Top && neighborBlock != BlockType.Water))
                                    {
                                        if (BlockData.IsVegetationBlock(block))
                                        {
                                            vVertices.AddRange(Face.GetVegetationFaceVertices((BlockType)block, blockPos, BlockNeedsShadow(x, y, z, chunk), chunk.TopBlockPositions[x, z] - y));
                                            vFaceCount++;
                                        }
                                        else
                                        {
                                            tVertices.AddRange(Face.GetBlockFaceVertices(block, face.Key, blockPos, BlockNeedsShadow(x, y, z, chunk), chunk.TopBlockPositions[x, z] - y));
                                            tFaceCount++;
                                        }

                                    }
                                }
                                else
                                {
                                    var neighborChunk = world.Chunks.GetValueOrDefault(target + face.Value.Xz);

                                    if (neighborChunk == null && !BlockData.IsBolckTransparent(block))
                                    {
                                        if (world.WorldGenerator != null)
                                        {
                                            int topBlockY = world.WorldGenerator.GetHeightAtPosition(neighborPos.Xz);

                                            if (topBlockY < neighborPos.Y)
                                            {
                                                nVertices.AddRange(Face.GetBlockFaceVertices(block, face.Key, blockPos, BlockNeedsShadow(x, y, z, chunk), chunk.TopBlockPositions[x, z] - y));
                                                nFaceCount++;
                                            }
                                        }
                                    }
                                    else if (neighborChunk != null)
                                    {
                                        var neighborBlock = neighborChunk.GetBlock(neighborPos);

                                        if (BlockData.IsBolckTransparent(neighborBlock) && !BlockData.IsBolckTransparent(block))
                                        {
                                            nVertices.AddRange(Face.GetBlockFaceVertices(block, face.Key, blockPos, BlockNeedsShadow(x, y, z, chunk), chunk.TopBlockPositions[x, z] - y));
                                            nFaceCount++;
                                        }
                                        else if (BlockData.IsBolckTransparent(block) && neighborBlock == 0 || BlockData.IsBolckTransparent(block) && BlockData.IsVegetationBlock(neighborBlock) || BlockData.IsBolckTransparent(block) && block != BlockType.Water && block != BlockType.Lava && BlockData.IsBolckTransparent(neighborBlock))
                                        {
                                            if (BlockData.IsVegetationBlock(block))
                                            {
                                                vVertices.AddRange(Face.GetVegetationFaceVertices((BlockType)block, blockPos, BlockNeedsShadow(x, y, z, chunk), chunk.TopBlockPositions[x, z] - y));
                                                vFaceCount++;
                                            }
                                            else
                                            {
                                                tVertices.AddRange(Face.GetBlockFaceVertices(block, face.Key, blockPos, BlockNeedsShadow(x, y, z, chunk), chunk.TopBlockPositions[x, z] - y));
                                                tFaceCount++;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var neighborBlock = chunk.GetBlock(neighborPos);

                                if (neighborBlock == null || BlockData.IsBolckTransparent(neighborBlock) && !BlockData.IsBolckTransparent(block))
                                {
                                    nVertices.AddRange(Face.GetBlockFaceVertices(block, face.Key, blockPos, BlockNeedsShadow(x, y, z, chunk), chunk.TopBlockPositions[x, z] - y));
                                    nFaceCount++;
                                }
                                else if (BlockData.IsBolckTransparent(block) && neighborBlock == 0 || BlockData.IsBolckTransparent(block) && BlockData.IsVegetationBlock(neighborBlock) || BlockData.IsBolckTransparent(block) && block != BlockType.Water && block != BlockType.Lava && BlockData.IsBolckTransparent(neighborBlock) || (block == BlockType.Water && face.Key == FaceDirection.Top && neighborBlock != BlockType.Water))
                                {
                                    if (BlockData.IsVegetationBlock(block))
                                    {
                                        vVertices.AddRange(Face.GetVegetationFaceVertices((BlockType)block, blockPos, BlockNeedsShadow(x, y, z, chunk), chunk.TopBlockPositions[x, z] - y));
                                        vFaceCount++;
                                    }
                                    else
                                    {
                                        tVertices.AddRange(Face.GetBlockFaceVertices(block, face.Key, blockPos, BlockNeedsShadow(x, y, z, chunk), chunk.TopBlockPositions[x, z] - y));
                                        tFaceCount++;
                                    }
                                }
                            }
                        }
                    }

            createdMeshes.Enqueue(new ChunkMeshRawData(nVertices, tVertices, vVertices, chunk, nFaceCount, tFaceCount, vFaceCount));
        }
        private bool BlockNeedsShadow(int x, int y, int z, IChunk chunk)
        {
            return y < chunk.TopBlockPositions[x, z] && !BlockData.IsVegetationBlock(chunk.GetBlock(new Vector3(x, chunk.TopBlockPositions[x, z], z)))
                || y < chunk.TopBlockPositions[x, z] - 1;
        }
    }
}
