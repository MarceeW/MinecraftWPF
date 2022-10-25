namespace Minecraft.Logic
{
    internal interface IPlayerLogic
    {
        void Jump();
        void Move(Direction dir, float delta);
        void Update(float delta);
    }
}