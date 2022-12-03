using Minecraft.Render;
using Minecraft.Terrain;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Minecraft.Graphics
{
    internal class MeshGenerator
    {
        public bool ShouldRun { get; set; }
        private Thread generatorThread;
        public static PriorityQueue<Vector2, float> GeneratorQueue { get; } = new PriorityQueue<Vector2, float>();
        internal int RenderDistance { get; set; }
        private Queue<ChunkMeshRawData> createdMeshes;

        private IWorld world;
        private static ICamera camera;
        private static bool canAccessGeneratorQueue = true;

        public MeshGenerator(IWorld world, Queue<ChunkMeshRawData> createdMeshes, ICamera _camera,int renderDistance)
        {
            this.world = world;
            this.createdMeshes = createdMeshes;
            camera = _camera;
            RenderDistance = renderDistance;

            ShouldRun = true;

            if (world.Chunks.Count > 0)
            {
                foreach (var chunk in world.Chunks)
                {
                    chunk.Value.Mesh = new ChunkMesh();
                    AddToQueue(chunk.Key);
                }
            }

            generatorThread = new Thread(GeneratorLoop);
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
            canAccessGeneratorQueue = false;
            GeneratorQueue.Enqueue(toRender, (toRender - camera.Position.Xz / Chunk.Size).Length);
            canAccessGeneratorQueue = true;
        }
        private void CreateMeshes(Vector2 rangeCenter)
        {
            if (GeneratorQueue.Count > 0)
            {
                List<Vector2> toPlaceBack = new List<Vector2>();
            
                while (canAccessGeneratorQueue && GeneratorQueue.Count > 0)
                {
                    var chunkPos = GeneratorQueue.Dequeue();
            
                    if (WorldRenderer.IsChunkInRange(chunkPos, rangeCenter, RenderDistance))
                    {
                        CreateMesh(chunkPos);
                        break;
                    }
                    else
                        toPlaceBack.Add(chunkPos);
                }
                foreach (var chunk in toPlaceBack)
                    GeneratorQueue.Enqueue(chunk, (chunk - camera.Position.Xz / Chunk.Size).Length);
            }
            if (world.ChunksNeedsToBeRegenerated.Count > 0)
                CreateMesh(world.ChunksNeedsToBeRegenerated.Dequeue()); 
        }
        private void GeneratorLoop()
        {
            while (ShouldRun)
            {
                if(GeneratorQueue.Count > 0)
                {
                    CreateMeshes(WorldRenderer.RenderFrustumCenter);
                }
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

            createdMeshes.Enqueue(new ChunkMeshRawData(nVertices, tVertices, vVertices, chunk,nFaceCount,tFaceCount,vFaceCount));
        }
        private bool BlockNeedsShadow(int x, int y, int z, IChunk chunk)
        {
            return y < chunk.TopBlockPositions[x, z] && !BlockData.IsVegetationBlock(chunk.GetBlock(new Vector3(x, chunk.TopBlockPositions[x, z], z)))
                || y < chunk.TopBlockPositions[x, z] - 1;
        }
    }
}
