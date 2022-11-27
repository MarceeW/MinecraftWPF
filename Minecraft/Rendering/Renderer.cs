using OpenTK.Graphics.OpenGL;
using System;

namespace Minecraft.Render
{
    class Renderer
    {
        public event Action<float>? OnRendering;

        public IScene? Scene { get; set; }
        public void SetupRenderer(int width, int height)
        {
            GL.Enable(EnableCap.Multisample);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);
            GL.FrontFace(FrontFaceDirection.Ccw);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            Scene?.OnProjectionMatrixChange();
        }
        public void RenderFrame(float delta)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            OnRendering?.Invoke(delta);
            Scene?.Render(delta);
        }
    }
}
