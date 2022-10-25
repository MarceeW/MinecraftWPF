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
        public bool IsFlying { get; set; } = true;
        public Vector3 Position
        {
            get
            {
                return Camera.Position - new Vector3(0,1,0);
            }
        }

        public Force Force { get; private set; }
        public Hotbar Hotbar { get; }
        public Player(Vector3 position)
        {
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
