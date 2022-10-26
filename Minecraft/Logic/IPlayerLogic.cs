namespace Minecraft.Logic
{
    internal interface IPlayerLogic
    {
        bool Crouch { get; set; }
        bool Sprint { get; set; }

        void Jump();
        void Move(Direction dir, float delta);
        void Update(float delta);
    }
}