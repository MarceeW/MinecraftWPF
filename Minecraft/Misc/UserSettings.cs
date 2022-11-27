using System.IO;

namespace Minecraft.Misc
{
    class UserSettings
    {
        public static string UserSettingsFilePath = @"..\..\..\Config\usersettings.dat";
        public float Fov { get; }
        public int RenderDistance { get; }
        public float MouseSpeed { get; }
        public UserSettings()
        {
            var rawData = File.ReadAllLines(UserSettingsFilePath);
            Fov = float.Parse(rawData[0]);
            RenderDistance = int.Parse(rawData[1]);
            MouseSpeed = float.Parse(rawData[2]);
        }
        public UserSettings(float fov, int renderDistance, float mouseSpeed)
        {
            Fov = fov;
            RenderDistance = renderDistance;
            MouseSpeed = mouseSpeed;
        }

        public static void Save(float fov, int renderDistance, float mouseSpeed)
        {
            StreamWriter sw = new StreamWriter(UserSettingsFilePath);
            sw.WriteLine(fov);
            sw.WriteLine(renderDistance);
            sw.WriteLine(mouseSpeed);
            sw.Close();
        }
    }
}
