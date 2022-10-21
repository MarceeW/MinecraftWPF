using OpenTK.Mathematics;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Minecraft.Terrain
{
    internal static class WorldSerializer
    {
        private static string worldPath = @"..\..\..\Saves\world.bin";
        public static World? World { get; set; } = null;
        public static void SaveWorld()
        {
            if(World != null)
            {
                Stream stream = File.OpenWrite(worldPath);

                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(stream, World.Chunks);
                stream.Close();
            }  
        }
        public static Dictionary<Vector2, Chunk> LoadWorld()
        {
            if (WorldFileExists())
            {
                BinaryFormatter formatter = new BinaryFormatter();

                FileStream stream = File.OpenRead(worldPath);

                var world = formatter.Deserialize(stream) as Dictionary<Vector2,Chunk>;

                return world;
            }
            return null;
        }
        public static bool WorldFileExists()
        {
            return File.Exists(worldPath);
        }
    }
}
