using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Game;
using Minecraft.Graphics;
using Minecraft.Render;
using Minecraft.Terrain;
using Minecraft.UI;
using OpenTK.Mathematics;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Minecraft.Controller
{
    internal class GameController
    {
        public GameSession Session { get; set; }
        public WorldRenderer WorldRendererer { get; private set; }
        public bool IsGameRunning { get; private set; }
        public PlayerController PlayerController { get; private set; }

        private IWorldGenerator worldGenerator;
        private GameWindow gameWindow;

        private Thread updateThread;
        private Stopwatch gameStopwatch;
        public GameController(Renderer renderer,GameWindow gameWindow,GameSession session)
        {
            Session = session;
            WorldRendererer = new WorldRenderer();
            WorldRendererer.SetWorld(Session.World);
            this.gameWindow = gameWindow;

            WorldSerializer.World = Session.World;
    
            worldGenerator = new WorldGenerator(Session.World,WorldRendererer.RenderDistance);
            worldGenerator.ChunkAdded += WorldRendererer.AddToQueue;
            WorldRendererer.RenderDistanceChanged += (int rd) => worldGenerator.RenderDistance = rd;

            if (!WorldSerializer.WorldFileExists())
                worldGenerator.InitWorld();

            PlayerController = new PlayerController(Session.Player, Session.World);
            PlayerController.ChangedChunk += worldGenerator.ExpandWorld;

            renderer.OnRendering += WorldRendererer.CreateMeshesInQueue;
            renderer.OnRendering += worldGenerator.AddGeneratedChunksToWorld;

            var characterHand = new CharacterHand();
            gameWindow.MouseListener.LeftMouseClick += characterHand.OnHit;
            renderer.Scene = new Scene(WorldRendererer, characterHand);

            Ioc.Default.GetService<ICamera>()?.Init(Session.Player.Position);

            gameWindow.RenderSizeChange += renderer.Scene.OnProjectionMatrixChange;
            gameWindow.Loaded += (object sender, RoutedEventArgs e) => renderer.SetupRenderer((int)gameWindow.Width, (int)gameWindow.Height);
            gameWindow.PreviewKeyDown += PlayerController.OnKeyDown;
            gameWindow.PreviewKeyUp += PlayerController.OnKeyUp;

            //gameWindow.Loaded += (object sender, RoutedEventArgs e) =>
            //{
            //    if (WorldSerializer.WorldFileExists())
            //    {
            //        World.Chunks = WorldSerializer.LoadWorld();
            //
            //        foreach (var chunk in World.Chunks)
            //        {
            //            chunk.Value.Mesh = new ChunkMesh();
            //            WorldRendererer.AddToQueue(chunk.Key);
            //        }
            //    }
            //};
            //gameWindow.Closing += (object? sender,CancelEventArgs e) => WorldSerializer.SaveWorld();

            updateThread = new Thread(UpdateGameState);
            updateThread.SetApartmentState(ApartmentState.STA);
            gameStopwatch = new Stopwatch();

            gameWindow.MouseListener.RightMouseClick += () =>
            {
                var blockHit = Ray.Cast(Session.World, out bool hit, out FaceDirection hitFace);

                if (hit && !gameWindow.IsGamePaused)
                {
                    if(Session.World.GetBlock(blockHit) != BlockType.Grass && Session.World.GetBlock(blockHit) != BlockType.SparseGrass)
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
                    }

                    Session.World.AddBlock(blockHit, Session.Player.Hotbar.GetSelectedBlock());

                    characterHand.OnBlockPlace();
                }
            };

            gameWindow.MouseListener.LeftMouseClick += () =>
            {
                var blockHit = Ray.Cast(Session.World, out bool hit, out FaceDirection hitFace);

                if (hit && !gameWindow.IsInventoryOpened)
                {
                    Debug.WriteLine(blockHit);
                    Session.World.RemoveBlock(blockHit);
                }
            };

            gameWindow.Loaded += (object sender, RoutedEventArgs e) =>
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
                    gameWindow.ResetMousePosition();
                    worldGenerator.GenerateChunksToQueue();
                    PlayerController.Update((float)updateStep / 1000.0f);
                    accumulator -= updateStep;
                }

                Thread.Sleep(0);
            }
        }
    }
}
