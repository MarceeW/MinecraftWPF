using Minecraft.Terrain;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.Graphics
{
    internal struct ChunkMeshRawData
    {
        internal float[] nVertices;
        internal float[] tVertices;
        internal float[] vVertices;

        internal IChunk Owner;

        internal int nFaceCount;
        internal int tFaceCount;
        internal int vFaceCount;
        public ChunkMeshRawData(List<float> nVerticesList, List<float> tVerticesList, List<float> vVerticesList,IChunk owner,int nFaceCount, int tFaceCount, int vFaceCount)
        {
            nVertices = nVerticesList.ToArray();
            tVertices = tVerticesList.ToArray();
            vVertices = vVerticesList.ToArray();

            Owner = owner;

            this.nFaceCount = nFaceCount;
            this.tFaceCount = tFaceCount;
            this.vFaceCount = vFaceCount;
        }
    }
}
