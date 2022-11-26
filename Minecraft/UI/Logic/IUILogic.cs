using Minecraft.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.UI.Logic
{
    internal interface IUILogic
    {
        void ReadWorlds();
        void EnterWorld(GameSession session);
        void CreateWorld(string name, string seed);
        void OpenCloseInventory();
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
