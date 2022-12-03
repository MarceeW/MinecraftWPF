using Minecraft.Terrain;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Minecraft.Graphics
{
    internal class ChunkMesh
    {
        private int nFaceCount = 0, tFaceCount = 0, vFaceCount = 0;
        private int nVAO, tVAO, vVAO, nVBO, tVBO, vVBO;
        private bool hasData = false;
        public void RenderSolidMesh(Shader shader)
        {
            shader.Use();
            shader.SetInt("tex", AtlasTexturesData.Atlas.GetTexUnitId());

            GL.BindVertexArray(nVAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, nFaceCount * 6);
        }
        public void RenderTransparentMesh(Shader shader)
        {
            shader.Use();
            shader.SetInt("tex", AtlasTexturesData.Atlas.GetTexUnitId());

            if (tFaceCount > 0)
            {
                GL.Disable(EnableCap.CullFace);
                GL.BindVertexArray(tVAO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, tFaceCount * 6);
                GL.Enable(EnableCap.CullFace);
            }
            if (vFaceCount > 0)
            {
                GL.Disable(EnableCap.CullFace);
                GL.BindVertexArray(vVAO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, vFaceCount * 2 * 6);
                GL.Enable(EnableCap.CullFace);
            }
        }
        public void LoadToGPU(ChunkMeshRawData rawData)
        {
            vFaceCount=  rawData.vFaceCount;
            tFaceCount = rawData.tFaceCount;
            nFaceCount = rawData.nFaceCount;

            if (!hasData)
            {
                nVBO = GL.GenBuffer();
                nVAO = GL.GenVertexArray();

                tVBO = GL.GenBuffer();
                tVAO = GL.GenVertexArray();

                vVBO = GL.GenBuffer();
                vVAO = GL.GenVertexArray();
            }

            GL.BindVertexArray(nVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, nVBO);

            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * rawData.nVertices.Length, rawData.nVertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 9 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, 9 * sizeof(float), 8 * sizeof(float));
            GL.EnableVertexAttribArray(3);

            if (rawData.tVertices.Length > 0)
            {
                GL.BindVertexArray(tVAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, tVBO);

                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * rawData.tVertices.Length, rawData.tVertices, BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);

                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 9 * sizeof(float), 6 * sizeof(float));
                GL.EnableVertexAttribArray(2);

                GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, 9 * sizeof(float), 8 * sizeof(float));
                GL.EnableVertexAttribArray(3);
            }
            if (rawData.vVertices.Length > 0)
            {
                GL.BindVertexArray(vVAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vVBO);

                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * rawData.vVertices.Length, rawData.vVertices, BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);

                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 9 * sizeof(float), 6 * sizeof(float));
                GL.EnableVertexAttribArray(2);

                GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, 9 * sizeof(float), 8 * sizeof(float));
                GL.EnableVertexAttribArray(3);
            }

            hasData = true;
        }
    }
}