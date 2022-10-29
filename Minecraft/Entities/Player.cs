using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Graphics;
using Minecraft.Logic;
using Minecraft.Terrain;
using Minecraft.UI;
using OpenTK.Mathematics;

namespace Minecraft.Game
{
    internal class Player : IPlayer
    {
        public ICamera Camera { get; private set; }
        public bool IsFlying { get; set; } = false;
        public Vector3 Position
        {
            get
            {
                return Camera.Position - new Vector3(0, 1, 0);
            }
            set
            {
                Camera.SetPosition(value + new Vector3(0, 1, 0));
            }
        }

        public IForce Force { get; private set; }
        public IHotbar Hotbar { get; }
        public Player(Vector3 position)
        {
            Camera = Ioc.Default.GetService<ICamera>();
            Force = Ioc.Default.GetService<IForce>();
            Hotbar = Ioc.Default.GetService<IHotbar>();

            Position = position;
        }
    }
}
