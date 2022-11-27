using Minecraft.Game;
using Minecraft.Terrain;
using OpenTK.Mathematics;
using System;

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
        public bool CollisionEnabled = true;
        public event Action<float>? Walking;

        private IWorld world;
        private IPlayer player;

        private const float moveSpeed = 5.0f;
        private const float sprintSpeed = 1.5f;
        private const float crouchSpeed = .5f;

        private bool jumping = false;
        private bool grounded = false;

        private IBoxCollider collider;
        private IForce gravity;

        public bool Crouch { get; set; }
        public bool Sprint { get; set; }
        public void Init(IPlayer player, IWorld world)
        {
            this.world = world;
            this.player = player;

            gravity = new Force();
            gravity.SetForceType(ForceType.Rise);

            collider = new BoxCollider(player.Position, .5f, 1.75f, world);
        }
        public void Jump()
        {
            if (!jumping)
            {
                jumping = true;
                gravity.SetForceType(ForceType.Rise);
            }
        }
        public void Update(float delta)
        {
            if (!player.IsFlying)
            {
                gravity.ApplyGravity(out Vector3 deltaPos);
                deltaPos *= delta;

                if (CollisionEnabled)
                {
                    collider.Collision(ref deltaPos, out bool headHit, out bool groundHit);

                    grounded = groundHit;

                    if (headHit && gravity.Type == ForceType.Rise)
                        gravity.SetForceType(ForceType.Fall);
                    else if (groundHit)
                    {
                        jumping = false;
                        gravity.Reset();
                    }
                }

                player.Camera.ModPosition(deltaPos);
                collider.UpdatePosition(player.Position);
            }
        }
        public void Move(Direction dir, float delta)
        {
            float speed = Sprint ? moveSpeed * sprintSpeed : Crouch ? moveSpeed * crouchSpeed : moveSpeed;

            if (player.IsFlying)
                speed *= 2.5f;

            Vector3 deltaPos = new Vector3();

            switch (dir)
            {
                case Direction.Front:
                    if (grounded && !player.IsFlying)
                        Walking?.Invoke(Crouch ? delta * crouchSpeed : Sprint ? delta * sprintSpeed : delta);

                    deltaPos = Vector3.Normalize(new Vector3(player.Camera.Front.X, 0, player.Camera.Front.Z)) * speed * delta;
                    break;
                case Direction.Back:
                    if (grounded && !player.IsFlying)
                        Walking?.Invoke(Crouch ? delta * crouchSpeed : Sprint ? delta * sprintSpeed : delta);

                    deltaPos = -Vector3.Normalize(new Vector3(player.Camera.Front.X, 0, player.Camera.Front.Z)) * speed * delta;
                    break;
                case Direction.Up:
                    deltaPos = player.Camera.Up * speed * delta;
                    break;
                case Direction.Down:
                    deltaPos = -player.Camera.Up * speed * delta;
                    break;
                case Direction.Left:
                    deltaPos = -Vector3.Normalize(Vector3.Cross(player.Camera.Front, player.Camera.Up)) * speed * delta;

                    if (!player.IsFlying && grounded)
                        Walking?.Invoke(Crouch ? delta * crouchSpeed : Sprint ? delta * sprintSpeed : delta);
                    break;
                case Direction.Right:
                    deltaPos = Vector3.Normalize(Vector3.Cross(player.Camera.Front, player.Camera.Up)) * speed * delta;

                    if (!player.IsFlying && grounded)
                        Walking?.Invoke(Crouch ? delta * crouchSpeed : Sprint ? delta * sprintSpeed : delta);
                    break;
            }

            if (CollisionEnabled)
            {
                collider.Collision(ref deltaPos, out bool headHit, out bool groundHit);

                if (player.IsFlying && groundHit)
                    player.IsFlying = false;
            }

            player.Camera.ModPosition(deltaPos);
            collider.UpdatePosition(player.Position);
        }
    }
}
