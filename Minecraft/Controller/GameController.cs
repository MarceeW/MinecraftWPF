using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Game;
using Minecraft.Graphics;
using Minecraft.Misc;
using Minecraft.Render;
using Minecraft.Terrain;
using Minecraft.UI.Logic;
using System;
using System.Diagnostics;
using System.Threading;

namespace Minecraft.Controller
{
    internal class GameController : IDisposable
    {
        public GameSession Session { get; set; }
        public WorldRenderer WorldRendererer { get; private set; }
        public bool IsGameRunning { get; private set; }
        public PlayerController PlayerController { get; private set; }

        private IWorldGenerator worldGenerator;
        private IUILogic uiLogic;
        private GameWindow gameWindow;

        private Thread updateThread;
        private Stopwatch gameStopwatch;
        public GameController(int renderDistance, Renderer renderer, GameWindow gameWindow, IUILogic uiLogic, GameSession session)
        {
            Session = session;
            WorldRendererer = new WorldRenderer();
            WorldRendererer.SetWorld(Session.World);
            this.gameWindow = gameWindow;
            this.uiLogic = uiLogic;

            WorldSerializer.World = Session.World;

            worldGenerator = new WorldGenerator(Session.World, renderDistance);
            worldGenerator.ChunkAdded += WorldRendererer.AddToQueue;
            worldGenerator.WorldInitalized += () => session.Player.Position = worldGenerator.GetSpawnPosition(renderDistance);
            WorldRendererer.RenderDistanceChanged += (int rd) => worldGenerator.RenderDistance = rd;

            PlayerController = new PlayerController(Session.Player, Session.World);
            PlayerController.ChangedChunk += worldGenerator.ExpandWorld;

            renderer.OnRendering += worldGenerator.AddGeneratedChunksToWorld;

            var characterHand = new CharacterHand();
            gameWindow.MouseListener.LeftMouseClick += characterHand.OnHit;
            renderer.Scene = new Scene(WorldRendererer, characterHand);

            Session.Player.Camera.Init(Session.Player.Position);



            gameWindow.RenderSizeChange += renderer.Scene.OnProjectionMatrixChange;
            renderer.SetupRenderer((int)gameWindow.Width, (int)gameWindow.Height);
            gameWindow.PreviewKeyDown += PlayerController.OnKeyDown;
            gameWindow.PreviewKeyUp += PlayerController.OnKeyUp;

            updateThread = new Thread(UpdateGameState);
            updateThread.SetApartmentState(ApartmentState.STA);
            gameStopwatch = new Stopwatch();

            gameWindow.MouseListener.RightMouseClick += () =>
            {
                if (!UILogic.IsInMainMenu)
                {
                    var blockHit = Ray.Cast(Session.World, out bool hit, out FaceDirection hitFace, out double rayDistance);

                    if (rayDistance > 3 && hit && !uiLogic.IsGamePaused)
                    {
                        if (Session.World.GetBlock(blockHit) != BlockType.Grass && Session.World.GetBlock(blockHit) != BlockType.SparseGrass)
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
                }
            };

            gameWindow.MouseListener.LeftMouseClick += () =>
            {
                if (!UILogic.IsInMainMenu)
                {
                    var blockHit = Ray.Cast(Session.World, out bool hit, out FaceDirection hitFace, out double rayDistance);

                    if (hit && !uiLogic.IsInventoryOpened)
                    {
                        Debug.WriteLine(blockHit);
                        Session.World.RemoveBlock(blockHit);
                    }
                }
            };

            updateThread.Start();
            IsGameRunning = true;
        }
        public void InitUserSettings(UserSettings settings)
        {
            PlayerController.MouseSpeed = settings.MouseSpeed;
            WorldRendererer.RenderDistance = settings.RenderDistance;
            Ioc.Default.GetService<ICamera>().Fov = settings.Fov;
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
                    dt = updatePerSec;

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
                    uiLogic.ResetMousePosition();
                    worldGenerator.GenerateChunksToQueue();
                    PlayerController.Update((float)updateStep / 1000.0f);
                    accumulator -= updateStep;
                }

                Thread.Sleep(0);
            }
        }

        public void Dispose()
        {
            IsGameRunning = false;
            Session.Save();
            Session = null;
            WorldRendererer = null;
            PlayerController = null;
            gameWindow.MouseListener.Reset();

            GC.Collect();
        }
    }
}
