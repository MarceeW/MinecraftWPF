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

        event ShaderMat4Handler? ViewMatrixChange;

        void ChangeFront(Vector3 front);
        void ChangePitch(float change);
        void ChangeYaw(float change);
        void Init();
        void ModPosition(Vector3 change);
        void ResetPitch(float resetValue);
        void SetPosition(Vector3 position);
        void UpdateViewMatrix();
    }
}