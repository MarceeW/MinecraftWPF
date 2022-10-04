using Minecraft.Controller;
using Minecraft.Game;
using Minecraft.Graphics;
using Minecraft.Terrain;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Minecraft.Logic
{
    internal enum Direction
    {
        Front,
        Back,
        Up,
        Down,
        Left,
        Right
    }
    internal class PlayerLogic
    {
        public bool Sprint = false;     

        private World world;
        private Player player;

        private const float moveSpeed = 10.0f;
        private const float sprintSpeed = 50.0f;
        private const float mouseSpeed = 0.125f;
        private const float playerHeight = 2.0f;
        private const float colliderBoxSize = 0.5f;

        private bool jumping = false;
        private Force force;
        public PlayerLogic(Player player,World world)
        {
            this.world = world;
            this.player = player;

            force = new Force();
            force.SetForceType(ForceType.Rise);
        }
        public void Jump()
        {
            if (!jumping)
            {
                jumping = true;
                force.SetForceType(ForceType.Rise);
            }
        }
        public void Update(float updateDelta)
        {
            
        }
        public void Move(Direction dir,float delta)
        {
            float speed = Sprint ? sprintSpeed : moveSpeed;

            Vector3 deltaPos = new Vector3();

            switch (dir)
            {
                case Direction.Front:
                    deltaPos = player.Camera.Front * speed * delta;
                    break;
                case Direction.Back:
                    deltaPos = -player.Camera.Front * speed * delta;
                    break;
                case Direction.Up:
                    deltaPos = player.Camera.Up * speed * delta;
                    break;
                case Direction.Down:
                    deltaPos = -player.Camera.Up * speed * delta;
                    break;
                case Direction.Left:
                    deltaPos = -Vector3.Normalize(Vector3.Cross(player.Camera.Front, player.Camera.Up)) * speed * delta;
                    break;
                case Direction.Right:
                    deltaPos = Vector3.Normalize(Vector3.Cross(player.Camera.Front, player.Camera.Up)) * speed * delta;
                    break;
            }
            Collision(ref deltaPos);

            player.Camera.ModPosition(deltaPos);
        }
        public void ChangeView()
        {
            //needs to move out from here
            player.Camera.ChangePitch(-MathHelper.DegreesToRadians(MouseController.DeltaY) * mouseSpeed);
            player.Camera.ChangeYaw(MathHelper.DegreesToRadians(MouseController.DeltaX) * mouseSpeed);

            if (player.Camera.Pitch > MathHelper.DegreesToRadians(89.0))
                player.Camera.ResetPitch((float)MathHelper.DegreesToRadians(89.0));
            else if (player.Camera.Pitch < MathHelper.DegreesToRadians(-89.0))
                player.Camera.ResetPitch((float)MathHelper.DegreesToRadians(-89.0));

            Vector3 front = new Vector3();
            front.X = (float)(Math.Cos(player.Camera.Pitch) * Math.Cos(player.Camera.Yaw));
            front.Y = (float) Math.Sin(player.Camera.Pitch);
            front.Z = (float)(Math.Cos(player.Camera.Pitch) * Math.Sin(player.Camera.Yaw));

            player.Camera.Front = Vector3.Normalize(front);
        }
        private bool Collision(ref Vector3 deltaPos)
        {
            int broadPhaseLeft;
            int broadPhaseRight;

            int broadPhaseTop;
            int broadPhaseBottom;

            if (deltaPos.X < 0.0)
            {
                broadPhaseLeft = (int)(player.Position.X + deltaPos.X) - 1;
                broadPhaseRight = (int)player.Position.X + 1;
            }
            else
            {
                broadPhaseLeft = (int)player.Position.X;
                broadPhaseRight = (int)(player.Position.X + deltaPos.X) + 1;
            }

            if (deltaPos.Z < 0.0)
            {
                broadPhaseTop = (int)(player.Position.X + deltaPos.X) - 1;
                broadPhaseBottom = (int)player.Position.X + 1;
            }
            else
            {
                broadPhaseTop = (int)player.Position.X;
                broadPhaseBottom = (int)(player.Position.X + deltaPos.X) + 1;
            }

            Debug.WriteLine($"Left: {broadPhaseLeft} Right: {broadPhaseRight} Bot: {broadPhaseBottom} Top: {broadPhaseTop}");

            for (int x = broadPhaseLeft; x <= broadPhaseRight; x++)
            {
                for (int z = broadPhaseTop; z <= broadPhaseBottom; z++)
                {
                    if(world.GetBlock(new Vector3(x,(int)player.Position.Y,z)) > 0)
                    {
                        if(SweptAABB(deltaPos, new Vector2(x, z), out Vector2? normal, out float collisionTime))
                        {
                            deltaPos.X *= collisionTime;
                            deltaPos.Z *= collisionTime;
                            return true;
                        }      
                    }
                }
            }

            return false;
        }
        private bool SweptAABB(in Vector3 deltaPos, in Vector2 cell, out Vector2? normal, out float collisionTime)
        {
            float xInvEntry, zInvEntry;
            float xInvExit, zInvExit;

            if(deltaPos.X > 0.0f)
            {
                xInvEntry = cell.X - (player.Position.X + colliderBoxSize);
                xInvExit = (cell.X + 1) - player.Position.X;
            }
            else
            {
                xInvEntry = (cell.X + 1) - player.Position.X;
                xInvExit = cell.X - (player.Position.X + colliderBoxSize);
            }

            if (deltaPos.Z > 0.0f)
            {
                zInvEntry = cell.Y - (player.Position.Z + colliderBoxSize);
                zInvExit = (cell.Y + 1) - player.Position.Z;
            }
            else
            {
                zInvEntry = (cell.Y + 1) - player.Position.Z;
                zInvExit = cell.Y - (player.Position.Z + colliderBoxSize);
            }

            float xEntry, zEntry;
            float xExit, zExit;

            if(deltaPos.X == 0.0f)
            {
                xEntry = float.NegativeInfinity;
                xExit = float.PositiveInfinity;
            }
            else
            {
                xEntry = xInvEntry / deltaPos.X;
                xExit = xInvExit / deltaPos.X;
            }

            if (deltaPos.Z == 0.0f)
            {
                zEntry = float.NegativeInfinity;
                zExit = float.PositiveInfinity;
            }
            else
            {
                zEntry = zInvEntry / deltaPos.Z;
                zExit = zInvExit / deltaPos.Z;
            }

            collisionTime = Math.Max(xEntry, zEntry);
            float exitTime = Math.Min(xExit, zExit);

            if (collisionTime > exitTime || xEntry < 0.0f && zEntry < 0.0f || xEntry > 1.0f || zEntry > 1.0f)
            {
                normal = null;
                return false;
            }
            else
            {
                if(xEntry > zEntry)
                {
                    if(xInvEntry < 0.0f)
                        normal = new Vector2(1.0f, 0.0f);
                    else
                        normal = new Vector2(-1.0f, 0.0f);
                }
                else
                {
                    if (zInvEntry < 0.0f)
                        normal = new Vector2(0.0f, 1.0f);
                    else
                        normal = new Vector2(0.0f, -1.0f);
                }
                return true;
            }
        }
    }
}
