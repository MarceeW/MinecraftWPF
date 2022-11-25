using Minecraft.Render;
using Minecraft.Logic;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Minecraft.Terrain.Noise;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Game;
using OpenTK.Platform.Windows;

namespace Minecraft.Terrain
{
    internal class WorldGenerator : IWorldGenerator
    {
        public event Action<Vector2>? ChunkAdded;
        public event Action? WorldInitalized;
        public int RenderDistance { get; set; }

        private FastNoise noise;

        private const int worldDepth = 32;
        private const int noiseDepth = 32;

        private IWorld world;
        private PriorityQueue<Vector2, float> generatorQueue;
        private Queue<KeyValuePair<Vector2, IChunk>> generatedChunks;
        private static Random random = new Random();
        private bool worldInit = false;

        public WorldGenerator(IWorld world,int renderDistance)
        {
            generatorQueue = new PriorityQueue<Vector2, float>();
            generatedChunks = new Queue<KeyValuePair<Vector2, IChunk>>();

            this.world = world;
            this.world.WorldGenerator = this;

            noise = new FastNoise();
            noise.SetSeed(world.Seed);
            noise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
            noise.SetInterp(FastNoise.Interp.Linear);
            noise.SetFractalOctaves(4);
            noise.SetFrequency(0.005f);
            noise.SetFractalGain(0.55f);
            noise.SetCellularJitter(0.005f);
            noise.SetFractalType(FastNoise.FractalType.FBM);

            if (world.Chunks.Count == 0)
                InitWorld(renderDistance);
            else
                worldInit = true;
        }
        public void AddGeneratedChunksToWorld(float delta)
        {
            while (generatedChunks.Count > 0)
            {
                var chunk = generatedChunks.Dequeue();
                world.AddChunk(chunk.Key, chunk.Value);
                ChunkAdded?.Invoke(chunk.Key);
            }
            if(generatedChunks.Count == 0 && !worldInit)
            {
                worldInit = true;
                WorldInitalized?.Invoke();
            }
        }
        public void GenerateChunksToQueue()
        {
            if (generatorQueue.Count > 0)
            {
                for (int maxGenerationInSingleRender = 0; maxGenerationInSingleRender < 16; maxGenerationInSingleRender++)
                {
                    AddChunk(generatorQueue.Dequeue());
                    if (generatorQueue.Count == 0)
                        break;
                }
            }
        }
        public int GetHeightAtPosition(Vector2 pos)
        {
            return (int)Math.Round((noise.GetValue(pos.X, pos.Y) + noise.GetSimplexFractal(pos.X, pos.Y)) / (1.0f / noiseDepth)) + worldDepth;
        }
        public BlockType GetBlockAtHeight(int y, int depth = 0)
        {
            if (y == 0)
                return BlockType.Bedrock;
            else if (depth > 5)
            {
                if (random.NextDouble() > 0.98)
                    return BlockType.CopperOre;
                else if (depth > 10 && random.NextDouble() > 0.98)
                    return BlockType.IronOre;
                else if (depth > 15 && random.NextDouble() > 0.98)
                    return BlockType.GoldOre;
                else if(depth > 25)
                {
                    if (random.NextDouble() > 0.98)
                        return BlockType.DiamondOre;
                    else if (random.NextDouble() > 0.99)
                        return BlockType.EmeraldOre;
                }
                return BlockType.Stone;
            }   
            else if (y < worldDepth - 8)
                return BlockType.Sand;
            else if (depth >= 1)
                return BlockType.Dirt;
            else
                return BlockType.GrassBlock;
        }
        public void ExpandWorld(Direction dir, Vector2 position)
        {
            int x = (int)position.X;
            int z = (int)position.Y;

            if (dir == Direction.Left)
            {
                AddChunkRange(0, RenderDistance, -RenderDistance, RenderDistance, x, z, -1, 1);
            }
            else if (dir == Direction.Right)
            {
                AddChunkRange(0, RenderDistance, -RenderDistance, RenderDistance, x, z, 1, 1);
            }
            else if (dir == Direction.Down)
            {
                AddChunkRange(-RenderDistance, RenderDistance, 0, RenderDistance, x, z, 1, -1);

            }
            else if (dir == Direction.Up)
            {
                AddChunkRange(-RenderDistance, RenderDistance, 0, RenderDistance, x, z, 1, 1);
            }
        }
        private void InitWorld(int renderDistance)
        {
            for (int x = -renderDistance; x <= renderDistance; x++)
                for (int z = -renderDistance; z <= renderDistance; z++)
                    lock (world)
                        AddChunk(new Vector2(x, z));
        }
        private void AddChunkRange(int fromXRange, int toXRgange, int fromZRange, int toZRange, int x, int z, int xSign, int zSign)
        {
            for (int xs = fromXRange; xs < toXRgange; xs++)
                for (int zs = fromZRange; zs < toZRange; zs++)
                {
                    Vector2 pos = new Vector2(x + xs * xSign, z + zs * zSign);
                    generatorQueue.Enqueue(pos, (pos - new Vector2(x, z)).Length);
                }
        }
        private void AddChunk(Vector2 position)
        {
            if (!world.Chunks.ContainsKey(position))
            {
                Chunk chunk = new Chunk(position);
                CreateChunk(chunk, position * Chunk.Size);
                generatedChunks.Enqueue(new KeyValuePair<Vector2, IChunk>(position, chunk));
            }
        }
        private void CreateChunk(IChunk chunk, Vector2 offset)
        {
            for (int x = 0; x < Chunk.Size; x++)
            {
                for (int z = 0; z < Chunk.Size; z++)
                {
                    int y = GetHeightAtPosition(new Vector2(x + offset.X, z + offset.Y));

                    int depth = 0;

                    if (y < worldDepth - 5)
                    {
                        if (y < 0)
                            y *= -1;
                        else if (y == 0)
                            y++;

                        for (int waterY = y + 1; waterY < worldDepth - 10; waterY++)
                            chunk.AddBlock(new Vector3(x, waterY, z), BlockType.Water, true);
                    }

                    for (; y >= 0; y--)
                    {
                        var block = GetBlockAtHeight(y, depth);
                        chunk.AddBlock(new Vector3(x, y, z), block, true);

                        var chance = random.NextDouble();

                        if (block == BlockType.GrassBlock)
                        {
                            if (chance <= 0.016)
                            {
                                chance = random.NextDouble();

                                chunk.AddBlock(new Vector3(x, y, z), BlockType.Dirt, true);

                                if (chance <= 0.1)
                                    world.AddEntity(new Vector3(x + offset.X, y + 1, z + offset.Y), EntityType.BirchTree, chunk);
                                else
                                    world.AddEntity(new Vector3(x + offset.X, y + 1, z + offset.Y), EntityType.OakTree, chunk);
                            }
                            else if (chance > 0.1 && chance <= 0.3)
                            {
                                chunk.AddBlock(new Vector3(x, y + 1, z), BlockType.Grass, false);
                            }
                            else if (chance > 0.3 && chance <= 0.35)
                            {
                                chunk.AddBlock(new Vector3(x, y + 1, z), BlockType.SparseGrass, false);
                            }
                            else if (chance > 0.35 && chance <= 0.36)
                            {
                                chunk.AddBlock(new Vector3(x, y + 1, z), BlockType.Poppy, false);
                            }
                            else if (chance > 0.36 && chance <= 0.365)
                            {
                                chunk.AddBlock(new Vector3(x, y + 1, z), BlockType.Allium, false);
                            }
                            else if (chance > 0.365 && chance <= 0.366)
                            {
                                chunk.AddBlock(new Vector3(x, y + 1, z), BlockType.BlueOrchid, false);
                            }
                            else if (chance > 0.366 && chance <= 0.3665)
                            {
                                chunk.AddBlock(new Vector3(x, y + 1, z), BlockType.AzureBluet, false);
                            }
                            else if (chance > 0.3665 && chance <= 0.37)
                            {
                                chunk.AddBlock(new Vector3(x, y + 1, z), BlockType.RedMushroom, false);
                            }
                        }
                        else if (block == BlockType.Sand)
                        {
                            if (chance <= 0.005)
                                chunk.AddBlock(new Vector3(x, y + 1, z), BlockType.DeadBush, false);
                        }

                        depth++;
                    }
                }
            }
        }
        public static int GenerateSeed(string seed)
        {
            if (seed == "")
                return random.Next();
            else if (int.TryParse(seed, out int s))
                return s;

            int finalSeed = 0;

            for (int i = 0; i < seed.Length; i++)
                finalSeed += (i + 1) * seed[i]; 

            return finalSeed;
        }
        public Vector3 GetSpawnPosition(int range)
        {
            while (true)
            {
                int x = random.Next(range);
                int z = random.Next(range);

                if (world.Chunks.ContainsKey(new Vector2(x, z)))
                {
                    int topBlockY = world.GetChunk(new Vector3(x, 0, z), out Vector2 chunkPos).TopBlockPositions[x % Chunk.Size, z % Chunk.Size];
                    var topBlock = world.GetBlock(new Vector3(x, topBlockY, z));

                    while (BlockData.IsBolckTransparent(topBlock))
                        topBlock = world.GetBlock(new Vector3(x, topBlockY--, z));

                    if (topBlock == BlockType.GrassBlock || topBlock == BlockType.Sand && world.GetBlock(new Vector3(x, topBlockY + 1, z)) != BlockType.Water)
                        return new Vector3(x, topBlockY + 2, z);
                    else if (BlockData.IsVegetationBlock(topBlock) || topBlock == BlockType.Water)
                        return new Vector3(x, topBlockY, z);
                }
            }
        }
    }
}