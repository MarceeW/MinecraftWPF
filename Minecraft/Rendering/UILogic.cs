using Minecraft.Terrain;
using Minecraft.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.Rendering
{
    public class UILogic : IUILogic
    {
        GameWindow gw;
        public UILogic(GameWindow gw)
        {
            this.gw = gw;
        }
        public void ReadWorlds()
        {
            if (!Directory.Exists(WorldSerializer.SavesLocation))
                Directory.CreateDirectory(WorldSerializer.SavesLocation);

            var worldPaths = Directory.GetDirectories(WorldSerializer.SavesLocation);

            List<WorldData> savesData = new List<WorldData>();

            foreach (var path in worldPaths)
            {
                var files = Directory.GetFiles(path);
                if (files.Length >= 2)
                {
                    var worldData = File.ReadAllLines(files.Where(fileName => fileName.Contains("worldInfo")).First());
                    savesData.Add(new WorldData() { WorldName = worldData[0], WorldSeed = int.Parse(worldData[1]), LastPlayed = DateTime.Parse(worldData[2]), WorldPath = path });
                }
            }
            gw.WorldSelector.ItemsSource = savesData.OrderByDescending(x => x.LastPlayed).ToList();
        }
    }
}
