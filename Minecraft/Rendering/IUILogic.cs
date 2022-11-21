using Minecraft.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.Rendering
{
    internal interface IUILogic
    {
        void ReadWorlds();
        void EnterWorld(GameSession session);
        void UpdateHotbarItems();
        void CreateWorld(string name, string seed);
        void OpenCloseInventory();
        void ReloadTextures();
        void CreateHotbar();
        void SetupBindings();
        void LoadSettingsIntoControls();
        void ResetMousePosition();
        void OpenClosePauseMenu();
        void OpenCloseSettingsMenu();
        void OpenCloseMainMenu();
        public void OpenCloseWorldSelectorMenu();
        public void OpenCloseWorldCreationMenu();
    }
}
