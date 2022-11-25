using OpenTK.Mathematics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
using Assimp;
using Assimp.Configs;
using System.Collections.Generic;
using System.IO;

namespace Minecraft.Graphics.Model
{
    internal class Model
    {
        public Model(string basePath, string fileName,bool needToFlipTextures = true)
        {
            directory = basePath;
            this.needToFlipTextures = needToFlipTextures;
            loadModel(Path.Combine(basePath,fileName));
        }
        public void Draw(Shader shader)
        {
            meshes.ForEach(x => x.Draw(shader));
        }
        List<Mesh> meshes;
        Dictionary<string, Texture> loadedTextures;
        string directory;
        bool needToFlipTextures;

        void loadModel(string path)
        {
            meshes = new List<Mesh>();
            loadedTextures = new Dictionary<string, Texture>();

            AssimpContext importer = new AssimpContext();
            Scene scene = importer.ImportFile(path,PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);

            processNode(scene.RootNode, scene);
        }
        void processNode(Node node,Scene scene)
        {
            foreach (int ind in node.MeshIndices)
            {
                Assimp.Mesh mesh = scene.Meshes[ind];
                meshes.Add(processMesh(mesh,scene));
            }
            foreach (var n in node.Children)
                processNode(n, scene);
        }
        Mesh processMesh(Assimp.Mesh mesh,Scene scene)
        {
            List<Vertex> Vertices = new List<Vertex>();
            List<Texture> Textures = new List<Texture>();
            List<uint> Indices = new List<uint>();

            for(int i = 0; i < mesh.Vertices.Count; i++)
            {
                Vector3D v = mesh.Vertices[i];
                Vector3D n = mesh.Normals[i];
                Vector2 t = new Vector2();

                if (mesh.TextureCoordinateChannelCount > 0)
                    t = new Vector2(mesh.TextureCoordinateChannels[0][i].X, mesh.TextureCoordinateChannels[0][i].Y);

                Vertices.Add(new Vertex(new Vector3(v.X, v.Y, v.Z), new Vector3(n.X, n.Y, n.Z), t));
            }
            foreach(var f in mesh.Faces)
                foreach(var i in f.Indices)
                    Indices.Add((uint)i);

            if(mesh.MaterialIndex > 0)
            {
                Material material = scene.Materials[mesh.MaterialIndex];

                List<Texture> diffuseMaps = loadMaterialTextures(material, TextureType.Diffuse, "texture_diffuse");
                List<Texture> specularMaps = loadMaterialTextures(material, TextureType.Specular, "texture_specular");

                Textures.AddRange(diffuseMaps);
                Textures.AddRange(specularMaps);
            }

            return new Mesh(Vertices, Textures, Indices);
        }
        List<Texture> loadMaterialTextures(Material material,TextureType type,string typeName)
        {
            List<Texture> materialTextures = new List<Texture>();

            foreach (var t in material.GetMaterialTextures(type))
            {
                string path = Path.Join(directory, t.FilePath);

                if (loadedTextures.ContainsKey(path))
                    materialTextures.Add(loadedTextures[path]);
                else
                {
                    Graphics.Texture tex = new Graphics.Texture(path, false,needToFlipTextures);
                    
                    Texture texture = new Texture();
                    texture.id = (uint)tex.Handle;
                    texture.path = Path.Combine(directory, t.FilePath);
                    texture.type = typeName;
                    
                    loadedTextures.Add(path, texture);
                    materialTextures.Add(texture);
                }
            }

            return materialTextures;
        }
    }
}
