using OpenTK.Mathematics;
using OpenTK.Input;
using Minecraft.Controller;
using Minecraft.Game;
using System;

namespace Minecraft.Graphics
{
    internal class Camera : ICamera
    {
        public Vector3 Position { get; private set; }
        public Vector3 Up { get; private set; }
        public Vector3 Front { get; set; }
        public Matrix4 View { get; private set; }
        public float Fov { get; set; }
        public float Pitch { get; private set; }
        public float Yaw { get; private set; }

        public event ShaderMat4Handler? ViewMatrixChange;
        public event ShaderVec3Handler? FrontChange;

        public Camera(Vector3 startPos)
        {
            Position = startPos;
            Up = Vector3.UnitY;
        }
        public void Init()
        {
            UpdateViewMatrix();
        }
        public void SetPosition(Vector3 position)
        {
            Position = position;
        }
        public void ModPosition(Vector3 change)
        {
            Position += change;
        }
        public void ChangeFront(Vector3 front)
        {
            Front = front;
        }
        public void ChangeYaw(float change)
        {
            Yaw += change;
        }
        public void ChangePitch(float change)
        {
            Pitch += change;
        }
        public void ResetPitch(float resetValue)
        {
            Pitch = resetValue;
        }
        public void ChangeView(float deltaX, float deltaY, float mouseSpeed)
        {
            ChangePitch(-MathHelper.DegreesToRadians(deltaY) * mouseSpeed);
            ChangeYaw(MathHelper.DegreesToRadians(deltaX) * mouseSpeed);

            if (Pitch > MathHelper.DegreesToRadians(89.0))
                ResetPitch((float)MathHelper.DegreesToRadians(89.0));
            else if (Pitch < MathHelper.DegreesToRadians(-89.0))
                ResetPitch((float)MathHelper.DegreesToRadians(-89.0));

            Vector3 front = new Vector3();
            front.X = (float)(Math.Cos(Pitch) * Math.Cos(Yaw));
            front.Y = (float)Math.Sin(Pitch);
            front.Z = (float)(Math.Cos(Pitch) * Math.Sin(Yaw));

            Front = Vector3.Normalize(front);
        }
        public void UpdateFront()
        {
            FrontChange?.Invoke("viewDir", Front);
        }
        public void UpdateViewMatrix()
        {
            View = Matrix4.LookAt(Position, Position + Front, Up);
            ViewMatrixChange?.Invoke("view", View);
        }
    }
}
