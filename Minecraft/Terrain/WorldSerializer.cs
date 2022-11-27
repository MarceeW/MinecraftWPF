using OpenTK.Mathematics;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Minecraft.Terrain
{
    internal static class WorldSerializer
    {
        public static IWorld? World { get; set; } = null;
        public static string SavesLocation = @"..\..\..\Saves";
        public static void SaveWorld(string path)
        {
            if (World != null)
            {
                Stream stream = File.OpenWrite(path + @"\world.bin");

                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(stream, World.Chunks);
                stream.Close();
            }
        }
        public static Dictionary<Vector2, IChunk> LoadWorld(string path)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = File.OpenRead(path + @"\world.bin");

            var world = formatter.Deserialize(stream) as Dictionary<Vector2, IChunk>;

            return world;

            return null;
        }
    }
}
