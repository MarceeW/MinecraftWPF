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
    internal class PlayerLogic : IPlayerLogic
    {
        public bool Sprint = false;
        public static bool CollisionEnabled = true;

        private World world;
        private Player player;

        private const float moveSpeed = 5.0f;
        private const float sprintSpeed = 50.0f;

        private bool jumping = false;
        private bool grounded = false;
        private bool falling = false;

        private IBoxCollider collider;
        private IForce force;
        public PlayerLogic(Player player, World world)
        {
            this.world = world;
            this.player = player;

            force = new Force();
            force.SetForceType(ForceType.Rise);

            collider = new BoxCollider(player.Position, 1, 2, world);
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
            if (!player.IsFlying)
            {
                force.Apply(out Vector3 deltaPos);
                deltaPos *= delta;

                collider.Collision(ref deltaPos, out bool headHit, out bool groundHit);

                if (headHit && force.Type == ForceType.Rise)
                    force.SetForceType(ForceType.Fall);
                else if (groundHit)
                    jumping = false;

                player.Camera.ModPosition(deltaPos);
                collider.Position = player.Position - new Vector3(0.5f);
            }
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

            if (CollisionEnabled)
                collider.Collision(ref deltaPos, out bool headHit, out bool groundHit);

            player.Camera.ModPosition(deltaPos);
            collider.Position = player.Position - new Vector3(0.5f);

            //Debug.WriteLine(collider.Position);
        }
    }
}
