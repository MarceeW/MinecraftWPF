using OpenTK.Mathematics;

namespace Minecraft.Logic
{
    internal interface IBoxCollider
    {
        float Height { get; }
        Vector3 Position { get; }
        float Width { get; }

        void Collision(ref Vector3 deltaPos, out bool headHit, out bool groundHit);
        void UpdatePosition(Vector3 pos);
    }
}