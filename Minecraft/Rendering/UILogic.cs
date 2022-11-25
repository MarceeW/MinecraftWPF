using Minecraft.Controller;
using Minecraft.Render;
using Minecraft.Terrain;
using Minecraft.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Image = System.Windows.Controls.Image;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Media;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Vector2 = OpenTK.Mathematics.Vector2;

namespace Minecraft.Rendering
{
    internal class UILogic : IUILogic
    {
        internal GameWindow gw;
        public UILogic(GameWindow gw)
        {
            this.gw = gw;
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
                        savesData.Add(new WorldData() { WorldName = worldData[0], WorldSeed = int.Parse(worldData[1]), LastPlayed = date, WorldPath = path });
                    else
                        savesData.Add(new WorldData() { WorldName = worldData[0], WorldSeed = int.Parse(worldData[1]), LastPlayed = DateTime.Now, WorldPath = path });
                    
                }
            }
            gw.WorldSelector.ItemsSource = savesData.OrderByDescending(x => x.LastPlayed).ToList();
        }

        public void EnterWorld(GameSession session)
        {
            gw.renderer = new Renderer();
            MouseController.HideMouse();

            if (gw.IsInMainMenu)
            {
                gw.IsInMainMenu = false;
                OpenCloseWorldSelectorMenu();
                gw.OverWorldCover.Visibility = Visibility.Hidden;
            }

            gw.Crosshair.Visibility = Visibility.Visible;
            gw.gameController = new GameController((int)gw.RenderDistanceSlider.Value, gw.renderer, gw, session);
            gw.invlogic.UpdateHotbarItems();
            SetupBindings();
            gw.OpenTkControl.Focus();
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
            var worldData = new WorldData() { LastPlayed = DateTime.Now, WorldName = name, WorldSeed = WorldGenerator.GenerateSeed(seed), WorldPath = worldSaveDir };

            gw.IsInMainMenu = false;
            gw.WorldName.Text = "";
            gw.WorldSeed.Text = "";
            OpenCloseWorldCreationMenu();
            EnterWorld(new GameSession(worldData, true));
        }
        public void OpenCloseInventory()
        {
            if (!gw.IsPauseMenuOpened && !gw.IsInMainMenu)
            {
                gw.pickedItem = null;
                gw.PickedItemImage.Source = null;

                gw.IsInventoryOpened = !gw.IsInventoryOpened;

                gw.InventoryGrid.Visibility = gw.InventoryGrid.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                gw.InventoryText.Visibility = gw.InventoryText.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

                gw.PauseGame();
            }
        }

     

        public void SetupBindings()
        {
            if (gw.FovSlider.DataContext == null)
                gw.FovSlider.DataContext = Ioc.Default.GetService<ICamera>();
            if (gw.gameController != null)
            {
                gw.RenderDistanceSlider.DataContext = gw.gameController.WorldRendererer;
                gw.SensitivitySlider.DataContext = gw.gameController.PlayerController;

                gw.gameController.InitUserSettings(new UserSettings((float)gw.FovSlider.Value, (int)gw.RenderDistanceSlider.Value, (float)gw.SensitivitySlider.Value));
            }
        }

        public void LoadSettingsIntoControls()
        {
            gw.FovSlider.Value = gw.userSettings.Fov;
            gw.RenderDistanceSlider.Value = gw.userSettings.RenderDistance;
            gw.SensitivitySlider.Value = gw.userSettings.MouseSpeed;
        }

        public void ResetMousePosition()
        {
            if (gw.NeedsToResetMouse)
                MouseController.MoveMouse(gw.CenterPosition);   
        }
        
        public void OpenClosePauseMenu()
        {
            if (!gw.IsInMainMenu)
            {
                gw.IsPauseMenuOpened = !gw.IsPauseMenuOpened;

                gw.PauseMenu.Visibility = gw.PauseMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                gw.HotbarGrid.Visibility = gw.HotbarGrid.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

                gw.PauseGame();
            }
        }
        public void OpenCloseSettingsMenu()
        {
            gw.IsSettingsMenuOpened = !gw.IsSettingsMenuOpened;

            if (!gw.IsInMainMenu)
                gw.PauseMenu.Visibility = gw.PauseMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            else
            {
                gw.MainMenu.Visibility = gw.MainMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                gw.OverWorldCover.Visibility = gw.OverWorldCover.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }

            gw.SettingsMenu.Visibility = gw.SettingsMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
        public void OpenCloseMainMenu()
        {
            gw.IsInMainMenu = !gw.IsInMainMenu;
            gw.HotbarGrid.Visibility = Visibility.Hidden;
            gw.MainMenu.Visibility = gw.MainMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            gw.Crosshair.Visibility = gw.Crosshair.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
        public void OpenCloseWorldSelectorMenu()
        {
            if (gw.IsInMainMenu)
            {
                gw.OverWorldCover.Visibility = gw.OverWorldCover.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                gw.MainMenu.Visibility = gw.MainMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }

            gw.WorldSelectorMenu.Visibility = gw.WorldSelectorMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
        public void OpenCloseWorldCreationMenu()
        {
            if (gw.IsInMainMenu)
                gw.WorldSelectorMenu.Visibility = gw.WorldSelectorMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            else
                gw.OverWorldCover.Visibility = gw.OverWorldCover.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

            gw.WorldCreatorMenu.Visibility = gw.WorldCreatorMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
