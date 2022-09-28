using Minecraft.Graphics.Shapes;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Minecraft.Graphics
{
    internal class Skybox : IRenderable
    {
        public Shader Shader { get; private set; }
        public Texture? Texture { get; private set; }
        public Vector3? Color { get; private set; }
        public Skybox(Shader shader,Texture texture)
        {
            Texture = texture;
            Shader = shader;
            Shader.SetInt("skybox", Texture.GetTexUnitId());
        }
        public void Render()
        {
            Shader.Use();
            Texture?.Use();

            GL.Disable(EnableCap.CullFace);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.BindVertexArray(Cube.GetVAO());
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.DepthFunc(DepthFunction.Less);
            GL.Enable(EnableCap.CullFace);
        }
    }
}
