using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Minecraft.Terrain
{
    class Entity
    {
        int Width { get; }
        int Height { get; }
        int Depth { get; }
        KeyValuePair<Vector3, BlockType>[] Blocks { get; }
        public Entity(XElement entity)
        {
            var blocks = entity.DescendantNodes();
            ;
            //Blocks = new KeyValuePair<Vector3, BlockType>[];
        }
    }
}
