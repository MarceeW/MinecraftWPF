using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.UI.Logic
{
    internal interface IInventoryLogic
    {
        void CreateInventory();
        void SetupHotbar();
        void OnMouseEnterBlockImage(object sender, System.Windows.Input.MouseEventArgs e);
        void OnMouseLeaveBlockImage(object sender, System.Windows.Input.MouseEventArgs e);
        void CreateHotbar();
        void ReloadTextures();
        void UpdateHotbarItems();
    }
}
