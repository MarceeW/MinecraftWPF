using Minecraft.Game;
using Minecraft.Terrain;
using System;

namespace Minecraft.Logic
{
    internal interface IPlayerLogic
    {
        bool Crouch { get; set; }
        bool Sprint { get; set; }
        event Action<float>? Walking;

        void Init(IPlayer player, IWorld world);
        void Jump();
        void Move(Direction dir, float delta);
        void Update(float delta);
    }
}