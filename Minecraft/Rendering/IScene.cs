using Minecraft.Graphics;

namespace Minecraft.Render
{
    internal interface IScene
    {
        event ShaderMat4Handler ProjectionMatrixChange;

        void Dispose();
        void OnProjectionMatrixChange();
        void Render(float delta);
    }
}