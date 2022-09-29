using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.Terrain
{
    internal static class EntityData
    {
        public static Dictionary<EntityType,Entity> Entities
        {
            get
            {

            }
        }
        private static Dictionary<EntityType,Entity> entities = new Dictionary<EntityType,Entity>();
        private static Dictionary<EntityType,Entity> LoadEntities()
        {
            var ret = new Dictionary<EntityType, Entity>();
        }
    }
}
