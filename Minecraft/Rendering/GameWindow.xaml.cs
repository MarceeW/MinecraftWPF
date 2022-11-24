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
using System.Runtime.CompilerServices;

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

        internal Renderer renderer;

        internal PickedItem? pickedItem;
        internal GameController gameController;
        internal UserSettings userSettings;
        internal BitmapImage currentTexture;

        GameWindowViewModel vm;
        internal UILogic logic;
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

            float hudScale = 0.6f;

            MouseListener = new MouseListener(this);
            Inventory = new Inventory();
            Hotbar = Ioc.Default.GetService<IHotbar>();

            HotbarGrid.Width *= hudScale;
            HotbarGrid.Height *= hudScale;

            logic.LoadSettingsIntoControls();
            logic.SetupBindings();

            logic.CreateHotbar();
            CreateInventory();
            SetupHotbar();
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
                                logic.OpenCloseInventory();
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
                                    logic.ResetMousePosition();
                                }
                                MouseController.ShowMouse();
                            }
                            break;
                        case Key.Escape:
                            {

                                if (IsInventoryOpened)
                                {
                                    logic.OpenCloseInventory();
                                }
                                else if (IsSettingsMenuOpened)
                                {
                                    logic.OpenCloseSettingsMenu();
                                }
                                else
                                {
                                    logic.OpenClosePauseMenu();
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
        internal void PauseGame()
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
                logic.OpenClosePauseMenu();
            }
            else if (e.Source == Settings || e.Source == SettingsMenuBack || e.Source == MainMenuSettings)
            {
                logic.OpenCloseSettingsMenu();
            }
            else if (e.Source == SaveAndExit)
            {
                gameController.Dispose();
                logic.ReadWorlds();
                logic.OpenClosePauseMenu();
                logic.OpenCloseMainMenu();
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
                    logic.ReloadTextures();
                }

            }
            else if (e.Source == Create)
            {
                logic.CreateWorld(WorldName.Text, WorldSeed.Text);
            }
            else if (e.Source == WorldDetailsCancel || e.Source == CreateNewWorld)
            {
                logic.OpenCloseWorldCreationMenu();
            }
            else if (e.Source == Play || e.Source == BackToMainMenu)
            {
                logic.OpenCloseWorldSelectorMenu();
                WorldSelector.Items.Refresh();
            }
            else if (e.Source == ExitGame)
            {
                Close();
            }
            else if (e.Source == EnterSelectedWorld && WorldSelector.SelectedIndex >= 0)
            {
                logic.EnterWorld(new GameSession(WorldSelector.SelectedItem as WorldData, false));
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
        
        private void WorldSelector_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (WorldSelector.SelectedIndex >= 0)
                logic.EnterWorld(new GameSession(WorldSelector.SelectedItem as WorldData, false));
        }
    }
}