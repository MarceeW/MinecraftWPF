using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Game;
using Minecraft.Graphics;
using Minecraft.Render;
using Minecraft.Terrain;
using Minecraft.UI;
using OpenTK.Mathematics;
using System;
using System.IO;
using System.Windows.Media;

namespace Minecraft.Controller
{
    class GameSession
    {
        public IPlayer Player { get; private set; }
        public IWorld World { get; private set; }
        public WorldData WorldData { get; private set; }
        public bool IsNew { get; }

        public GameSession(WorldData worldData,bool isNewWorld)
        {
            IsNew = isNewWorld;
            WorldData = worldData;
            Player = new Player();

            if (isNewWorld)
            {
                World = new World(WorldData.WorldSeed);
                Ioc.Default.GetService<IHotbar>().Reset();
            }
            else
            {
                World = new World(WorldSerializer.LoadWorld(WorldData.WorldPath),worldData.WorldSeed);
                var ppraw = File.ReadAllLines(WorldSerializer.SavesLocation + @"\" + WorldData.WorldName + @"\" + "playerData.dat");
                var pp = ppraw[0].Split(';');
                Player.Position = new Vector3(float.Parse(pp[0]), float.Parse(pp[1]), float.Parse(pp[2]));
                Player.IsFlying = bool.Parse(pp[3]);
                var pcf = ppraw[1].Split(';');
                Player.Camera.Front = new Vector3(float.Parse(pcf[0]), float.Parse(pcf[1]), float.Parse(pcf[2]));
                Player.Hotbar.Deserialize(ppraw[2]);
            }
        }
        public void Save()
        {
            StreamWriter worldData = new StreamWriter(WorldSerializer.SavesLocation + @"\" + WorldData.WorldName + @"\" + "worldInfo.dat");
            worldData.WriteLine(WorldData.WorldName);
            worldData.WriteLine(WorldData.WorldSeed);
            worldData.WriteLine(DateTime.Now);
            worldData.Close();

            StreamWriter playerData = new StreamWriter(WorldSerializer.SavesLocation + @"\" + WorldData.WorldName + @"\" + "playerData.dat");
            playerData.WriteLine($"{Player.Position.X};{Player.Position.Y};{Player.Position.Z};{Player.IsFlying}");
            playerData.WriteLine($"{Player.Camera.Front.X};{Player.Camera.Front.Y};{Player.Camera.Front.Z}");
            playerData.WriteLine(Player.Hotbar);
            playerData.Close();

            WorldSerializer.SaveWorld(WorldData.WorldPath);
        }
    }
}
