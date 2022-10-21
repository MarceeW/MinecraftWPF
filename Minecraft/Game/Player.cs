using Minecraft.Graphics;
using Minecraft.Logic;
using Minecraft.UI;
using OpenTK.Mathematics;

namespace Minecraft.Game
{
    internal class Player
    {
        public Camera Camera { get; private set; }
        public const float PlayerHeight = 2.0f;
        public const float EyeHeight = 1.5f;
        public Vector3 Position
        {
            get
            {
                return Camera.Position;
            }
        }

        public bool IsFlying { get; private set; }
        public Force Force { get; private set; }
        public Hotbar Hotbar { get; }
        public Player(Vector3 position)
        {
            IsFlying = true;
            Camera = new Camera(position);
            Force = new Force();
            Hotbar = new Hotbar();
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
