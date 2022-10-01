using Minecraft.Graphics.Model;
using Minecraft.Terrain;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Minecraft.Graphics
{
    internal class ChunkMeshGenerator
    {
        //TODO
        public Queue<Vector2> GeneratorQueue { get; private set; }
        public Queue<ChunkMesh> LoadQueue { get; private set; }

        private Thread generatorThread;
        private World world;

        private bool running;
        public ChunkMeshGenerator(World world)
        {
            GeneratorQueue = new Queue<Vector2>();
            this.world = world;
            generatorThread = new Thread(Generator);
        }
        public void Start()
        {
            running = true;
            generatorThread.Start();
        }
        public void Stop()
        {
            running = false;
        }
        private void Generator()
        {
            while (running)
            {

            }
        }
    }
}
