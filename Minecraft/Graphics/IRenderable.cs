using OpenTK.Mathematics;

namespace Minecraft.Graphics
{
    internal interface IRenderable
    {
        Shader Shader { get; }
        Texture? Texture { get; }
        Vector3? Color { get;}
        void Render();
    }
}
