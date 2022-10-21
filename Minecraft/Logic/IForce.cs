using OpenTK.Mathematics;

namespace Minecraft.Logic
{
    internal interface IForce
    {
        void Apply(out Vector3 deltaPos, ref bool riseState);
        void Reset();
        void SetForceType(ForceType type);
    }
}