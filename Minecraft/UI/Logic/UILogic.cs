using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Controller;
using Minecraft.Graphics;
using Minecraft.Misc;
using Minecraft.Render;
using Minecraft.Terrain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace Minecraft.UI.Logic
{
    public class UILogic : IUILogic
    {
        public event Action<bool>? Pause;
        public GameWindow GameWindow
        {
            get
            {
                return gameWindow;
            }
            set
            {
                gameWindow = value;
                gameWindow.PreviewKeyDown += OnKeyDown;

                LoadSplashText();
                LoadSettingsIntoControls();
                SetupBindings();
            }
        }
        public bool NeedsToResetMouse = true;
        public static bool IsInMainMenu { get; set; } = true;
        public static bool IsHudVisible { get; private set; } = true;
        public bool IsInventoryOpened { get; set; } = false;
        public bool IsGamePaused { get; set; } = false;
        public bool IsSettingsMenuOpened { get; set; } = false;
        public bool IsPauseMenuOpened { get; set; } = false;

        private GameWindow? gameWindow;
        private UserSettings userSettings = new UserSettings();
        private GameController gameController;
        private void LoadSplashText()
        {
            var splashes = File.ReadAllLines(@"..\..\..\Assets\Splashes.txt");

            Random r = new Random();
            string text = splashes[r.Next(0, splashes.Length)];

            GameWindow.SplashText.Text = text;

            var margin = GameWindow.SplashText.Margin;
            margin.Right -= text.Length * 8;
            margin.Top += text.Length * 3;
            GameWindow.SplashText.Margin = margin;
        }
        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F3:
                    {
                        gameWindow.fpsCounter.Visibility = gameWindow.fpsCounter.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
                    }
                    break;
                case Key.F1:
                    {
                        IsHudVisible = !IsHudVisible;
                        gameWindow.HotbarGrid.Visibility = gameWindow.HotbarGrid.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
                        gameWindow.Crosshair.Visibility = gameWindow.Crosshair.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
                    }
                    break;
                case Key.G:
                    {
                        gameWindow.ShowWireFrames = !gameWindow.ShowWireFrames;
                        e.Handled = true;
                    }
                    break;
                case Key.Escape:
                    {

                        if (IsInventoryOpened)
                            Ioc.Default.GetService<IInventoryLogic>().OpenCloseInventory();
                        else if (IsSettingsMenuOpened)
                            OpenCloseSettingsMenu();
                        else
                            OpenClosePauseMenu();
                    }
                    break;
            }
        }
        public void PauseGame()
        {
            IsGamePaused = !IsGamePaused;

            Pause?.Invoke(IsGamePaused);

            if (IsGamePaused)
            {
                var effect = new BlurEffect();
                effect.Radius = 15;

                gameWindow.OpenTkControl.Effect = effect;
            }
            else
                gameWindow.OpenTkControl.Effect = null;

            gameWindow.PauseMenuDarkener.Visibility = gameWindow.PauseMenuDarkener.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            gameWindow.Crosshair.Visibility = gameWindow.Crosshair.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

            NeedsToResetMouse = !NeedsToResetMouse;

            if (NeedsToResetMouse)
                MouseController.HideMouse();
            else
                MouseController.ShowMouse();

        }
        public void ReadWorlds()
        {
            if (!Directory.Exists(WorldSerializer.SavesLocation))
                Directory.CreateDirectory(WorldSerializer.SavesLocation);

            var worldPaths = Directory.GetDirectories(WorldSerializer.SavesLocation);

            List<WorldData> savesData = new List<WorldData>();

            foreach (var path in worldPaths)
            {
                var files = Directory.GetFiles(path);
                if (files.Length >= 2)
                {
                    var worldData = File.ReadAllLines(files.Where(fileName => fileName.Contains("worldInfo")).First());

                    if (DateTime.TryParse(worldData[2], out DateTime date))
                        savesData.Add(new WorldData() { WorldName = worldData[0], WorldSeed = int.Parse(worldData[1]), LastPlayed = date, WorldPath = path, IsFlat = bool.Parse(worldData[3]) });
                    else
                        savesData.Add(new WorldData() { WorldName = worldData[0], WorldSeed = int.Parse(worldData[1]), LastPlayed = DateTime.Now, WorldPath = path, IsFlat = bool.Parse(worldData[3]) });

                }
            }
            GameWindow.WorldSelector.ItemsSource = savesData.OrderByDescending(x => x.LastPlayed).ToList();
        }
        public void EnterWorld(GameSession? gameSession = null)
        {
            GameSession session = null;

            if (GameWindow.WorldSelector.SelectedIndex >= 0)
                session = new GameSession(gameWindow.WorldSelector.SelectedItem as WorldData, false);   

            if (gameSession != null)
                session = gameSession;

            GameWindow.renderer = new Renderer();
            MouseController.HideMouse();

            if (IsInMainMenu)
            {
                IsInMainMenu = false;
                OpenCloseWorldSelectorMenu();
                GameWindow.OverWorldCover.Visibility = Visibility.Hidden;
            }

            GameWindow.Crosshair.Visibility = Visibility.Visible;
            gameController = new GameController((int)GameWindow.RenderDistanceSlider.Value, GameWindow.renderer, GameWindow, this, session);

            Ioc.Default.GetService<IInventoryLogic>().UpdateHotbarItems();
            SetupBindings();
            GameWindow.OpenTkControl.Focus();
        }
        public void CreateWorld(string name, string seed)
        {
            var worldDirs = from x in Directory.GetDirectories(WorldSerializer.SavesLocation)
                            select x.Split("\\").Last();
            if (name == "")
                name = "New World";

            string baseName = name;
            int i = 1;
            while (worldDirs.Contains(name))
            {
                name = $"{baseName} ({i})";
                i++;
            }

            string worldSaveDir = WorldSerializer.SavesLocation + @"\" + name;
            Directory.CreateDirectory(worldSaveDir);
            var worldData = new WorldData() { LastPlayed = DateTime.Now, WorldName = name, WorldSeed = WorldGenerator.GenerateSeed(seed), WorldPath = worldSaveDir, IsFlat = GameWindow.NormalFlatOption.IsChecked == true };

            IsInMainMenu = false;
            GameWindow.WorldName.Text = "New World";
            GameWindow.WorldSeed.Text = "";
            GameWindow.NormalFlatOption.IsChecked = false;

            GameWindow.WorldSelector.SelectedItem = null;
            GameWindow.WorldSelector.SelectedIndex = -1;

            OpenCloseWorldCreationMenu();

            EnterWorld(new GameSession(worldData, true));
        }
        public void SetupBindings()
        {
            GameWindow.FovSlider.DataContext = Ioc.Default.GetService<ICamera>();

            if (gameController != null)
            {
                GameWindow.RenderDistanceSlider.DataContext = gameController.WorldRendererer;
                GameWindow.SensitivitySlider.DataContext = gameController.PlayerController;
                gameController.InitUserSettings(new UserSettings((float)GameWindow.FovSlider.Value, (int)GameWindow.RenderDistanceSlider.Value, (float)GameWindow.SensitivitySlider.Value));
            }
        }
        public void OnSaveAndExit()
        {
            gameController.Dispose();
            ReadWorlds();
            OpenClosePauseMenu();
            OpenCloseMainMenu();
            MouseController.ShowMouse();
        }
        public void LoadSettingsIntoControls()
        {
            GameWindow.FovSlider.Value = userSettings.Fov;
            GameWindow.RenderDistanceSlider.Value = userSettings.RenderDistance;
            GameWindow.SensitivitySlider.Value = userSettings.MouseSpeed;
        }

        public void ResetMousePosition()
        {
            if (NeedsToResetMouse)
                MouseController.MoveMouse(GameWindow.CenterPosition);
        }

        public void OpenClosePauseMenu()
        {
            if (!IsInMainMenu)
            {
                IsPauseMenuOpened = !IsPauseMenuOpened;

                GameWindow.PauseMenu.Visibility = GameWindow.PauseMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                GameWindow.HotbarGrid.Visibility = GameWindow.HotbarGrid.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

                PauseGame();
            }
        }
        public void OpenCloseSettingsMenu()
        {
            IsSettingsMenuOpened = !IsSettingsMenuOpened;

            if (!IsInMainMenu)
                GameWindow.PauseMenu.Visibility = GameWindow.PauseMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            else
            {
                GameWindow.MainMenu.Visibility = GameWindow.MainMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                GameWindow.OverWorldCover.Visibility = GameWindow.OverWorldCover.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }

            GameWindow.SettingsMenu.Visibility = GameWindow.SettingsMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
        public void OpenCloseMainMenu()
        {
            IsInMainMenu = !IsInMainMenu;
            GameWindow.HotbarGrid.Visibility = Visibility.Hidden;
            GameWindow.MainMenu.Visibility = GameWindow.MainMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            GameWindow.Crosshair.Visibility = GameWindow.Crosshair.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
        public void OpenCloseWorldSelectorMenu()
        {
            if (IsInMainMenu)
            {
                GameWindow.OverWorldCover.Visibility = GameWindow.OverWorldCover.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                GameWindow.MainMenu.Visibility = GameWindow.MainMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }

            GameWindow.WorldSelectorMenu.Visibility = GameWindow.WorldSelectorMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
        public void OpenCloseWorldCreationMenu()
        {
            if (IsInMainMenu)
                GameWindow.WorldSelectorMenu.Visibility = GameWindow.WorldSelectorMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            else
                GameWindow.OverWorldCover.Visibility = GameWindow.OverWorldCover.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

            GameWindow.WorldCreatorMenu.Visibility = GameWindow.WorldCreatorMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
       
    }
}
