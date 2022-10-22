using Minecraft.Controller;
using Minecraft.Game;
using Minecraft.Graphics;
using Minecraft.Terrain;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using static System.Windows.Forms.DataFormats;

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
        public static bool CollisionEnabled = true;

        private World world;
        private Player player;

        private const float moveSpeed = 5.0f;
        private const float sprintSpeed = 50.0f;
        private const float mouseSpeed = 0.125f;

        private bool jumping = false;
        private bool falling = false;

        private IBoxCollider collider;
        private IForce force;
        public PlayerLogic(Player player, World world)
        {
            this.world = world;
            this.player = player;

            force = new Force();
            force.SetForceType(ForceType.Rise);

            collider = new BoxCollider(player.Position, 1, 1, world);
        }
        public void Jump()
        {
            if (!jumping)
            {
                jumping = true;
                force.SetForceType(ForceType.Rise);
            }
        }
        public void Update(float delta)
        {
            //force.Apply(out Vector3 deltaPos, ref jumping);
            //deltaPos *= delta;
            //collider.Collision(ref deltaPos);

            //player.Camera.ModPosition(deltaPos);
            //collider.Position = player.Position - new Vector3(0.5f);
        }
        public void Move(Direction dir, float delta)
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

            if(CollisionEnabled)
                collider.Collision(ref deltaPos);

            player.Camera.ModPosition(deltaPos);
            collider.Position = player.Position - new Vector3(0.5f);

            //Debug.WriteLine(collider.Position);
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
            front.Y = (float)Math.Sin(player.Camera.Pitch);
            front.Z = (float)(Math.Cos(player.Camera.Pitch) * Math.Sin(player.Camera.Yaw));

            player.Camera.Front = Vector3.Normalize(front);
        }
    }
}
