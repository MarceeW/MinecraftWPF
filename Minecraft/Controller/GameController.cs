using Minecraft.Game;
using Minecraft.Graphics;
using Minecraft.Render;
using Minecraft.Terrain;
using OpenTK.Mathematics;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace Minecraft.Controller
{
    internal class GameController
    {
        public Player Player { get; private set; }
        public World World { get; private set; }
        public WorldRenderer worldRendererer { get; }
        public bool IsGameRunning { get; private set; }

        private PlayerController playerController;
        private WorldGenerator worldGenerator;

        private Thread updateThread;
        private Stopwatch gameStopwatch;

        MouseListener mouseListener;
        public GameController(Renderer renderer,RenderWindow renderWindow)
        {

            World = new World();
            Player = new Player(new Vector3(0, 64, 0));
            
            worldGenerator = new WorldGenerator(World);
            worldRendererer = new WorldRenderer(World,Player.Camera);
            worldGenerator.ChunkAdded += worldRendererer.AddToQueue;
            worldGenerator.InitWorld();

            playerController = new PlayerController(Player, World);
            playerController.ChangedChunk += worldGenerator.ExpandWorld;
            playerController.Moved += World.OrderByPlayerPosition;

            renderer.OnRendering += worldRendererer.CreateMeshesInQueue;
            renderer.OnRendering += worldGenerator.AddGeneratedChunksToWorld;
            renderer.Scene = new Scene(Player.Camera, World, worldRendererer);

            playerController.InitPlayerCamera();

            renderWindow.RenderSizeChange += renderer.Scene.OnProjectionMatrixChange;
            renderWindow.Loaded += (object sender, RoutedEventArgs e) => renderer.SetupRenderer((int)renderWindow.Width, (int)renderWindow.Height);

            WindowController.Window = renderWindow;

            updateThread = new Thread(UpdateGameState);
            updateThread.SetApartmentState(ApartmentState.STA);
            gameStopwatch = new Stopwatch();

            MouseController.HideMouse();

            mouseListener = new MouseListener();
            mouseListener.RightMouseClick += () =>
            {
                var blockHit = Ray.Cast(Player.Camera, World, out bool hit, out FaceDirection hitFace);

                if (hit)
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

                    World.AddBlock(blockHit, BlockType.WoodPlank);
                }
            };

            mouseListener.LeftMouseClick += () =>
            {
                var blockHit = Ray.Cast(Player.Camera, World, out bool hit, out FaceDirection hitFace);

                if (hit)
                {
                    World.RemoveBlock(blockHit);
                }
            };

            renderWindow.MouseDown += mouseListener.OnMouseDown;

            renderWindow.Loaded += (object sender, RoutedEventArgs e) =>
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
            double updateStep = 1000 / updatePerSec; // milliseconds
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
                    WindowController.CheckForKeyPress();
                    WindowController.ResetMousePosition();

                    worldGenerator.GenerateChunksToQueue();
                    playerController.Update((float)updateStep / 1000.0f);
                    accumulator -= updateStep;
                }

                Thread.Sleep(0);
            }
        }
    }
}
