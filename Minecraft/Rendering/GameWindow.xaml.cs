using Minecraft.Controller;
using Minecraft.Render;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using System;
using OpenTK.Windowing.Common;
using System.Windows;
using System.Windows.Forms;
using Minecraft.UI;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Minecraft.Terrain;
using OpenTK.Graphics.OpenGL;
using System.Windows.Media;

using ToolTip = System.Windows.Controls.ToolTip;
using System.Windows.Media.Effects;
using Cursors = System.Windows.Input.Cursors;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Graphics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Minecraft.Rendering;

namespace Minecraft
{
    public delegate void MouseInputHandler(MouseMoveEventArgs mouseMoveEventArgs);
    public delegate void GameUpdateHandler();
    partial class GameWindow : Window
    {
        public Vector2 CenterPosition;
        public event Action? RenderSizeChange;
        public event Action<bool>? Pause;
        public IHotbar Hotbar { get; private set; }
        public Inventory Inventory { get; private set; }

        public bool NeedsToResetMouse = true;
        public bool ShowWireFrames = false;

        public bool IsInventoryOpened = false;
        public bool IsInMainMenu = true;
        public bool IsGamePaused = false;
        public bool IsSettingsMenuOpened = false;
        public bool IsPauseMenuOpened = false;
        public MouseListener MouseListener { get; private set; }

        private Renderer renderer;

        private PickedItem? pickedItem;
        private GameController gameController;
        private UserSettings userSettings;
        private BitmapImage currentTexture;

        GameWindowViewModel vm;
        UILogic logic;
        public GameWindow()
        {
            InitializeComponent();
            logic = new UILogic(this);
            vm = new GameWindowViewModel(logic);

            Title = "Minecraft";
            WindowState = System.Windows.WindowState.Maximized;
            WindowStyle = WindowStyle.None;

            var settings = new GLWpfControlSettings
            {
                MajorVersion = 3,
                MinorVersion = 1,
            };
            OpenTkControl.Start(settings);

            currentTexture = (BitmapImage)Resources["BlockAtlas"];
            var resolution = Screen.PrimaryScreen.Bounds;

            Left = resolution.Width / 2 - Width / 2;
            Top = resolution.Height / 2 - Height / 2;

            CenterPosition = new Vector2(resolution.Width / 2, resolution.Height / 2);

            userSettings = new UserSettings();

            //logic = new UILogic(this);
            //vm = new GameWindowViewModel(logic);

            //logic.ReadWorlds();

            float hudScale = 0.6f;

            MouseListener = new MouseListener(this);
            Inventory = new Inventory();
            Hotbar = Ioc.Default.GetService<IHotbar>();

            HotbarGrid.Width *= hudScale;
            HotbarGrid.Height *= hudScale;

            LoadSettingsIntoControls();
            SetupBindings();

            CreateHotbar();
            CreateInventory();
            SetupHotbar();
        }

        private void EnterWorld(GameSession session)
        {
            renderer = new Renderer();
            MouseController.HideMouse();

            if (IsInMainMenu)
            {
                IsInMainMenu = false;
                OpenCloseWorldSelectorMenu();
                OverWorldCover.Visibility = Visibility.Hidden;
            }

            Crosshair.Visibility = Visibility.Visible;
            gameController = new GameController((int)RenderDistanceSlider.Value, renderer, this, session);
            UpdateHotbarItems();
            SetupBindings();
            OpenTkControl.Focus();
        }
        private void UpdateHotbarItems()
        {
            if (Hotbar != null)
            {
                foreach (var img in HotbarGrid.Children)
                {
                    if (img is Image i)
                    {
                        var data = i.Name.Split('_');
                        if (data[0] == "HotbarItem" && Hotbar.Items[int.Parse(data[1])] != BlockType.Air)
                        {
                            i.Source = new CroppedBitmap(currentTexture, AtlasTexturesData.GetTextureRect(Hotbar.Items[int.Parse(data[1])]));
                        }
                        else if (data[0] == "HotbarItem")
                            i.Source = null;
                    }
                }
            }
            HotbarGrid.Visibility = Visibility.Visible;
        }
        private void CreateWorld(string name, string seed)
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

            IsInMainMenu = false;
            WorldName.Text = "";
            WorldSeed.Text = "";
            OpenCloseWorldCreationMenu();
            EnterWorld(new GameSession(worldData, true));
        }
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (pickedItem != null)
            {
                PickedItemCanvas.Margin = new Thickness(e.GetPosition(null).X - PickedItemImage.Width - 10, e.GetPosition(null).Y, 0, 0);
            }
            base.OnMouseMove(e);
        }
        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!IsInventoryOpened)
            {
                if (Hotbar != null && e.MiddleButton == MouseButtonState.Pressed && WorldRenderer.CurrentTarget != null && (BlockType)WorldRenderer.CurrentTarget != BlockType.Air)
                {
                    Hotbar.ChangeBlock(Hotbar.SelectedItemIndex, (BlockType)WorldRenderer.CurrentTarget);

                    var hotbarImage = (Image)HotbarGrid.FindName($"HotbarItem_{Hotbar.SelectedItemIndex}");
                    hotbarImage.Source = new CroppedBitmap(currentTexture, AtlasTexturesData.GetTextureRect((BlockType)WorldRenderer.CurrentTarget));
                }
            }
            base.OnMouseDown(e);
        }
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Source == OpenTkControl)
            {
                if ((int)e.Key >= 35 && (int)e.Key <= 43) //34 = 0
                {
                    Hotbar.SetSelectedIndex((int)e.Key - 35);
                    Grid.SetColumn((Image)HotbarGrid.FindName("SelectedFrame"), Hotbar.SelectedItemIndex);
                }
                else
                {
                    switch (e.Key)
                    {
                        case Key.F3:
                            {
                                fpsCounter.Visibility = fpsCounter.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
                            }
                            break;
                        case Key.E:
                            {
                                OpenCloseInventory();
                            }
                            break;
                        case Key.G:
                            {
                                ShowWireFrames = !ShowWireFrames;
                                e.Handled = true;
                            }
                            break;
                        case Key.P:
                            {
                                NeedsToResetMouse = !NeedsToResetMouse;

                                if (NeedsToResetMouse)
                                    MouseController.HideMouse();
                                else
                                {
                                    ResetMousePosition();
                                }
                                MouseController.ShowMouse();
                            }
                            break;
                        case Key.Escape:
                            {

                                if (IsInventoryOpened)
                                {
                                    OpenCloseInventory();
                                }
                                else if (IsSettingsMenuOpened)
                                {
                                    OpenCloseSettingsMenu();
                                }
                                else
                                {
                                    OpenClosePauseMenu();
                                }
                            }
                            break;
                    }
                }
            }

            base.OnKeyDown(e);
        }
        protected override void OnMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        {
            if (Hotbar != null)
            {
                Hotbar.UpdateSelectedIndex(e.Delta);

                Grid.SetColumn((Image)HotbarGrid.FindName("SelectedFrame"), Hotbar.SelectedItemIndex);
            }

            base.OnMouseWheel(e);
        }
        protected override void OnLocationChanged(EventArgs e)
        {
            CenterPosition = new Vector2((float)(Left + Width / 2), (float)(Top + Height / 2));
            base.OnLocationChanged(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            userSettings.Save((float)FovSlider.Value, (int)RenderDistanceSlider.Value, (float)SensitivitySlider.Value);
            Environment.Exit(0);
            base.OnClosed(e);
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            CenterPosition = new Vector2((float)(Left + Width / 2), (float)(Top + Height / 2));

            RenderSizeChange?.Invoke();
        }
        private void PauseGame()
        {
            IsGamePaused = !IsGamePaused;

            Pause?.Invoke(IsGamePaused);

            if (IsGamePaused)
            {
                var effect = new BlurEffect();
                effect.Radius = 15;

                OpenTkControl.Effect = effect;
            }
            else
                OpenTkControl.Effect = null;

            PauseMenuDarkener.Visibility = PauseMenuDarkener.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            Crosshair.Visibility = Crosshair.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

            NeedsToResetMouse = !NeedsToResetMouse;

            if (NeedsToResetMouse)
                MouseController.HideMouse();
            else
            {
                MouseController.ShowMouse();
            }
        }
        private void OpenCloseInventory()
        {
            if (!IsPauseMenuOpened && !IsInMainMenu)
            {
                pickedItem = null;
                PickedItemImage.Source = null;

                IsInventoryOpened = !IsInventoryOpened;

                InventoryGrid.Visibility = InventoryGrid.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                InventoryText.Visibility = InventoryText.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

                PauseGame();
            }
        }
        private void CreateInventory()
        {
            double inventoryFrameSize = InventoryGrid.Width / Inventory.Columns;
            InventoryGrid.Height = inventoryFrameSize * Inventory.Rows;

            for (int i = 0; i < Inventory.Rows; i++)
            {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(1, GridUnitType.Star);

                InventoryGrid.RowDefinitions.Add(rowDef);
            }
            for (int i = 0; i < Inventory.Columns; i++)
            {
                ColumnDefinition colDef = new ColumnDefinition();
                colDef.Width = new GridLength(1, GridUnitType.Star);

                InventoryGrid.ColumnDefinitions.Add(colDef);
            }

            for (int x = 0; x < Inventory.Columns; x++)
            {
                for (int y = 0; y < Inventory.Rows; y++)
                {
                    Image frame = new Image();
                    frame.Source = (BitmapSource)Resources["InventoryFrame"];
                    frame.Name = $"InventoryItemFrame_{x}_{y}";

                    RenderOptions.SetBitmapScalingMode(frame, BitmapScalingMode.NearestNeighbor);

                    Grid.SetRow(frame, y);
                    Grid.SetColumn(frame, x);

                    InventoryGrid.Children.Add(frame);
                    InventoryGrid.RegisterName(frame.Name, frame);
                }
            }

            if (Inventory != null)
            {
                for (int x = 0; x < Inventory.Columns; x++)
                {
                    for (int y = 0; y < Inventory.Rows; y++)
                    {
                        if (Inventory.Blocks[y, x] == BlockType.Air)
                            continue;

                        Image item = new Image();
                        item.Name = $"InventoryItem_{x}_{y}";
                        item.Width = 48;

                        item.MouseEnter += OnMouseEnterBlockImage;
                        item.MouseLeave += OnMouseLeaveBlockImage;
                        item.MouseDown += InventoryItemMouseDown;

                        var tooltip = new ToolTip();
                        tooltip.Height = 30;
                        tooltip.FontSize = 16;
                        tooltip.Style = (Style)Resources["ItemToolTip"];
                        tooltip.Content = BlockData.GetBlockName(Inventory.Blocks[y, x]);
                        tooltip.Visibility = Visibility.Visible;

                        item.ToolTip = tooltip;
                        item.Source = new CroppedBitmap((BitmapSource)Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect(Inventory.Blocks[y, x]));

                        RenderOptions.SetBitmapScalingMode(item, BitmapScalingMode.NearestNeighbor);

                        Grid.SetRow(item, y);
                        Grid.SetColumn(item, x);

                        InventoryGrid.Children.Add(item);
                        InventoryGrid.RegisterName(item.Name, item);
                    }
                }
            }
        }
        private void ReloadTextures()
        {
            foreach (var img in InventoryGrid.Children)
            {
                if (img is Image i)
                {
                    var data = i.Name.Split('_');
                    if (data[0] == "InventoryItem")
                    {
                        i.Source = new CroppedBitmap(currentTexture, AtlasTexturesData.GetTextureRect(Inventory.Blocks[int.Parse(data[2]), int.Parse(data[1])]));
                    }
                }
            }
            foreach (var img in HotbarGrid.Children)
            {
                if (img is Image i)
                {
                    var data = i.Name.Split('_');
                    if (data[0] == "HotbarItem" && Hotbar.Items[int.Parse(data[1])] != BlockType.Air)
                    {
                        i.Source = new CroppedBitmap(currentTexture, AtlasTexturesData.GetTextureRect(Hotbar.Items[int.Parse(data[1])]));
                    }
                }
            }
        }
        private void CreateHotbar()
        {
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(1, GridUnitType.Star);
            HotbarGrid.RowDefinitions.Add(row);

            for (int i = 0; i < Hotbar.MaxItems; i++)
            {
                ColumnDefinition def = new ColumnDefinition();
                def.Width = new GridLength(1, GridUnitType.Star);
                HotbarGrid.ColumnDefinitions.Add(def);
            }

            for (int i = 0; i < Hotbar.MaxItems; i++)
            {
                Image frame = new Image();
                frame.Source = (BitmapSource)Resources["ItemFrame"];
                frame.Name = "ItemFrame" + i;

                RenderOptions.SetBitmapScalingMode(frame, BitmapScalingMode.NearestNeighbor);

                Grid.SetRow(frame, 0);
                Grid.SetColumn(frame, i);

                HotbarGrid.Children.Add(frame);
                HotbarGrid.RegisterName(frame.Name, frame);
            }

            Image selectedFrame = new Image();
            selectedFrame.Source = (BitmapSource)Resources["SelectedItemFrame"];
            selectedFrame.Name = "SelectedFrame";

            RenderOptions.SetBitmapScalingMode(selectedFrame, BitmapScalingMode.NearestNeighbor);

            Grid.SetRow(selectedFrame, 0);
            Grid.SetColumn(selectedFrame, 0);

            HotbarGrid.Children.Add(selectedFrame);
            HotbarGrid.RegisterName(selectedFrame.Name, selectedFrame);
        }
        private void SetupHotbar()
        {
            if (Hotbar != null)
            {
                for (int i = 0; i < Hotbar.MaxItems; i++)
                {

                    Image item = new Image();
                    item.Name = "HotbarItem_" + i;
                    item.Width = 36;

                    item.MouseEnter += OnMouseEnterBlockImage;
                    item.MouseLeave += OnMouseLeaveBlockImage;

                    if (Hotbar.Items[i] != BlockType.Air)
                        item.Source = new CroppedBitmap((BitmapSource)Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect(Hotbar.Items[i]));

                    RenderOptions.SetBitmapScalingMode(item, BitmapScalingMode.NearestNeighbor);

                    Grid.SetRow(item, 0);
                    Grid.SetColumn(item, i);

                    HotbarGrid.Children.Add(item);
                    HotbarGrid.RegisterName(item.Name, item);
                }
            }
        }
        private void SetupBindings()
        {
            if (FovSlider.DataContext == null)
            {
                FovSlider.DataContext = Ioc.Default.GetService<ICamera>();
            }
            if (gameController != null)
            {
                RenderDistanceSlider.DataContext = gameController.WorldRendererer;
                SensitivitySlider.DataContext = gameController.PlayerController;

                gameController.InitUserSettings(new UserSettings((float)FovSlider.Value, (int)RenderDistanceSlider.Value, (float)SensitivitySlider.Value));
            }
        }
        private void LoadSettingsIntoControls()
        {
            FovSlider.Value = userSettings.Fov;
            RenderDistanceSlider.Value = userSettings.RenderDistance;
            SensitivitySlider.Value = userSettings.MouseSpeed;
        }

        public void ResetMousePosition()
        {
            if (NeedsToResetMouse)
            {
                MouseController.MoveMouse(CenterPosition);
            }
        }
        private void OpenTkControl_OnRender(TimeSpan delta)
        {
            if (!IsInMainMenu)
            {
                if (ShowWireFrames)
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                else
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

                renderer.RenderFrame(delta.Milliseconds / 1000.0f);

                fpsCounter.Text = Math.Round(1.0 / delta.TotalSeconds, 0) + " Fps";
            }
        }
        private void OnMouseEnterBlockImage(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as Image).Width *= 1.2;
            Cursor = Cursors.Hand;
        }
        private void OnMouseLeaveBlockImage(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as Image).Width /= 1.2;
            Cursor = Cursors.Arrow;
        }
        private void InventoryItemMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var item = sender as Image;
                var itemData = item.Name.Split('_');

                PickedItemImage.Source = item.Source;

                pickedItem = new PickedItem(item.Source.Clone(), Inventory.Blocks[int.Parse(itemData[2]), int.Parse(itemData[1])]);
            }
            else
            {
                PickedItemImage.Source = null;
                pickedItem = null;
            }
        }
        private void HotbarMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Hotbar != null)
            {
                var grid = sender as Grid;

                var pos = e.GetPosition(grid);

                pos.X /= grid.Width / Hotbar.MaxItems;

                var clickedImage = (Image)HotbarGrid.FindName($"HotbarItem_{(int)pos.X}");

                if (pickedItem != null)
                {
                    BlockType toChange = pickedItem.type;

                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        pickedItem.src = clickedImage.Source?.Clone();
                        pickedItem.type = Hotbar.Items[(int)pos.X];

                        clickedImage.Source = PickedItemImage.Source;

                        PickedItemImage.Source = pickedItem.src;
                    }
                    else
                    {
                        clickedImage.Source = null;
                        toChange = BlockType.Air;
                    }

                    Hotbar.ChangeBlock((int)pos.X, toChange);
                }
                else if (pickedItem == null)
                {
                    pickedItem = new PickedItem((CroppedBitmap)clickedImage.Source, Hotbar.Items[(int)pos.X]);

                    clickedImage.Source = null;
                    PickedItemImage.Source = pickedItem.src;
                    Hotbar.ChangeBlock((int)pos.X, BlockType.Air);

                    PickedItemCanvas.Margin = new Thickness(e.GetPosition(null).X - PickedItemImage.Width - 10, e.GetPosition(null).Y, 0, 0);
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    clickedImage.Source = null;
                    Hotbar.ChangeBlock((int)pos.X, BlockType.Air);
                }
            }
        }

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var img = e.Source as Image;
            img.Source = (BitmapSource)Resources["MenuButtonSelectedFrame"];
        }

        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var img = e.Source as Image;
            img.Source = (BitmapSource)Resources["MenuButtonFrame"];
        }

        private void Button_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.Source == BackToGame)
            {
                OpenClosePauseMenu();
            }
            else if (e.Source == Settings || e.Source == SettingsMenuBack || e.Source == MainMenuSettings)
            {
                OpenCloseSettingsMenu();
            }
            else if (e.Source == SaveAndExit)
            {
                gameController.Dispose();
                //logic.ReadWorlds();
                OpenClosePauseMenu();
                OpenCloseMainMenu();
                MouseController.ShowMouse();
            }
            else if (e.Source == LoadTexture)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Png images (*.png;)|*.png;";

                var result = openFileDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    AtlasTexturesData.TexturePath = openFileDialog.FileName;
                    currentTexture = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.RelativeOrAbsolute));
                    ReloadTextures();
                }

            }
            else if (e.Source == Create)
            {
                CreateWorld(WorldName.Text, WorldSeed.Text);
            }
            else if (e.Source == WorldDetailsCancel || e.Source == CreateNewWorld)
            {
                OpenCloseWorldCreationMenu();
            }
            else if (e.Source == Play || e.Source == BackToMainMenu)
            {
                OpenCloseWorldSelectorMenu();
                WorldSelector.Items.Refresh();
            }
            else if (e.Source == ExitGame)
            {
                Close();
            }
            else if (e.Source == EnterSelectedWorld && WorldSelector.SelectedIndex >= 0)
            {
                EnterWorld(new GameSession(WorldSelector.SelectedItem as WorldData, false));
            }
            else if (e.Source == DeleteSelectedWorld)
            {
                if (WorldSelector.SelectedIndex >= 0)
                {
                    var world = WorldSelector.SelectedItem as WorldData;

                    if (world != null)
                    {
                        Directory.Delete(world.WorldPath, true);
                        (WorldSelector.ItemsSource as List<WorldData>)?.Remove(world);
                        WorldSelector.Items.Refresh();
                    }
                }
            }
        }
        private void OpenClosePauseMenu()
        {
            if (!IsInMainMenu)
            {
                IsPauseMenuOpened = !IsPauseMenuOpened;

                PauseMenu.Visibility = PauseMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                HotbarGrid.Visibility = HotbarGrid.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

                PauseGame();
            }
        }
        private void OpenCloseSettingsMenu()
        {
            IsSettingsMenuOpened = !IsSettingsMenuOpened;

            if (!IsInMainMenu)
                PauseMenu.Visibility = PauseMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            else
            {
                MainMenu.Visibility = MainMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                OverWorldCover.Visibility = OverWorldCover.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }

            SettingsMenu.Visibility = SettingsMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
        private void OpenCloseMainMenu()
        {
            IsInMainMenu = !IsInMainMenu;
            HotbarGrid.Visibility = Visibility.Hidden;
            MainMenu.Visibility = MainMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            Crosshair.Visibility = Crosshair.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
        private void OpenCloseWorldSelectorMenu()
        {
            if (IsInMainMenu)
            {
                OverWorldCover.Visibility = OverWorldCover.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                MainMenu.Visibility = MainMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }

            WorldSelectorMenu.Visibility = WorldSelectorMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
        private void OpenCloseWorldCreationMenu()
        {
            if (IsInMainMenu)
                WorldSelectorMenu.Visibility = WorldSelectorMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            else
                OverWorldCover.Visibility = OverWorldCover.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

            WorldCreatorMenu.Visibility = WorldCreatorMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
        private void WorldSelector_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (WorldSelector.SelectedIndex >= 0)
                EnterWorld(new GameSession(WorldSelector.SelectedItem as WorldData, false));
        }
    }
}