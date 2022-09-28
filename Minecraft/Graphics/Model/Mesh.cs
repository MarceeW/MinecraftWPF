using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Graphics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Minecraft.Graphics.Model
{
    public struct Texture
    {
        public uint id;
        public string type,path;
    }
    public struct Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 texCoord;

        public Vertex(Vector3 position, Vector3 normal, Vector2 texCoord)
        {
            this.position = position;
            this.normal = normal;
            this.texCoord = texCoord;
        }
    }
    internal class Mesh
    {
        public Vertex[] Vertices;
        public Texture[] Textures;
        public uint[] Indices;

        private int VBO, VAO, EBO;

        public Mesh(List<Vertex> vertices, List<Texture> textures, List<uint> indices)
        {
            Vertices = vertices.ToArray();
            Textures = textures.ToArray();
            Indices = indices.ToArray();

            setupMesh();
        }
        public void Draw(Shader shader)
        {
            uint diffuseNr = 1;
            uint specularNr = 1;

            int i = 0;
            foreach (var tex in Textures)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i);

                string number = "";
                string name = tex.type;
                if (name == "texture_diffuse")
                    number = (diffuseNr++).ToString();
                else if(name == "texture_specular")
                    number = (specularNr++).ToString();

                shader.SetInt(name + number, i);
                GL.BindTexture(TextureTarget.Texture2D, tex.id);
                i++;
            }
            
            GL.BindVertexArray(VAO);
            GL.DrawElements(BeginMode.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            GL.ActiveTexture(TextureUnit.Texture0);
        }

        private void setupMesh()
        {
            VBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();
            EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * Marshal.SizeOf<Vertex>(), Vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer,EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("normal"));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("texCoord"));

            GL.BindVertexArray(0);
        }
    }
}
