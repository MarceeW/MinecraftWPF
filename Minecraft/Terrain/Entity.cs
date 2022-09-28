using OpenTK.Mathematics;
using System.Collections.Generic;
using System.IO;

namespace Minecraft.Terrain
{
    internal enum EntityType
    {
        Tree
    }
    internal static class Entity
    {
        private static Dictionary<EntityType, KeyValuePair<BlockType, Vector3>[]>? entityPositions;
        public static Dictionary<EntityType, KeyValuePair<BlockType, Vector3>[]> EntityPositions
        {
            get
            {
                if(entityPositions == null)
                {
                    entityPositions = new Dictionary<EntityType, KeyValuePair<BlockType, Vector3>[]>();

                    var rawData = File.ReadAllLines(@"..\..\..\Assets\Terrain\Entities.ent");

                    EntityType currentType = 0;
                    foreach(var line in rawData)
                    {
                        if (line[0] == '/')
                            currentType = GetEntityTypeByString(line.Split('/')[0]);
                        else
                        {
                            var entityRawData = line.Split(',');
                            KeyValuePair<BlockType, Vector3>[] positions = new KeyValuePair<BlockType, Vector3>[entityRawData.Length];

                            int i = 0;
                            foreach(var block in entityRawData)
                            {
                                var blockRawData = block.Split(':');
                                positions[i] = new KeyValuePair<BlockType, Vector3>((BlockType)int.Parse(blockRawData[3]), new Vector3(float.Parse(blockRawData[0]), float.Parse(blockRawData[1]), float.Parse(blockRawData[2])));
                                i++;
                            }
                            entityPositions.Add(currentType, positions);
                        }
                    }
                }

                return entityPositions;
            }
        }
        private static EntityType GetEntityTypeByString(string name)
        {
            EntityType type = 0;

            if (name == "Tree")
                type = EntityType.Tree;

            return type;
        }
    }
}
