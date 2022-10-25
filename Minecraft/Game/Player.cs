using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Graphics;
using Minecraft.Logic;
using Minecraft.UI;
using OpenTK.Mathematics;

namespace Minecraft.Game
{
    internal class Player : IPlayer
    {
        public ICamera Camera { get; private set; }
        public const float PlayerHeight = 2.0f;
        public const float EyeHeight = 1.5f;
        public bool IsFlying { get; set; } = false;
        public Vector3 Position
        {
            get
            {
                return Camera.Position - new Vector3(0, 1, 0);
            }
        }

        public IForce Force { get; private set; }
        public IHotbar Hotbar { get; }
        public Player(Vector3 position)
        {
            Camera = Ioc.Default.GetService<ICamera>();
            Force = Ioc.Default.GetService<IForce>();
            Hotbar = Ioc.Default.GetService<IHotbar>();

            SetPosition(position);
        }
        public Vector3 GetPosition()
        {
            return Camera.Position;
        }
        public void SetPosition(Vector3 position)
        {
            Camera.SetPosition(position);
        }
    }
}
