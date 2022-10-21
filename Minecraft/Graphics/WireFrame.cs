using Minecraft.Render;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

namespace Minecraft.Graphics
{
    internal static class LineRenderer
    {
        public static Shader? Shader { get; private set; } 

        public static void InitShader()
        {
            Shader = new Shader(@"..\..\..\Graphics\Shaders\Block\blockWireFrameVert.glsl",@"..\..\..\Graphics\Shaders\Block\blockWireFrameFrag.glsl");
        }
        private static int wireFrameVBO = GL.GenBuffer();
        private static int wireFrameVAO = GL.GenVertexArray();

        private static int axisVBO = GL.GenBuffer();
        private static int axisVAO = GL.GenVertexArray();
        public static void Axes(Vector3 position)
        {
            Shader?.Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, axisVBO);
            GL.BindVertexArray(axisVAO);

            float[] vertices =
            {
                position.X,     position.Y,     position.Z,     1, 0, 0,
                position.X + 1, position.Y,     position.Z,     1, 0, 0,

                position.X,     position.Y,     position.Z,     0, 1, 0,
                position.X,     position.Y + 1, position.Z,     0, 1, 0,
                
                position.X,     position.Y,     position.Z,     0, 0, 1,
                position.X,     position.Y,     position.Z + 1, 0, 0, 1
            };

            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.DepthFunc(DepthFunction.Lequal);
            GL.LineWidth(2.0f);
            GL.DrawArrays(PrimitiveType.Lines, 0, 6);
            GL.DepthFunc(DepthFunction.Less);
        }
        public static void WireWrame(Vector3 position,Vector3 color)
        {
            var vertices = Face.GetBlockFaceWireFrames(position,color);

            Shader?.Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, wireFrameVBO);
            GL.BindVertexArray(wireFrameVAO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.DepthFunc(DepthFunction.Lequal);
            GL.LineWidth(2.0f);
            GL.DrawArrays(PrimitiveType.Lines, 0, 8 * 6);
            GL.DepthFunc(DepthFunction.Less);
        }
    }
}
