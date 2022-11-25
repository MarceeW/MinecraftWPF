using OpenTK.Mathematics;
using System;

namespace Minecraft.Logic
{
    enum ForceType { Rise, Fall }
    internal class Force : IForce
    {
        public ForceType Type { get; private set; }
        public Direction SlideDirection { get; set; }

        private const float maxFallSpeed = 100.0f;
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
        public void ApplyGravity(out Vector3 deltaPos)
        {

            double deltaY = 0;
            deltaPos = new Vector3();

            if (Type == ForceType.Rise)
            {
                deltaY = Math.Sin(currentStep += forceGraphStep) * riseForce;
                if (currentStep >= Math.PI)
                    SetForceType(ForceType.Fall);
            }
            else
                deltaY = -Math.Min(Math.Pow(Math.E, currentStep += forceGraphStep / 2), maxFallSpeed);
            deltaPos.Y += (float)deltaY;
        }
    }
}
