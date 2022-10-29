using OpenTK.Mathematics;

namespace Minecraft.Logic
{
    internal interface IForce
    {
        ForceType Type { get; }

        void ApplyGravity(out Vector3 deltaPos);
        void Reset();
        void SetForceType(ForceType type);
    }
}