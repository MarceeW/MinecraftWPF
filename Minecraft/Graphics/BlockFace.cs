using Minecraft.Terrain;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Minecraft.Graphics
{
    internal enum FaceDirection
    {
        Top, Bot, Right, Left, Front, Back
    }
    internal static class FaceDirectionVectors
    {
        private static Dictionary<FaceDirection, Vector3>? faceDirVectors;
        public static Dictionary<FaceDirection, Vector3> Vectors
        {
            get
            {
                if (faceDirVectors == null)
                {
                    faceDirVectors = new Dictionary<FaceDirection, Vector3>();

                    faceDirVectors.Add(FaceDirection.Top, Vector3.UnitY);
                    faceDirVectors.Add(FaceDirection.Bot, -Vector3.UnitY);
                    faceDirVectors.Add(FaceDirection.Right, Vector3.UnitX);
                    faceDirVectors.Add(FaceDirection.Left, -Vector3.UnitX);
                    faceDirVectors.Add(FaceDirection.Front, Vector3.UnitZ);
                    faceDirVectors.Add(FaceDirection.Back, -Vector3.UnitZ);
                }
                return faceDirVectors;
            }
        }
    }
    internal static class BlockFace
    {
        public const int SingleFaceVertexCount = 54;
        public static int SingleVertexFloats = 9;
        public static float[] GetBlockFaceVertices(BlockType? block, FaceDirection face, Vector3 position)
        {
            if(block == BlockType.Air || block == null)
                return new float[0];

            var texCoords = AtlasTexturesData.GetTextureCoords((BlockType)block, face);

            position += new Vector3(0.5f);

            if (block == BlockType.Water)
                position.Y -= 0.2f;

            if (face == FaceDirection.Top)
                return
                new float[SingleFaceVertexCount]
                {  //Positions                                                 //Normals            //Block Texture Coords        //Shade
                    -0.5f + position.X,  0.5f + position.Y, -0.5f + position.Z,  0.0f,  1.0f,  0.0f,  texCoords[0],  texCoords[3],  1.0f,
                     0.5f + position.X,  0.5f + position.Y, -0.5f + position.Z,  0.0f,  1.0f,  0.0f,  texCoords[2],  texCoords[3],  1.0f,
                     0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  0.0f,  1.0f,  0.0f,  texCoords[2],  texCoords[1],  1.0f,
                     0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  0.0f,  1.0f,  0.0f,  texCoords[2],  texCoords[1],  1.0f,
                    -0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  0.0f,  1.0f,  0.0f,  texCoords[0],  texCoords[1],  1.0f,
                    -0.5f + position.X,  0.5f + position.Y, -0.5f + position.Z,  0.0f,  1.0f,  0.0f,  texCoords[0],  texCoords[3],  1.0f 
                };
            else if (face == FaceDirection.Bot)
                return
                new float[SingleFaceVertexCount]
                {  //Positions                                                 //Normals            //Block Texture Coords        //Shade
                    -0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  0.0f, -1.0f,  0.0f,  texCoords[0],  texCoords[3],  0.5f,
                    -0.5f + position.X, -0.5f + position.Y,  0.5f + position.Z,  0.0f, -1.0f,  0.0f,  texCoords[0],  texCoords[1],  0.5f,
                     0.5f + position.X, -0.5f + position.Y,  0.5f + position.Z,  0.0f, -1.0f,  0.0f,  texCoords[2],  texCoords[1],  0.5f,
                     0.5f + position.X, -0.5f + position.Y,  0.5f + position.Z,  0.0f, -1.0f,  0.0f,  texCoords[2],  texCoords[1],  0.5f,
                     0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  0.0f, -1.0f,  0.0f,  texCoords[2],  texCoords[3],  0.5f,
                    -0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  0.0f, -1.0f,  0.0f,  texCoords[0],  texCoords[3],  0.5f   
                };
            else if (face == FaceDirection.Right)
                return
                new float[SingleFaceVertexCount]
                {  //Positions                                                 //Normals            //Block Texture Coords        //Shade
                     0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[3],  0.5f,
                     0.5f + position.X,  0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[3],  0.5f,
                     0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[1],  0.5f,
                     0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[1],  0.5f,
                     0.5f + position.X, -0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[1],  0.5f,
                     0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[3],  0.5f
                };
            else if (face == FaceDirection.Left)
                return
                new float[SingleFaceVertexCount]
                {  //Positions                                                 //Normals            //Block Texture Coords        //Shade
                    -0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[3],  0.5f,
                    -0.5f + position.X, -0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[1],  0.5f,
                    -0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[1],  0.5f,
                    -0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[1],  0.5f,
                    -0.5f + position.X,  0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[3],  0.5f,
                    -0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[3],  0.5f
                };
            else if (face == FaceDirection.Front)
                return
                new float[SingleFaceVertexCount]
                {  //Positions                                                 //Normals            //Block Texture Coords        //Shade
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  texCoords[0],  texCoords[1],  0.7f,
                    -0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  texCoords[0],  texCoords[3],  0.7f,
                     0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  texCoords[2],  texCoords[3],  0.7f,
                     0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  texCoords[2],  texCoords[3],  0.7f,
                     0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  texCoords[2],  texCoords[1],  0.7f,
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  texCoords[0],  texCoords[1],  0.7f
                };
            else
                return
                new float[SingleFaceVertexCount]
                {  //Positions                                                 //Normals            //Block Texture Coords        //Shade
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,  0.0f,  0.0f,  1.0f,  texCoords[0],  texCoords[1],  0.7f,
                     0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,  0.0f,  0.0f,  1.0f,  texCoords[2],  texCoords[1],  0.7f,
                     0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,  0.0f,  0.0f,  1.0f,  texCoords[2],  texCoords[3],  0.7f,
                     0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,  0.0f,  0.0f,  1.0f,  texCoords[2],  texCoords[3],  0.7f,
                    -0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,  0.0f,  0.0f,  1.0f,  texCoords[0],  texCoords[3],  0.7f,
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,  0.0f,  0.0f,  1.0f,  texCoords[0],  texCoords[1],  0.7f
                };
        }
        public static float[] GetVegetationFaceVertices(BlockType? block, Vector3 position)
        {
            var texCoords = AtlasTexturesData.GetTextureCoords((BlockType)block,null);

            position += new Vector3(0.5f);

                return
                new float[]
                {
                     0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[3],  0.5f,
                     0.5f + position.X,  0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[3],  0.5f,
                    -0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[1],  0.5f,
                    -0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[1],  0.5f,
                    -0.5f + position.X, -0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[1],  0.5f,
                     0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[3],  0.5f,

                    -0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[3],  0.5f,
                     0.5f + position.X, -0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[1],  0.5f,
                     0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[1],  0.5f,
                     0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[1],  0.5f,
                    -0.5f + position.X,  0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[3],  0.5f,
                    -0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[3],  0.5f
                };
        }
        public static float[] GetBlockFaceWireFrames(FaceDirection face, Vector3 position)
        {
            position += new Vector3(0.5f);

                return
                new float[]
                {  //Positions                                                
                    -0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,
                     0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,
                     0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,
                     0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,
                     0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,
                    -0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,
                    -0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,
                    -0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,
                                                            
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,
                     0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,
                     0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,
                     0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,
                     0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,
                                                            
                     0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,
                     0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,
                     0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,
                     0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,
                     0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,
                     0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,
                     0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,
                     0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,
                                                            
                    -0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,
                    -0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,
                    -0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,
                    -0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,
                                              
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,
                    -0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,
                    -0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,
                     0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,
                     0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,
                     0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,
                     0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,
                                             
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,
                    -0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,
                    -0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,
                     0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,
                     0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,
                     0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,
                     0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z
                };
        }
    }
}
