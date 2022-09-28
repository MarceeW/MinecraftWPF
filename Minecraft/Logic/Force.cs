using Minecraft.Game;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.Logic
{
    enum ForceType { Rise, Fall }
    internal class Force
    {
        private const float gravityStrength = 0.5f;
        private const float maxFallSpeed = 1.0f; 

        private double forceGraphStep;
        private double currentStep;
        private ForceType forceType;
        public Force()
        {
            const int steps = 360;
            forceGraphStep = Math.PI / steps;
            currentStep = 0;
        }
        public void SetForceType(ForceType type)
        {
            currentStep = 0;
            forceType = type;
        }
        public void Apply(ref Vector3 deltaPos)
        {
            double deltaY = 0;

            if(forceType == ForceType.Rise)
            {
                deltaY = Math.Sin(currentStep + forceGraphStep) - Math.Sin(currentStep) * gravityStrength;
                if (currentStep >= Math.PI / 2)
                    SetForceType(ForceType.Fall);
            }
            else
            {
                 deltaY = -(Math.Sin(currentStep + forceGraphStep) - Math.Sin(currentStep)) / gravityStrength;
            }
            currentStep += forceGraphStep;
            deltaPos.Y += (float)deltaY / 5;
        }
    }
}
