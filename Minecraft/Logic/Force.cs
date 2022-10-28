using OpenTK.Mathematics;
using System;

namespace Minecraft.Logic
{
    enum ForceType { Rise, Fall }
    internal class Force : IForce
    {
        public ForceType Type { get; private set; }

        private const float maxFallSpeed = 60.0f;
        private const float riseForce = 10.0f;

        private double forceGraphStep;
        private double currentStep;
        public Force()
        {
            const int steps = 25;
            forceGraphStep = Math.PI / steps;
            currentStep = 0;
        }
        public void SetForceType(ForceType type)
        {
            currentStep = 0;
            Type = type;
        }
        public void Reset()
        {
            currentStep = 0;
        }
        public void Apply(out Vector3 deltaPos)
        {
            double deltaY = 0;
            deltaPos = new Vector3();

            if (Type == ForceType.Rise)
            {
                deltaY = Math.Sin(currentStep += forceGraphStep) * riseForce;
                if (currentStep >= Math.PI)
                {
                    SetForceType(ForceType.Fall);
                }
            }
            else
            {
                deltaY = -Math.Min(Math.Pow(2, currentStep += forceGraphStep), maxFallSpeed);
            }
            deltaPos.Y += (float)deltaY;
        }
    }
}
