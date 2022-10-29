using Minecraft.Graphics;
using Minecraft.Logic;
using Minecraft.UI;
using OpenTK.Mathematics;

namespace Minecraft.Game
{
    internal interface IPlayer
    {
        ICamera Camera { get; }
        IForce Force { get; }
        IHotbar Hotbar { get; }
        bool IsFlying { get; set; }
        Vector3 Position { get; set; }
    }
}