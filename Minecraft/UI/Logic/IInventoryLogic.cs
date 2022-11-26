using System.Windows.Input;

namespace Minecraft.UI.Logic
{
    public interface IInventoryLogic
    {
        GameWindow GameWindow { get; set; }
        IHotbar Hotbar { get; }
        Inventory Inventory { get; }
        bool IsOpened { get; }

        void CreateHotbar();
        void CreateInventory();
        void OnHotbarMouseDown(object sender, MouseButtonEventArgs e);
        void OnInventoryItemMouseDown(object sender, MouseButtonEventArgs e);
        void OnKeyDown(object sender, KeyEventArgs e);
        void OnMouseDown(object sender, MouseButtonEventArgs e);
        void OnMouseEnterBlockImage(object sender, MouseEventArgs e);
        void OnMouseLeaveBlockImage(object sender, MouseEventArgs e);
        void OnMouseMove(object sender, MouseEventArgs e);
        void OnMouseWheel(object sender, MouseWheelEventArgs e);
        void OpenCloseInventory();
        void ReloadTextures();
        void SetupHotbar();
        void UpdateHotbarItems();
    }
}