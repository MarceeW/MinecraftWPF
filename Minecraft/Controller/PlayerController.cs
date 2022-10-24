using Minecraft.Game;
using Minecraft.Logic;
using Minecraft.Terrain;
using OpenTK.Mathematics;
using System;
using System.Windows.Input;
using System.Diagnostics;

namespace Minecraft.Controller
{
    delegate void ChunkGeneratorHandler(Direction dir,Vector2 position);
    internal class PlayerController
    {
        public event ChunkGeneratorHandler? ChangedChunk;
        public event Action<Vector2>? Moved;

        public static bool CanMove = true;

        private Player player;
        private PlayerLogic playerLogic;
        public PlayerController(Player player,World world)
        {
            this.player = player;
            playerLogic = new PlayerLogic(player,world);

            player.Camera.Fov = 85.0f;

            player.Camera.Front = Vector3.UnitX;
        }
        public void InitPlayerCamera()
        {
            player.Camera.Init();
        }
        public void Update(float delta)
        {
            if (CanMove)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    playerLogic.Sprint = true;
                else
                    playerLogic.Sprint = false;

                Vector2 lastPlayerPos = player.GetPosition().Xz;

                int lastChunkX = (int)(lastPlayerPos.X / Chunk.Size);
                int lastChunkZ = (int)(lastPlayerPos.Y / Chunk.Size);

                if (Keyboard.IsKeyDown(Key.W))
                    playerLogic.Move(Direction.Front, delta);
                if (Keyboard.IsKeyDown(Key.D))
                    playerLogic.Move(Direction.Right, delta);
                if (Keyboard.IsKeyDown(Key.S))
                    playerLogic.Move(Direction.Back, delta);
                if (Keyboard.IsKeyDown(Key.A))
                    playerLogic.Move(Direction.Left, delta);
                if (Keyboard.IsKeyDown(Key.Space))
                    //playerLogic.Jump();
                    playerLogic.Move(Direction.Up, delta);
                if (Keyboard.IsKeyDown(Key.LeftShift))
                    playerLogic.Move(Direction.Down, delta);

                Vector2 playerPos = player.GetPosition().Xz;

                int currChunkX = (int)(playerPos.X / Chunk.Size);
                int currChunkZ = (int)(playerPos.Y / Chunk.Size);

                if (playerPos != lastPlayerPos)
                {
                    if (currChunkX != lastChunkX || currChunkZ != lastChunkZ)
                    {
                        Debug.WriteLine("Chunk: " + new Vector2(currChunkX, currChunkZ));
                        int deltaX = currChunkX - lastChunkX;
                        int deltaZ = currChunkZ - lastChunkZ;

                        Direction dir;

                        if (deltaX < 0)
                            dir = Direction.Left;
                        else if (deltaX > 0)
                            dir = Direction.Right;
                        else if (deltaZ < 0)
                            dir = Direction.Down;
                        else
                            dir = Direction.Up;

                        ChangedChunk?.Invoke(dir, playerPos / Chunk.Size);
                    }
                    Moved?.Invoke(playerPos);
                }

                playerLogic.Update(delta);
                playerLogic.ChangeView();
            } 
        }
    }
}
