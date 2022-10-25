using OpenTK.Mathematics;

namespace Minecraft.Graphics
{
    internal interface ICamera
    {
        float Fov { get; set; }
        Vector3 Front { get; set; }
        float Pitch { get; }
        Vector3 Position { get; }
        Vector3 Up { get; }
        Matrix4 View { get; }
        float Yaw { get; }

        event ShaderVec3Handler? FrontChange;
        event ShaderMat4Handler? ViewMatrixChange;

        void ChangeFront(Vector3 front);
        void ChangePitch(float change);
        void ChangeView(float deltaX, float deltaY, float mouseSpeed);
        void ChangeYaw(float change);
        void Init(Vector3 startPos);
        void ModPosition(Vector3 change);
        void ResetPitch(float resetValue);
        void SetPosition(Vector3 position);
        void UpdateFront();
        void UpdateViewMatrix();
    }
}