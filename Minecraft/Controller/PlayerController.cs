﻿using Minecraft.Game;
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
        class DoubleKeyPressChecker
        {
            public Key KeyToListen { get; private set; }
            public event Action? OnDoublePress;
            private Stopwatch stopwatch;
            private long lastPress = 0;
            private const int maxDoubleClickLatencyMs = 350;
            private const int minDoubleClickLatencyMs = 1;

            private bool valid = true;

            public DoubleKeyPressChecker(Key key)
            {
                KeyToListen = key;
                stopwatch = new Stopwatch();

                stopwatch.Start();
            }
            public void Check(Key key)
            {
                if(key == KeyToListen)
                {
                    
                    long delta = stopwatch.ElapsedMilliseconds - lastPress;
                    

                    if (valid && delta < maxDoubleClickLatencyMs && delta >= minDoubleClickLatencyMs)
                    {
                        OnDoublePress?.Invoke();
                        stopwatch.Restart();
                    }

                    valid = false;
                    lastPress = stopwatch.ElapsedMilliseconds;
                }                
            }
            public void Validate(Key key)
            {
                if(key == KeyToListen)
                {
                    valid = true;
                }
            }
        }

        public event ChunkGeneratorHandler? ChangedChunk;
        public event Action<Vector2>? Moved;

        public static bool CanMove = true;

        private Player player;
        private PlayerLogic playerLogic;
        private const float mouseSpeed = 0.125f;
        private DoubleKeyPressChecker jumpListener;
        public PlayerController(Player player,World world)
        {
            this.player = player;
            playerLogic = new PlayerLogic(player,world);
            jumpListener = new DoubleKeyPressChecker(Key.Space);
            jumpListener.OnDoublePress += () => player.IsFlying = !player.IsFlying;
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
                {
                    if(player.IsFlying)
                        playerLogic.Move(Direction.Up, delta);
                    else
                        playerLogic.Jump();
                }
                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    if (player.IsFlying)
                        playerLogic.Move(Direction.Down, delta);
                    else
                        playerLogic.Crouch = true;
                }
                else
                    playerLogic.Crouch = false;

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
                player.Camera.ChangeView(MouseController.DeltaX, MouseController.DeltaY, mouseSpeed);
            } 
        }
        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            jumpListener.Check(e.Key);
        }
        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            jumpListener.Validate(e.Key);
        }
    }
}
