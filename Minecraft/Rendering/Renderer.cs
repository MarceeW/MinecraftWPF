using Assimp;
using Minecraft.Controller;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Diagnostics;
using System.Windows;

namespace Minecraft.Render
{
    class Renderer : IDisposable
    {
        public event Action<float>? OnRendering;

        public IScene? Scene { get; set; }
        public void Dispose()
        {
            Scene?.Dispose();
        }
        public void SetupRenderer(int width,int height)
        {
            GL.Enable(EnableCap.Multisample);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);
            GL.FrontFace(FrontFaceDirection.Ccw);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            Scene?.OnProjectionMatrixChange((float)width / height);
        }
        public void RenderFrame(float delta)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            OnRendering?.Invoke(delta);
            //LineRenderer.WireWrame(camera.Position - new Vector3(0.5f), new Vector3(0.0f));
            //LineRenderer.Axes(camera.Position - new Vector3(0.5f));
            Scene?.Render(delta);
        }
    }
}
