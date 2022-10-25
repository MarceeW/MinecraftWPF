using OpenTK.Mathematics;

namespace Minecraft.Logic
{
    internal interface IBoxCollider
    {
        Vector3 Position { get; set; }
        float Height { get; }
        float Width { get; }
        void Collision(ref Vector3 deltaPos,out bool headHit, out bool groundHit);
    }
}