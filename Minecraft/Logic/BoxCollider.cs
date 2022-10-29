using Minecraft.Terrain;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Documents;

namespace Minecraft.Logic
{
    internal class BoxCollider : IBoxCollider
    {
        public Vector3 Position { get; private set; }
        public float Width { get; }
        public float Height { get; }

        private IWorld worldToCollide;

        public BoxCollider(Vector3 position, float width, float height, IWorld worldToCollide)
        {
            Width = width;
            Height = height;

            UpdatePosition(position);
            this.worldToCollide = worldToCollide;
        }
        public void Collision(ref Vector3 deltaPos, out bool headHit, out bool groundHit)
        {
            bool xBlocked = false, yBlocked = false, zBlocked = false;

            headHit = false;
            groundHit = false;

            BroadPhase(ref deltaPos, ref xBlocked, ref yBlocked, ref zBlocked, ref headHit, ref groundHit);
            BroadPhase(ref deltaPos, ref xBlocked, ref yBlocked, ref zBlocked, ref headHit, ref groundHit);
            BroadPhase(ref deltaPos, ref xBlocked, ref yBlocked, ref zBlocked, ref headHit, ref groundHit);
        }
        public void UpdatePosition(Vector3 pos)
        {
            Position = pos - new Vector3(Width / 2, 0.5f, Width / 2);
        }
        private bool BroadPhase(ref Vector3 deltaPos, ref bool xBlocked, ref bool yBlocked, ref bool zBlocked, ref bool headHit, ref bool groundHit)
        {
            int broadPhaseLeft;
            int broadPhaseRight;

            int broadPhaseBottom;
            int broadPhaseTop;

            int broadPhaseBack;
            int broadPhaseFront;

            if (deltaPos.X < 0.0)
            {
                broadPhaseLeft = (int)Math.Floor(Position.X + deltaPos.X - Width);
                broadPhaseRight = (int)Math.Ceiling(Position.X);
            }
            else
            {
                if (deltaPos.X == 0)
                {
                    broadPhaseLeft = (int)Math.Floor(Position.X);
                    broadPhaseRight = (int)Math.Floor(Position.X + Width);
                }
                else
                {
                    broadPhaseLeft = (int)Math.Floor(Position.X);
                    broadPhaseRight = (int)Math.Ceiling(Position.X + deltaPos.X + Width);
                }
            }

            if (deltaPos.Y < 0.0)
            {
                broadPhaseBottom = (int)Math.Floor(Position.Y + deltaPos.Y - Height);
                broadPhaseTop = (int)Math.Floor(Position.Y + Height);
            }
            else
            {
                if (deltaPos.Y == 0)
                {
                    broadPhaseBottom = (int)Math.Floor(Position.Y);
                    broadPhaseTop = (int)Math.Floor(Position.Y + Height);
                }
                else
                {
                    broadPhaseBottom = (int)Math.Floor(Position.Y);
                    broadPhaseTop = (int)Math.Ceiling(Position.Y + deltaPos.Y + Height);
                }

            }

            if (deltaPos.Z < 0.0)
            {
                broadPhaseBack = (int)Math.Floor(Position.Z + deltaPos.Z - Width);
                broadPhaseFront = (int)Math.Floor(Position.Z);
            }
            else
            {
                if (deltaPos.Z == 0)
                {
                    broadPhaseBack = (int)Math.Floor(Position.Z);
                    broadPhaseFront = (int)Math.Floor(Position.Z + Width);
                }
                else
                {
                    broadPhaseBack = (int)Math.Floor(Position.Z);
                    broadPhaseFront = (int)Math.Floor(Position.Z + deltaPos.Z + Width);
                }
            }

            float minCollisionTime = float.MaxValue;

            Vector3 collisionNormal = new Vector3();

            //Debug.WriteLine(deltaPos);
            //Debug.WriteLine($"Left:{broadPhaseLeft} Right:{broadPhaseRight} Bot:{broadPhaseBottom} Top:{broadPhaseTop} Back:{broadPhaseBack} Front:{broadPhaseFront}");

            for (int x = broadPhaseLeft; x <= broadPhaseRight; x++)
                for (int y = broadPhaseBottom; y <= broadPhaseTop; y++)
                    for (int z = broadPhaseBack; z <= broadPhaseFront; z++)
                    {
                        var blockPos = new Vector3(x, y, z);

                        if (BlockData.IsBlockSolid(worldToCollide.GetBlock(blockPos)))
                        {
                            if (SweptAABB(deltaPos, blockPos, out Vector3 _collisionNormal, out float collisionTime))
                            {
                                if (collisionTime < minCollisionTime)
                                {
                                    minCollisionTime = collisionTime;
                                    collisionNormal = _collisionNormal;
                                }
                            }
                        }
                    }

            headHit = collisionNormal.Y == 1 || headHit;
            groundHit = collisionNormal.Y == -1 || groundHit;

            if (minCollisionTime < 1f)
            {
                //Debug.WriteLine(collisionNormal);

                float remainingTime = 1 - minCollisionTime;

                float dot = Vector3.Dot(deltaPos, collisionNormal) * remainingTime;

                if (!xBlocked)
                    deltaPos.X -= collisionNormal.X * dot;

                if (!yBlocked)
                    deltaPos.Y -= collisionNormal.Y * dot;

                if (!zBlocked)
                    deltaPos.Z -= collisionNormal.Z * dot;

                if (collisionNormal.X != 0)
                    xBlocked = true;

                if (collisionNormal.Y != 0)
                    yBlocked = true;

                if (collisionNormal.Z != 0)
                    zBlocked = true;

                return true;
            }
            return false;
        }
        private bool SweptAABB(in Vector3 deltaPos, in Vector3 blockPos, out Vector3 collisionNormal, out float collisionTime)
        {

            float xInvEntry, yInvEntry, zInvEntry;
            float xInvExit, yInvExit, zInvExit;

            if (deltaPos.X > 0f)
            {
                xInvEntry = blockPos.X - (Position.X + Width);
                xInvExit = (blockPos.X + 1) - Position.X;
            }
            else
            {
                xInvEntry = (blockPos.X + 1) - Position.X;
                xInvExit = blockPos.X - (Position.X + Width);
            }

            if (deltaPos.Y > 0f)
            {
                yInvEntry = blockPos.Y - (Position.Y + Height);
                yInvExit = (blockPos.Y + 1) - Position.Y;
            }
            else
            {
                yInvEntry = (blockPos.Y + 1) - Position.Y;
                yInvExit = blockPos.Y - (Position.Y + Height);
            }

            if (deltaPos.Z > 0f)
            {
                zInvEntry = blockPos.Z - (Position.Z + Width);
                zInvExit = (blockPos.Z + 1) - Position.Z;
            }
            else
            {
                zInvEntry = (blockPos.Z + 1) - Position.Z;
                zInvExit = blockPos.Z - (Position.Z + Width);
            }

            float xEntry, yEntry, zEntry;
            float xExit, yExit, zExit;

            if (deltaPos.X == 0f)
            {
                xEntry = float.NegativeInfinity;
                xExit = float.PositiveInfinity;
            }
            else
            {
                xEntry = xInvEntry / deltaPos.X;
                xExit = xInvExit / deltaPos.X;
            }

            if (deltaPos.Y == 0f)
            {
                yEntry = float.NegativeInfinity;
                yExit = float.PositiveInfinity;
            }
            else
            {
                yEntry = yInvEntry / deltaPos.Y;
                yExit = yInvExit / deltaPos.Y;
            }

            if (deltaPos.Z == 0f)
            {
                zEntry = float.NegativeInfinity;
                zExit = float.PositiveInfinity;
            }
            else
            {
                zEntry = zInvEntry / deltaPos.Z;
                zExit = zInvExit / deltaPos.Z;
            }

            collisionTime = Math.Max(xEntry, Math.Max(yEntry, zEntry));
            float exitTime = Math.Min(xExit, Math.Min(yExit, zExit));

            if (collisionTime > exitTime || (xEntry < 0.0f && yEntry < 0.0f && zEntry < 0.0f) || xEntry > 1.0f || yEntry > 1.0f || zEntry > 1.0f)
            {
                collisionNormal = Vector3.Zero;
                return false;
            }
            else
            {
                if (xEntry > yEntry && xEntry > zEntry || (xEntry == yEntry && deltaPos.X != 0 && deltaPos.Y != 0))
                    collisionNormal = new Vector3(Math.Sign(deltaPos.X), 0, 0);
                else if (yEntry > zEntry && yEntry > xEntry)
                    collisionNormal = new Vector3(0, Math.Sign(deltaPos.Y), 0);
                else
                    collisionNormal = new Vector3(0, 0, Math.Sign(deltaPos.Z));


                return true;
            }
        }
    }
}
