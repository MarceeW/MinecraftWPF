using Minecraft.Graphics;
using Minecraft.Logic;
using OpenTK.Mathematics;

namespace Minecraft.Game
{
    internal class Player
    {
        public Camera Camera { get; private set; }

        public bool IsFlying { get; private set; }
        public Force Force { get; private set; }
        private Inventory inventory;
        public Player(Vector3 position)
        {
            IsFlying = true;
            Camera = new Camera(position);
            Force = new Force();
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
