using Minecraft.Render;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

namespace Minecraft.Graphics
{
    internal static class WireFrame
    {
        public static Shader? Shader { get; private set; } 

        public static void InitShader()
        {
            Shader = new Shader(@"..\..\..\Graphics\Shaders\Block\blockWireFrameVert.glsl",@"..\..\..\Graphics\Shaders\Block\blockWireFrameFrag.glsl");
        }
        private static int VBO = GL.GenBuffer();
        private static int VAO = GL.GenVertexArray();
        public static void Render(Vector3 position,FaceDirection face,Vector3 color)
        {
            var vertices = BlockFace.GetBlockFaceWireFrames(face, position);

            Shader?.Use();
            Shader?.SetVec3("frameColor", color);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BindVertexArray(VAO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.DepthFunc(DepthFunction.Lequal);
            GL.LineWidth(2.0f);
            GL.DrawArrays(PrimitiveType.Lines, 0, 8 * 6);
            GL.DepthFunc(DepthFunction.Less);
        }
    }
}
