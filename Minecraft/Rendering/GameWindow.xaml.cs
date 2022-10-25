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
using System.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace Minecraft
{
    public delegate void MouseInputHandler(MouseMoveEventArgs mouseMoveEventArgs);
    public delegate void GameUpdateHandler();
    partial class GameWindow : Window
    {
        class PickedItem
        {
            public CroppedBitmap src;
            public BlockType type;

            public PickedItem(CroppedBitmap src, BlockType type)
            {
                this.src = src;
                this.type = type;
            }
        }

        public Vector2 CenterPosition;
        public event Action<float>? RenderSizeChange;
        public event Action<bool>? Pause;
        public IHotbar Hotbar { get; }
        public Inventory Inventory { get; set; } = new Inventory();

        public bool PauseMenuOpened { get; private set; } = false;

        public bool IsInventoryOpened = false;
        public bool NeedsToResetMouse = true;
        public bool ShowWireFrames = false;
        public MouseListener MouseListener { get; }

        private Renderer renderer;

        private PickedItem? pickedItem;
        private GameController gameController;
        public GameWindow()
        {
            InitializeComponent();

            Title = "Minecraft";
            WindowState = System.Windows.WindowState.Normal;
            WindowStyle = WindowStyle.None;

            var settings = new GLWpfControlSettings
            {
                MajorVersion = 3,
                MinorVersion = 1,
            };
            OpenTkControl.Start(settings);

            MouseListener = new MouseListener(this);
            renderer = new Renderer();
            gameController = new GameController(renderer, this);

            var resolution = Screen.PrimaryScreen.Bounds;

            Left = resolution.Width / 2 - Width / 2;
            Top = resolution.Height / 2 - Height / 2;

            CenterPosition = new Vector2(resolution.Width / 2, resolution.Height / 2);

            float hudScale = 0.6f;

            Hotbar = Ioc.Default.GetService<IHotbar>();

            HotbarGrid.Width *= hudScale;
            HotbarGrid.Height *= hudScale;

            MouseController.HideMouse();

            CreateHotbar();
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
                if (Hotbar != null && e.MiddleButton == MouseButtonState.Pressed && WorldRenderer.CurrentTarget != null)
                {
                    Hotbar.ChangeBlock(Hotbar.SelectedItemIndex, (BlockType)WorldRenderer.CurrentTarget);

                    var hotbarImage = (Image)HotbarGrid.FindName($"HotbarItem_{Hotbar.SelectedItemIndex}");
                    hotbarImage.Source = new CroppedBitmap((BitmapSource)Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect((BlockType)WorldRenderer.CurrentTarget));
                }
            }
            base.OnMouseDown(e);
        }
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if(e.Source == OpenTkControl)
            {
                switch (e.Key)
                {
                    case Key.E:
                        {
                            OpenCloseInventory();
                        }
                    break;
                    case Key.T:
                        {
                            if (!IsInventoryOpened)
                            {
                                CommandLine.Focusable = true;

                                CommandLine.Visibility = Visibility.Visible;
                                CommandLine.Focus();

                                PlayerController.CanMove = false;
                            }                 
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

                            if(NeedsToResetMouse)
                                MouseController.HideMouse();
                            else
                                MouseController.ShowMouse();
                        }
                        break;
                    case Key.Escape:
                        {
                            
                            if (CommandLine.Visibility == Visibility.Visible)
                            {
                                CommandLine.Focusable = false;
                                CommandLine.Visibility = Visibility.Hidden;
                                PlayerController.CanMove = true;
                            }
                            else if (IsInventoryOpened)
                            {
                                OpenCloseInventory();
                            }
                            else
                            {
                                Close();
                            }
                        }
                        break;
                }
            }

            base.OnKeyDown(e);
        }
        protected override void OnMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        {
            if(Hotbar != null)
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
            Environment.Exit(0);
            base.OnClosed(e);
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            CenterPosition = new Vector2((float)(Left + Width / 2), (float)(Top + Height / 2));

            RenderSizeChange?.Invoke((float)OpenTkControl.FrameBufferWidth / OpenTkControl.FrameBufferHeight);
        }
        private void PauseGame()
        {
            PauseMenuOpened = !PauseMenuOpened;

            Pause?.Invoke(PauseMenuOpened);

            if (PauseMenuOpened)
            {
                var effect = new BlurEffect();
                effect.Radius = 15;

                OpenTkControl.Effect = effect;
            }
            else
                OpenTkControl.Effect = null;

            PauseMenuDarkener.Visibility = PauseMenuDarkener.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

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
            pickedItem = null;
            PickedItemImage.Source = null;

            IsInventoryOpened = !IsInventoryOpened;

            InventoryData.Visibility = InventoryData.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            InventoryGrid.Visibility = InventoryGrid.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            InventoryText.Visibility = InventoryText.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

            PauseGame();
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

                        var tooltip = new ToolTip();
                        tooltip.Content = Inventory.Blocks[y, x];
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
                    if (Hotbar.Items[i] == BlockType.Air)
                        continue;

                    Image item = new Image();
                    item.Name = "HotbarItem_" + i;
                    item.Width = 40;

                    item.MouseEnter += OnMouseEnterBlockImage;
                    item.MouseLeave += OnMouseLeaveBlockImage;

                    item.Source = new CroppedBitmap((BitmapSource)Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect(Hotbar.Items[i]));

                    RenderOptions.SetBitmapScalingMode(item, BitmapScalingMode.NearestNeighbor);

                    Grid.SetRow(item, 0);
                    Grid.SetColumn(item, i);

                    HotbarGrid.Children.Add(item);
                    HotbarGrid.RegisterName(item.Name, item);
                }
            }
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
            if (ShowWireFrames)
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            else
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            renderer.RenderFrame(delta.Milliseconds / 1000.0f);

            fpsCounter.Content = "FPS:\t" + Math.Round(1.0 / delta.TotalSeconds, 0);                
        }
        private void OnMouseEnterBlockImage(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }
        private void OnMouseLeaveBlockImage(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }
        private void InventoryMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                var grid = sender as Grid;

                var pos = e.GetPosition(grid);

                pos.X /= grid.Width / Inventory.Columns;
                pos.Y /= grid.Height / Inventory.Rows;

                var blockType = Inventory.Blocks[(int)pos.Y, (int)pos.X];

                if(blockType != BlockType.Air)
                {
                    pickedItem = new PickedItem(new CroppedBitmap((BitmapSource)Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect(blockType)), blockType);

                    PickedItemImage.Source = pickedItem.src;
                }

                PickedItemCanvas.Margin = new Thickness(e.GetPosition(null).X - PickedItemImage.Width - 10, e.GetPosition(null).Y, 0, 0);
            }
            else
            {
                PickedItemImage.Source = null;
                pickedItem = null;
            }
        }
        private void HotbarGridMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(Hotbar != null)
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
                        pickedItem.src = (CroppedBitmap)clickedImage.Source;
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
                else if(pickedItem == null)
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
    }
}