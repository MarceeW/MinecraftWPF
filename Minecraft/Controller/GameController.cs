using Minecraft.Game;
using Minecraft.Graphics;
using Minecraft.Render;
using Minecraft.Terrain;
using Minecraft.UI;
using OpenTK.Mathematics;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Minecraft.Controller
{
    internal class GameController
    {
        public Player Player { get; private set; }
        public World World { get; private set; }
        public WorldRenderer WorldRendererer { get; }
        public RenderWindow RenderWindow { get; }
        public bool IsGameRunning { get; private set; }

        private PlayerController playerController;
        private WorldGenerator worldGenerator;

        private Thread updateThread;
        private Stopwatch gameStopwatch;

        MouseListener mouseListener;
        public GameController(Renderer renderer,RenderWindow renderWindow)
        {

            RenderWindow = renderWindow;

            World = new World();
            WorldSerializer.World = World;

            Player = new Player(new Vector3(0, 40, 0));
            
            worldGenerator = new WorldGenerator(World);
            WorldRendererer = new WorldRenderer(World,Player.Camera);
            worldGenerator.ChunkAdded += WorldRendererer.AddToQueue;

            mouseListener = new MouseListener(RenderWindow);

            if (!WorldSerializer.WorldFileExists())
                worldGenerator.InitWorld();

            playerController = new PlayerController(Player, World);
            playerController.ChangedChunk += worldGenerator.ExpandWorld;

            renderer.OnRendering += WorldRendererer.CreateMeshesInQueue;
            renderer.OnRendering += worldGenerator.AddGeneratedChunksToWorld;

            var characterHand = new CharacterHand(Player.Hotbar);
            mouseListener.LeftMouseClick += characterHand.OnHit;

            renderer.Scene = new Scene(Player.Camera, World, WorldRendererer, characterHand);

            playerController.InitPlayerCamera();

            RenderWindow.Hotbar = Player.Hotbar;
            RenderWindow.RenderSizeChange += renderer.Scene.OnProjectionMatrixChange;
            RenderWindow.Loaded += (object sender, RoutedEventArgs e) => renderer.SetupRenderer((int)renderWindow.Width, (int)renderWindow.Height);

            //renderWindow.Loaded += (object sender, RoutedEventArgs e) =>
            //{
            //    World.Chunks = WorldSerializer.LoadWorld();
            //
            //    foreach (var chunk in World.Chunks)
            //    {
            //        chunk.Value.Mesh = new ChunkMesh();
            //        worldRendererer.AddToQueue(chunk.Key);
            //    }
            //};
            //renderWindow.Closing += (object? sender,CancelEventArgs e) => WorldSerializer.SaveWorld();


            updateThread = new Thread(UpdateGameState);
            updateThread.SetApartmentState(ApartmentState.STA);
            gameStopwatch = new Stopwatch();

            MouseController.HideMouse();

            mouseListener.RightMouseClick += () =>
            {
                var blockHit = Ray.Cast(Player.Camera, World, out bool hit, out FaceDirection hitFace);

                if (hit && !renderWindow.IsInventoryOpened)
                {
                    switch (hitFace)
                    {
                        case FaceDirection.Top:
                            blockHit.Y++;
                            break;
                        case FaceDirection.Bot:
                            blockHit.Y--;
                            break;
                        case FaceDirection.Right:
                            blockHit.X++;
                            break;
                        case FaceDirection.Left:
                            blockHit.X--;
                            break;
                        case FaceDirection.Front:
                            blockHit.Z++;
                            break;
                        case FaceDirection.Back:
                            blockHit.Z--;
                            break;
                        default:
                            break;
                    }

                    World.AddBlock(blockHit, Player.Hotbar.GetSelectedBlock());

                    characterHand.OnBlockPlace();
                }
            };

            mouseListener.LeftMouseClick += () =>
            {
                var blockHit = Ray.Cast(Player.Camera, World, out bool hit, out FaceDirection hitFace);

                if (hit && !renderWindow.IsInventoryOpened)
                {
                    Debug.WriteLine(blockHit);
                    World.RemoveBlock(blockHit);
                }
            };

            RenderWindow.MouseDown += mouseListener.OnMouseDown;

            RenderWindow.Loaded += (object sender, RoutedEventArgs e) =>
            { 
                updateThread.Start();
                IsGameRunning = true;
            }; 
        }
        private void UpdateGameState()
        {
            const int MAX_TPS = 120;
            const int MIN_TPS = 30;
            double TickRate;

            double updatePerSec = MAX_TPS;
            double updateStep = 1000 / updatePerSec;
            double accumulator = 0;

            gameStopwatch.Start();

            while (IsGameRunning)
            {
                gameStopwatch.Stop();
                double dt = gameStopwatch.Elapsed.TotalMilliseconds;
                gameStopwatch.Restart();

                if (dt > 500)
                {
                    dt = updatePerSec;
                }

                accumulator += dt;

                if (accumulator > updateStep * 2)
                {
                    updatePerSec *= 0.75;
                    if (updatePerSec < MIN_TPS) updatePerSec = MIN_TPS;
                    updateStep = 1000 / updatePerSec;
                    TickRate = updatePerSec;
                }
                else
                {
                    updatePerSec *= 1.1;
                    if (updatePerSec > MAX_TPS) updatePerSec = MAX_TPS;
                    updateStep = 1000 / updatePerSec;
                    TickRate = updatePerSec;
                }

                while (accumulator > updateStep)
                {
                    RenderWindow.Controller.ResetMousePosition();

                    worldGenerator.GenerateChunksToQueue();
                    playerController.Update((float)updateStep / 1000.0f);
                    accumulator -= updateStep;
                }

                Thread.Sleep(0);
            }
        }
    }
}
