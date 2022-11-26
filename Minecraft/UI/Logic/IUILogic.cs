using Minecraft.Controller;
using Minecraft.Misc;
using System;
using System.Windows.Input;

namespace Minecraft.UI.Logic
{
    public interface IUILogic
    {
        GameWindow GameWindow { get; set; }
        bool IsGamePaused { get; set; }
        bool IsInventoryOpened { get; set; }
        bool IsPauseMenuOpened { get; set; }
        bool IsSettingsMenuOpened { get; set; }

        event Action<bool>? Pause;

        void OnSaveAndExit();
        void CreateWorld(string name, string seed);
        void EnterWorld(GameSession? gameSession = null);
        void LoadSettingsIntoControls();
        void OnKeyDown(object sender, KeyEventArgs e);
        void OpenCloseMainMenu();
        void OpenClosePauseMenu();
        void OpenCloseSettingsMenu();
        void OpenCloseWorldCreationMenu();
        void OpenCloseWorldSelectorMenu();
        void PauseGame();
        void ReadWorlds();
        void ResetMousePosition();
        void SetupBindings();
    }
}