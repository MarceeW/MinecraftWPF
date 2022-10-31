using Assimp;
using Minecraft.Terrain;
using OpenTK.Mathematics;
using System;
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
    internal static class Face
    {
        public const int SingleFaceVertexCount = 54;
        public static int SingleVertexFloats = 9;
        public static float[] GetBlockFaceVertices(BlockType? block, FaceDirection face, Vector3 position, bool needsToShade, int topBlockDifference, float? shadeValue = null)
        {
            if(block == BlockType.Air || block == null)
                return new float[0];

            var texCoords = AtlasTexturesData.GetTextureCoords((BlockType)block, face);

            position += new Vector3(0.5f);

            if (block == BlockType.Water || block == BlockType.Lava)
                position.Y -= 0.2f;

            if (face == FaceDirection.Top)
            {
                float shade = Math.Clamp(1 - topBlockDifference * 0.065f, 0.4f, 1.0f);

                return
                new float[SingleFaceVertexCount]
                {  //Positions                                                 //Normals            //Block          Texture Coord  Shade
                    -0.5f + position.X,  0.5f + position.Y, -0.5f + position.Z,  0.0f,  1.0f,  0.0f,  texCoords[0],  texCoords[3], needsToShade ? shade : 1.0f,
                     0.5f + position.X,  0.5f + position.Y, -0.5f + position.Z,  0.0f,  1.0f,  0.0f,  texCoords[2],  texCoords[3], needsToShade ? shade : 1.0f,
                     0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  0.0f,  1.0f,  0.0f,  texCoords[2],  texCoords[1], needsToShade ? shade : 1.0f,
                     0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  0.0f,  1.0f,  0.0f,  texCoords[2],  texCoords[1], needsToShade ? shade : 1.0f,
                    -0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  0.0f,  1.0f,  0.0f,  texCoords[0],  texCoords[1], needsToShade ? shade : 1.0f,
                    -0.5f + position.X,  0.5f + position.Y, -0.5f + position.Z,  0.0f,  1.0f,  0.0f,  texCoords[0],  texCoords[3], needsToShade ? shade : 1.0f
                };
            }     
            else if (face == FaceDirection.Bot)
                return
                new float[SingleFaceVertexCount]
                {  //Positions                                                 //Normals            //Block Texture Coords        //Shade
                    -0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  0.0f, -1.0f,  0.0f,  texCoords[0],  texCoords[3], shadeValue == null ? 0.5f : (float)shadeValue,
                    -0.5f + position.X, -0.5f + position.Y,  0.5f + position.Z,  0.0f, -1.0f,  0.0f,  texCoords[0],  texCoords[1], shadeValue == null ? 0.5f : (float)shadeValue,
                     0.5f + position.X, -0.5f + position.Y,  0.5f + position.Z,  0.0f, -1.0f,  0.0f,  texCoords[2],  texCoords[1], shadeValue == null ? 0.5f : (float)shadeValue,
                     0.5f + position.X, -0.5f + position.Y,  0.5f + position.Z,  0.0f, -1.0f,  0.0f,  texCoords[2],  texCoords[1], shadeValue == null ? 0.5f : (float)shadeValue,
                     0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  0.0f, -1.0f,  0.0f,  texCoords[2],  texCoords[3], shadeValue == null ? 0.5f : (float)shadeValue,
                    -0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  0.0f, -1.0f,  0.0f,  texCoords[0],  texCoords[3], shadeValue == null ? 0.5f : (float)shadeValue   
                };
            else if (face == FaceDirection.Right)
                return
                new float[SingleFaceVertexCount]
                {  //Positions                                                 //Normals            //Block Texture Coords        //Shade
                     0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[3], shadeValue == null ?  0.5f : (float)shadeValue,
                     0.5f + position.X,  0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[3], shadeValue == null ?  0.5f : (float)shadeValue,
                     0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[1], shadeValue == null ?  0.5f : (float)shadeValue,
                     0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[1], shadeValue == null ?  0.5f : (float)shadeValue,
                     0.5f + position.X, -0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[1], shadeValue == null ?  0.5f : (float)shadeValue,
                     0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[3], shadeValue == null ?  0.5f : (float)shadeValue
                };
            else if (face == FaceDirection.Left)
                return
                new float[SingleFaceVertexCount]
                {  //Positions                                                 //Normals            //Block Texture Coords        //Shade
                    -0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[3],  shadeValue == null ?  0.5f : (float)shadeValue,
                    -0.5f + position.X, -0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[1],  shadeValue == null ?  0.5f : (float)shadeValue,
                    -0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[1],  shadeValue == null ?  0.5f : (float)shadeValue,
                    -0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[1],  shadeValue == null ?  0.5f : (float)shadeValue,
                    -0.5f + position.X,  0.5f + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[3],  shadeValue == null ?  0.5f : (float)shadeValue,
                    -0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[3],  shadeValue == null ?  0.5f : (float)shadeValue
                };
            else if (face == FaceDirection.Front)
                return
                new float[SingleFaceVertexCount]
                {  //Positions                                                 //Normals            //Block Texture Coords        //Shade
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  texCoords[0],  texCoords[1],  shadeValue == null ?  0.7f : (float)shadeValue,
                    -0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  texCoords[0],  texCoords[3],  shadeValue == null ?  0.7f : (float)shadeValue,
                     0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  texCoords[2],  texCoords[3],  shadeValue == null ?  0.7f : (float)shadeValue,
                     0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  texCoords[2],  texCoords[3],  shadeValue == null ?  0.7f : (float)shadeValue,
                     0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  texCoords[2],  texCoords[1],  shadeValue == null ?  0.7f : (float)shadeValue,
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  texCoords[0],  texCoords[1],  shadeValue == null ?  0.7f : (float)shadeValue
                };
            else
                return
                new float[SingleFaceVertexCount]
                {  //Positions                                                 //Normals            //Block Texture Coords        //Shade
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,  0.0f,  0.0f,  1.0f,  texCoords[0],  texCoords[1],  shadeValue == null ?  0.7f : (float)shadeValue,
                     0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,  0.0f,  0.0f,  1.0f,  texCoords[2],  texCoords[1],  shadeValue == null ?  0.7f : (float)shadeValue,
                     0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,  0.0f,  0.0f,  1.0f,  texCoords[2],  texCoords[3],  shadeValue == null ?  0.7f : (float)shadeValue,
                     0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,  0.0f,  0.0f,  1.0f,  texCoords[2],  texCoords[3],  shadeValue == null ?  0.7f : (float)shadeValue,
                    -0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z,  0.0f,  0.0f,  1.0f,  texCoords[0],  texCoords[3],  shadeValue == null ?  0.7f : (float)shadeValue,
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,  0.0f,  0.0f,  1.0f,  texCoords[0],  texCoords[1],  shadeValue == null ?  0.7f : (float)shadeValue
                };
        }
        public static float[] GetHandFaceVertices(FaceDirection face,Vector3 position, float thickness)
        {
            if (face == FaceDirection.Top)
            {
                return
                new float[SingleFaceVertexCount]
                {
                    -0.5f + position.X, -0.5f + thickness + position.Y, -0.5f + position.Z,  0.0f,  1.0f,  0.0f,  0.25f,  0.75f, 1.0f,
                    -0.5f + thickness + position.X, -0.5f + thickness + position.Y, -0.5f + position.Z,  0.0f,  1.0f,  0.0f,  0.5f,   0.75f, 1.0f,
                    -0.5f + thickness + position.X, -0.5f + thickness + position.Y,  0.5f + position.Z,  0.0f,  1.0f,  0.0f,  0.5f,   0.0f, 1.0f,
                    -0.5f + thickness + position.X, -0.5f + thickness + position.Y,  0.5f + position.Z,  0.0f,  1.0f,  0.0f,  0.5f,   0.0f, 1.0f,
                    -0.5f + position.X, -0.5f + thickness + position.Y,  0.5f + position.Z,  0.0f,  1.0f,  0.0f,  0.25f,  0.0f, 1.0f,
                    -0.5f + position.X, -0.5f + thickness + position.Y, -0.5f + position.Z,  0.0f,  1.0f,  0.0f,  0.25f,  0.75f, 1.0f
                };
            }
            else if (face == FaceDirection.Left)
                return
                new float[SingleFaceVertexCount]
                {
                    -0.5f + position.X, -0.5f + thickness + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  0.0f,  0.0f,      1.0f, //front top x
                    -0.5f + position.X, -0.5f + position.Y,  0.5f + position.Z,              1.0f,  0.0f,  0.0f,  0.25f,  0.0f,      1.0f, //front bot x
                    -0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,              1.0f,  0.0f,  0.0f,  0.25f,   0.75f,      1.0f, //back bot x
                    -0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,              1.0f,  0.0f,  0.0f,  0.25f,   0.75f,      1.0f, //back bot x
                    -0.5f + position.X, -0.5f + thickness + position.Y, -0.5f + position.Z,  1.0f,  0.0f,  0.0f,  0.0f,   0.75f,      1.0f, //back top x
                    -0.5f + position.X, -0.5f + thickness + position.Y,  0.5f + position.Z,  1.0f,  0.0f,  0.0f,  0.0f,  0.0f,      1.0f //front top x
                };
            else if (face == FaceDirection.Front)
                return
                new float[SingleFaceVertexCount]
                {
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  0.5f,    0.75f,  1.0f,
                    -0.5f + position.X, -0.5f + thickness + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  0.5f,    1.0f,   1.0f,
                    -0.5f + thickness + position.X,  -0.5f + thickness + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  0.75f,   1.0f,   1.0f,
                    -0.5f + thickness + position.X,  -0.5f + thickness + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  0.75f,   1.0f,   1.0f,
                    -0.5f + thickness + position.X, -0.5f + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  0.75f,   0.75f,  1.0f,
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z,  0.0f,  0.0f, -1.0f,  0.5f,    0.75f,  1.0f
                };
            else
                return
                new float[SingleFaceVertexCount]
                {
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,  0.0f,  0.0f, -1.0f,  0.25f,  0.75f,  1.0f,
                    -0.5f + position.X, -0.5f + thickness + position.Y,  -0.5f + position.Z,  0.0f,  0.0f, -1.0f,  0.25f,  1.0f,   1.0f,
                    -0.5f + thickness + position.X, -0.5f + thickness + position.Y,  -0.5f + position.Z,  0.0f,  0.0f, -1.0f,  0.5f,   1.0f,   1.0f,
                    -0.5f + thickness + position.X, -0.5f + thickness + position.Y,  -0.5f + position.Z,  0.0f,  0.0f, -1.0f,  0.5f,   1.0f,   1.0f,
                    -0.5f + thickness + position.X, -0.5f + position.Y,  -0.5f + position.Z,  0.0f,  0.0f, -1.0f,  0.5f,   0.75f,  1.0f,
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z,  0.0f,  0.0f, -1.0f,  0.25f,  0.75f,  1.0f
                };
        }
        public static float[] GetVegetationFaceVertices(BlockType block, Vector3 position, bool needsToShade, int topBlockDifference)
        {
            var texCoords = AtlasTexturesData.GetTextureCoords(block,null);

            position += new Vector3(0.5f);

            float shade = Math.Clamp(1 - topBlockDifference * 0.065f, 0.4f, 1.0f);
            
            return
                new float[]
                {
                    -0.5f + position.X, -0.5f + position.Y,  0.5f + position.Z,  0.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[1], needsToShade? shade : 1.0f,
                     0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  0.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[1], needsToShade? shade : 1.0f,
                    -0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  0.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[3], needsToShade? shade : 1.0f,
                    -0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  0.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[3], needsToShade? shade : 1.0f,
                     0.5f + position.X,  0.5f + position.Y, -0.5f + position.Z,  0.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[3], needsToShade? shade : 1.0f,
                     0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  0.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[1], needsToShade? shade : 1.0f,
                                                                                                                                   
                    -0.5f + position.X, -0.5f + position.Y, -0.5f + position.Z,  0.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[1], needsToShade? shade : 1.0f,
                     0.5f + position.X, -0.5f + position.Y,  0.5f + position.Z,  0.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[1], needsToShade? shade : 1.0f,
                    -0.5f + position.X,  0.5f + position.Y, -0.5f + position.Z,  0.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[3], needsToShade? shade : 1.0f,
                    -0.5f + position.X,  0.5f + position.Y, -0.5f + position.Z,  0.0f,  0.0f,  0.0f,  texCoords[0],  texCoords[3], needsToShade? shade : 1.0f,
                     0.5f + position.X,  0.5f + position.Y,  0.5f + position.Z,  0.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[3], needsToShade? shade : 1.0f,
                     0.5f + position.X, -0.5f + position.Y,  0.5f + position.Z,  0.0f,  0.0f,  0.0f,  texCoords[2],  texCoords[1], needsToShade? shade : 1.0f,            
                };
        }
        public static float[] GetBlockFaceWireFrames(Vector3 position,Vector3 color)
        {
            position += new Vector3(0.5f);

                return
                new float[]
                {  //Positions                                                
                    -0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                                                                                 
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                                                                                 
                     0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                                                                                 
                    -0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                                                                                 
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X,  0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X, -0.5f + position.Y,   0.5f + position.Z, color.X, color.Y, color.Z,
                                                                                 
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X,  0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                     0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z,
                    -0.5f + position.X, -0.5f + position.Y,  -0.5f + position.Z, color.X, color.Y, color.Z
                };
        }
    }
}
