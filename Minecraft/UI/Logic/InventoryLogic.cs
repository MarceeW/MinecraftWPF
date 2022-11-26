using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Controller;
using Minecraft.Render;
using Minecraft.Terrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Minecraft.UI.Logic
{
    public class InventoryLogic : IInventoryLogic
    {
        public Inventory Inventory { get; private set; }
        public IHotbar Hotbar { get; private set; }

        public GameWindow GameWindow
        {
            get
            {
                return gameWindow;
            }
            set
            {
                gameWindow = value;
                gameWindow.PreviewMouseDown += OnMouseDown;
                gameWindow.PreviewKeyDown += OnKeyDown;
                gameWindow.PreviewMouseWheel += OnMouseWheel;
                gameWindow.MouseMove += OnMouseMove;

                gameWindow.HotbarGrid.PreviewMouseDown += OnHotbarMouseDown;

                CreateHotbar();
                CreateInventory();
                SetupHotbar();
            }
        }
        internal PickedItem? pickedItem;
        private GameWindow gameWindow;

        public bool IsOpened { get; private set; }

        public InventoryLogic()
        {
            Inventory = new Inventory();
            Hotbar = Ioc.Default.GetService<IHotbar>();
        }

        public void CreateInventory()
        {
            double inventoryFrameSize = gameWindow.InventoryGrid.Width / Inventory.Columns;
            gameWindow.InventoryGrid.Height = inventoryFrameSize * Inventory.Rows;

            for (int i = 0; i < Inventory.Rows; i++)
            {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(1, GridUnitType.Star);

                gameWindow.InventoryGrid.RowDefinitions.Add(rowDef);
            }
            for (int i = 0; i < Inventory.Columns; i++)
            {
                ColumnDefinition colDef = new ColumnDefinition();
                colDef.Width = new GridLength(1, GridUnitType.Star);

                gameWindow.InventoryGrid.ColumnDefinitions.Add(colDef);
            }

            for (int x = 0; x < Inventory.Columns; x++)
            {
                for (int y = 0; y < Inventory.Rows; y++)
                {
                    Image frame = new Image();
                    frame.Source = (BitmapSource)gameWindow.Resources["InventoryFrame"];
                    frame.Name = $"InventoryItemFrame_{x}_{y}";

                    RenderOptions.SetBitmapScalingMode(frame, BitmapScalingMode.NearestNeighbor);

                    Grid.SetRow(frame, y);
                    Grid.SetColumn(frame, x);

                    gameWindow.InventoryGrid.Children.Add(frame);
                    gameWindow.InventoryGrid.RegisterName(frame.Name, frame);
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
                        item.MouseDown += OnInventoryItemMouseDown;

                        var tooltip = new ToolTip();
                        tooltip.Height = 30;
                        tooltip.FontSize = 16;
                        tooltip.Style = (Style)gameWindow.Resources["ItemToolTip"];
                        tooltip.Content = BlockData.GetBlockName(Inventory.Blocks[y, x]);
                        tooltip.Visibility = Visibility.Visible;

                        item.ToolTip = tooltip;
                        item.Source = new CroppedBitmap((BitmapSource)gameWindow.Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect(Inventory.Blocks[y, x]));

                        RenderOptions.SetBitmapScalingMode(item, BitmapScalingMode.NearestNeighbor);

                        Grid.SetRow(item, y);
                        Grid.SetColumn(item, x);

                        gameWindow.InventoryGrid.Children.Add(item);
                        gameWindow.InventoryGrid.RegisterName(item.Name, item);
                    }
                }
            }
        }
        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (pickedItem != null)
                gameWindow.PickedItemCanvas.Margin = new Thickness(e.GetPosition(null).X - gameWindow.PickedItemImage.Width - 10, e.GetPosition(null).Y, 0, 0);
        }

        public void SetupHotbar()
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
                        item.Source = new CroppedBitmap((BitmapSource)gameWindow.Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect(Hotbar.Items[i]));

                    RenderOptions.SetBitmapScalingMode(item, BitmapScalingMode.NearestNeighbor);

                    Grid.SetRow(item, 0);
                    Grid.SetColumn(item, i);

                    gameWindow.HotbarGrid.Children.Add(item);
                    gameWindow.HotbarGrid.RegisterName(item.Name, item);
                }
            }
        }
        public void OpenCloseInventory()
        {
            var uiLogic = Ioc.Default.GetService<IUILogic>();

            if (!uiLogic.IsPauseMenuOpened && !UILogic.IsInMainMenu)
            {
                pickedItem = null;
                GameWindow.PickedItemImage.Source = null;

                uiLogic.IsInventoryOpened = !uiLogic.IsInventoryOpened;

                GameWindow.InventoryGrid.Visibility = GameWindow.InventoryGrid.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                GameWindow.InventoryText.Visibility = GameWindow.InventoryText.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

                uiLogic.PauseGame();
            }
        }
        public void OnMouseWheel(object sender,MouseWheelEventArgs e)
        {
            if (Hotbar != null)
            {
                Hotbar.UpdateSelectedIndex(e.Delta);

                Grid.SetColumn((Image)gameWindow.HotbarGrid.FindName("SelectedFrame"), Hotbar.SelectedItemIndex);
            }
        }
        public void OnInventoryItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var item = sender as Image;
                var itemData = item.Name.Split('_');

                gameWindow.PickedItemImage.Source = item.Source;

                pickedItem = new PickedItem(item.Source.Clone(), Inventory.Blocks[int.Parse(itemData[2]), int.Parse(itemData[1])]);
            }
            else
            {
                gameWindow.PickedItemImage.Source = null;
                pickedItem = null;
            }
        }
        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((int)e.Key >= 35 && (int)e.Key <= 43) //34 = 0
            {
                Hotbar.SetSelectedIndex((int)e.Key - 35);
                Grid.SetColumn((Image)gameWindow.HotbarGrid.FindName("SelectedFrame"), Hotbar.SelectedItemIndex);
            }
            else
            {
                switch (e.Key)
                {
                    case Key.E:
                        {
                            OpenCloseInventory();
                        }
                        break;
                    case Key.G:
                        {
                            gameWindow.ShowWireFrames = !gameWindow.ShowWireFrames;
                            e.Handled = true;
                        }
                        break;
                }
            }
        }
        public void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsOpened)
            {
                gameWindow.PickedItemCanvas.Margin = new Thickness(e.GetPosition(null).X - gameWindow.PickedItemImage.Width - 10, e.GetPosition(null).Y, 0, 0);

                if (Hotbar != null && e.MiddleButton == MouseButtonState.Pressed && WorldRenderer.CurrentTarget != null && (BlockType)WorldRenderer.CurrentTarget != BlockType.Air)
                {
                    Hotbar.ChangeBlock(Hotbar.SelectedItemIndex, (BlockType)WorldRenderer.CurrentTarget);

                    var hotbarImage = (Image)gameWindow.HotbarGrid.FindName($"HotbarItem_{Hotbar.SelectedItemIndex}");
                    hotbarImage.Source = new CroppedBitmap(AtlasTexturesData.CurrentTexture, AtlasTexturesData.GetTextureRect((BlockType)WorldRenderer.CurrentTarget));
                }
            }
        }
        public void OnHotbarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Hotbar != null)
            {
                var grid = sender as Grid;

                var pos = e.GetPosition(grid);

                pos.X /= grid.Width / Hotbar.MaxItems;

                var clickedImage = (Image)gameWindow.FindName($"HotbarItem_{(int)pos.X}");

                if (pickedItem != null)
                {
                    BlockType toChange = pickedItem.type;

                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        pickedItem.src = clickedImage.Source?.Clone();
                        pickedItem.type = Hotbar.Items[(int)pos.X];

                        clickedImage.Source = gameWindow.PickedItemImage.Source;

                        gameWindow.PickedItemImage.Source = pickedItem.src;
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
                    gameWindow.PickedItemImage.Source = pickedItem.src;
                    Hotbar.ChangeBlock((int)pos.X, BlockType.Air);

                    gameWindow.PickedItemCanvas.Margin = new Thickness(e.GetPosition(null).X - gameWindow.PickedItemImage.Width - 10, e.GetPosition(null).Y, 0, 0);
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    clickedImage.Source = null;
                    Hotbar.ChangeBlock((int)pos.X, BlockType.Air);
                }
            }
        }

        public void OnMouseEnterBlockImage(object sender, MouseEventArgs e)
        {
            (sender as Image).Width *= 1.2;
            gameWindow.Cursor = Cursors.Hand;
        }

        public void OnMouseLeaveBlockImage(object sender, MouseEventArgs e)
        {
            (sender as Image).Width /= 1.2;
            gameWindow.Cursor = Cursors.Arrow;
        }

        public void CreateHotbar()
        {
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(1, GridUnitType.Star);
            gameWindow.HotbarGrid.RowDefinitions.Add(row);

            for (int i = 0; i < Hotbar.MaxItems; i++)
            {
                ColumnDefinition def = new ColumnDefinition();
                def.Width = new GridLength(1, GridUnitType.Star);
                gameWindow.HotbarGrid.ColumnDefinitions.Add(def);
            }

            for (int i = 0; i < Hotbar.MaxItems; i++)
            {
                Image frame = new Image();
                frame.Source = (BitmapSource)gameWindow.Resources["ItemFrame"];
                frame.Name = "ItemFrame" + i;

                RenderOptions.SetBitmapScalingMode(frame, BitmapScalingMode.NearestNeighbor);

                Grid.SetRow(frame, 0);
                Grid.SetColumn(frame, i);

                gameWindow.HotbarGrid.Children.Add(frame);
                gameWindow.HotbarGrid.RegisterName(frame.Name, frame);
            }

            Image selectedFrame = new Image();
            selectedFrame.Source = (BitmapSource)gameWindow.Resources["SelectedItemFrame"];
            selectedFrame.Name = "SelectedFrame";

            RenderOptions.SetBitmapScalingMode(selectedFrame, BitmapScalingMode.NearestNeighbor);

            Grid.SetRow(selectedFrame, 0);
            Grid.SetColumn(selectedFrame, 0);

            gameWindow.HotbarGrid.Children.Add(selectedFrame);
            gameWindow.HotbarGrid.RegisterName(selectedFrame.Name, selectedFrame);
        }

        public void ReloadTextures()
        {
            foreach (var img in gameWindow.InventoryGrid.Children)
            {
                if (img is Image i)
                {
                    var data = i.Name.Split('_');
                    if (data[0] == "InventoryItem")
                        i.Source = new CroppedBitmap(AtlasTexturesData.CurrentTexture, AtlasTexturesData.GetTextureRect(Inventory.Blocks[int.Parse(data[2]), int.Parse(data[1])]));
                }
            }
            foreach (var img in gameWindow.HotbarGrid.Children)
            {
                if (img is Image i)
                {
                    var data = i.Name.Split('_');
                    if (data[0] == "HotbarItem" && Hotbar.Items[int.Parse(data[1])] != BlockType.Air)
                        i.Source = new CroppedBitmap(AtlasTexturesData.CurrentTexture, AtlasTexturesData.GetTextureRect(Hotbar.Items[int.Parse(data[1])]));
                }
            }
        }

        public void UpdateHotbarItems()
        {
            if (Hotbar != null)
            {
                foreach (var img in gameWindow.HotbarGrid.Children)
                {
                    if (img is Image i)
                    {
                        var data = i.Name.Split('_');
                        if (data[0] == "HotbarItem" && Hotbar.Items[int.Parse(data[1])] != BlockType.Air)
                            i.Source = new CroppedBitmap(AtlasTexturesData.CurrentTexture, AtlasTexturesData.GetTextureRect(Hotbar.Items[int.Parse(data[1])]));
                        else if (data[0] == "HotbarItem")
                            i.Source = null;
                    }
                }
            }
            gameWindow.HotbarGrid.Visibility = Visibility.Visible;
        }
    }
}
