using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Graphics;
using Minecraft.Terrain;
using OpenTK.Mathematics;
using System;

namespace Minecraft.Game
{
    internal static class Ray
    {
        public static int MaxDistance { get; } = 6;
        public static Vector3 Cast(IWorld world, out bool hit,out FaceDirection hitFace)
        {
            var camera = Ioc.Default.GetService<ICamera>();

            double xDeltaDist = Math.Abs(1 / camera.Front.X);
            double yDeltaDist = Math.Abs(1 / camera.Front.Y);
            double zDeltaDist = Math.Abs(1 / camera.Front.Z);

            double xRayDist = 0;
            double yRayDist = 0;
            double zRayDist = 0;

            int mapX = camera.Position.X < 0 ? (int)(camera.Position.X) - 1 : (int)(camera.Position.X);
            int mapY = camera.Position.Y < 0 ? (int)(camera.Position.Y) - 1 : (int)(camera.Position.Y);
            int mapZ = camera.Position.Z < 0 ? (int)(camera.Position.Z) - 1 : (int)(camera.Position.Z);

            int stepX, stepY, stepZ;

            if(camera.Front.X > 0)
            {
                stepX = 1;
                xRayDist = Math.Abs((mapX + 1 - camera.Position.X)) * xDeltaDist;
            }
            else
            {
                stepX = -1;
                xRayDist = Math.Abs((camera.Position.X - mapX)) * xDeltaDist;
            }

            if (camera.Front.Y > 0)
            {
                stepY = 1;
                yRayDist = Math.Abs((mapY + 1 - camera.Position.Y)) * yDeltaDist;
            }
            else
            {
                stepY = -1;
                yRayDist = Math.Abs((camera.Position.Y - mapY)) * yDeltaDist;
            }

            if (camera.Front.Z > 0)
            {
                stepZ = 1;
                zRayDist = Math.Abs((mapZ + 1 - camera.Position.Z)) * zDeltaDist;
            }
            else
            {
                stepZ = -1;
                zRayDist = Math.Abs((camera.Position.Z - mapZ)) * zDeltaDist;
            }

            if (camera.Front.X == 0)
                xRayDist = double.PositiveInfinity;

            if (camera.Front.Y == 0)
                yRayDist = double.PositiveInfinity;

            if (camera.Front.Z == 0)
                zRayDist = double.PositiveInfinity;

            double rayDistance = 0;
            hit = false;
            hitFace = FaceDirection.Bot;

            Vector3 currentBlock = new Vector3(mapX, mapY, mapZ);

            while (!hit && (camera.Position - currentBlock).Length < MaxDistance)
            {
                if(xRayDist < yRayDist && xRayDist < zRayDist)
                {
                    xRayDist += xDeltaDist;
                    rayDistance = xRayDist;
                    mapX += stepX;

                    if (stepX < 0)
                        hitFace = FaceDirection.Right;
                    else
                        hitFace = FaceDirection.Left;
                }
                else if (yRayDist < xRayDist && yRayDist < zRayDist)
                {
                    yRayDist += yDeltaDist;
                    rayDistance = yRayDist;
                    mapY += stepY;

                    if (stepY < 0)
                        hitFace = FaceDirection.Top;
                    else
                        hitFace = FaceDirection.Bot;
                }
                else
                {
                    zRayDist += zDeltaDist;
                    rayDistance = zRayDist;
                    mapZ += stepZ;

                    if (stepZ < 0)
                        hitFace = FaceDirection.Front;
                    else
                        hitFace = FaceDirection.Back;
                }
                currentBlock = new Vector3(mapX, mapY, mapZ);
                var block = world.GetBlock(currentBlock);

                hit = BlockData.IsBlockSolid(block) || BlockData.IsVegetationBlock(block);
            }

            return currentBlock;
        }
    }
}
