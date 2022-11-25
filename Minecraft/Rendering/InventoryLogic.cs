using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Controller;
using Minecraft.Terrain;
using Minecraft.UI;
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

namespace Minecraft.Rendering
{
    internal class InventoryLogic : IInventoryLogic
    {
        public Inventory Inventory { get; private set; }
        public IHotbar Hotbar { get; private set; }

        
        internal GameWindow gw;
        internal PickedItem? pickedItem;


        public InventoryLogic(GameWindow gw)
        {
            Inventory = new Inventory();
            Hotbar = Ioc.Default.GetService<IHotbar>();
            this.gw = gw;
        }

      

        public void CreateInventory()
        {
            double inventoryFrameSize = gw.InventoryGrid.Width / Inventory.Columns;
            gw.InventoryGrid.Height = inventoryFrameSize * Inventory.Rows;

            for (int i = 0; i < Inventory.Rows; i++)
            {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(1, GridUnitType.Star);

                gw.InventoryGrid.RowDefinitions.Add(rowDef);
            }
            for (int i = 0; i < Inventory.Columns; i++)
            {
                ColumnDefinition colDef = new ColumnDefinition();
                colDef.Width = new GridLength(1, GridUnitType.Star);

                gw.InventoryGrid.ColumnDefinitions.Add(colDef);
            }

            for (int x = 0; x < Inventory.Columns; x++)
            {
                for (int y = 0; y < Inventory.Rows; y++)
                {
                    Image frame = new Image();
                    frame.Source = (BitmapSource)gw.Resources["InventoryFrame"];
                    frame.Name = $"InventoryItemFrame_{x}_{y}";

                    RenderOptions.SetBitmapScalingMode(frame, BitmapScalingMode.NearestNeighbor);

                    Grid.SetRow(frame, y);
                    Grid.SetColumn(frame, x);

                    gw.InventoryGrid.Children.Add(frame);
                    gw.InventoryGrid.RegisterName(frame.Name, frame);
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
                        tooltip.Style = (Style)gw.Resources["ItemToolTip"];
                        tooltip.Content = BlockData.GetBlockName(Inventory.Blocks[y, x]);
                        tooltip.Visibility = Visibility.Visible;

                        item.ToolTip = tooltip;
                        item.Source = new CroppedBitmap((BitmapSource)gw.Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect(Inventory.Blocks[y, x]));

                        RenderOptions.SetBitmapScalingMode(item, BitmapScalingMode.NearestNeighbor);

                        Grid.SetRow(item, y);
                        Grid.SetColumn(item, x);

                        gw.InventoryGrid.Children.Add(item);
                        gw.InventoryGrid.RegisterName(item.Name, item);
                    }
                }
            }
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
                        item.Source = new CroppedBitmap((BitmapSource)gw.Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect(Hotbar.Items[i]));

                    RenderOptions.SetBitmapScalingMode(item, BitmapScalingMode.NearestNeighbor);

                    Grid.SetRow(item, 0);
                    Grid.SetColumn(item, i);

                    gw.HotbarGrid.Children.Add(item);
                    gw.HotbarGrid.RegisterName(item.Name, item);
                }
            }
        }

        public void HotbarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Hotbar != null)
            {
                var grid = sender as Grid;

                var pos = e.GetPosition(grid);

                pos.X /= grid.Width / Hotbar.MaxItems;

                var clickedImage = (Image)gw.HotbarGrid.FindName($"HotbarItem_{(int)pos.X}");

                if (pickedItem != null)
                {
                    BlockType toChange = pickedItem.type;

                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        pickedItem.src = clickedImage.Source?.Clone();
                        pickedItem.type = Hotbar.Items[(int)pos.X];

                        clickedImage.Source = gw.PickedItemImage.Source;

                        gw.PickedItemImage.Source = pickedItem.src;
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
                    gw.PickedItemImage.Source = pickedItem.src;
                    Hotbar.ChangeBlock((int)pos.X, BlockType.Air);

                    gw.PickedItemCanvas.Margin = new Thickness(e.GetPosition(null).X - gw.PickedItemImage.Width - 10, e.GetPosition(null).Y, 0, 0);
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    clickedImage.Source = null;
                    Hotbar.ChangeBlock((int)pos.X, BlockType.Air);
                }
            }
        }

        public void InventoryItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var item = sender as Image;
                var itemData = item.Name.Split('_');

                gw.PickedItemImage.Source = item.Source;

                pickedItem = new PickedItem(item.Source.Clone(), Inventory.Blocks[int.Parse(itemData[2]), int.Parse(itemData[1])]);
            }
            else
            {
                gw.PickedItemImage.Source = null;
                pickedItem = null;
            }
        }

        public void OnMouseEnterBlockImage(object sender, MouseEventArgs e)
        {
            (sender as Image).Width *= 1.2;
            gw.Cursor = Cursors.Hand;
        }

        public void OnMouseLeaveBlockImage(object sender, MouseEventArgs e)
        {
            (sender as Image).Width /= 1.2;
            gw.Cursor = Cursors.Arrow;
        }

        public void CreateHotbar()
        {
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(1, GridUnitType.Star);
            gw.HotbarGrid.RowDefinitions.Add(row);

            for (int i = 0; i < Hotbar.MaxItems; i++)
            {
                ColumnDefinition def = new ColumnDefinition();
                def.Width = new GridLength(1, GridUnitType.Star);
                gw.HotbarGrid.ColumnDefinitions.Add(def);
            }

            for (int i = 0; i < Hotbar.MaxItems; i++)
            {
                Image frame = new Image();
                frame.Source = (BitmapSource)gw.Resources["ItemFrame"];
                frame.Name = "ItemFrame" + i;

                RenderOptions.SetBitmapScalingMode(frame, BitmapScalingMode.NearestNeighbor);

                Grid.SetRow(frame, 0);
                Grid.SetColumn(frame, i);

                gw.HotbarGrid.Children.Add(frame);
                gw.HotbarGrid.RegisterName(frame.Name, frame);
            }

            Image selectedFrame = new Image();
            selectedFrame.Source = (BitmapSource)gw.Resources["SelectedItemFrame"];
            selectedFrame.Name = "SelectedFrame";

            RenderOptions.SetBitmapScalingMode(selectedFrame, BitmapScalingMode.NearestNeighbor);

            Grid.SetRow(selectedFrame, 0);
            Grid.SetColumn(selectedFrame, 0);

            gw.HotbarGrid.Children.Add(selectedFrame);
            gw.HotbarGrid.RegisterName(selectedFrame.Name, selectedFrame);
        }

        public void ReloadTextures()
        {
            foreach (var img in gw.InventoryGrid.Children)
            {
                if (img is Image i)
                {
                    var data = i.Name.Split('_');
                    if (data[0] == "InventoryItem")
                        i.Source = new CroppedBitmap(gw.currentTexture, AtlasTexturesData.GetTextureRect(Inventory.Blocks[int.Parse(data[2]), int.Parse(data[1])]));
                }
            }
            foreach (var img in gw.HotbarGrid.Children)
            {
                if (img is Image i)
                {
                    var data = i.Name.Split('_');
                    if (data[0] == "HotbarItem" && Hotbar.Items[int.Parse(data[1])] != BlockType.Air)
                        i.Source = new CroppedBitmap(gw.currentTexture, AtlasTexturesData.GetTextureRect(Hotbar.Items[int.Parse(data[1])]));
                }
            }
        }

        public void UpdateHotbarItems()
        {
            if (Hotbar != null)
            {
                foreach (var img in gw.HotbarGrid.Children)
                {
                    if (img is Image i)
                    {
                        var data = i.Name.Split('_');
                        if (data[0] == "HotbarItem" && Hotbar.Items[int.Parse(data[1])] != BlockType.Air)
                            i.Source = new CroppedBitmap(gw.currentTexture, AtlasTexturesData.GetTextureRect(Hotbar.Items[int.Parse(data[1])]));
                        else if (data[0] == "HotbarItem")
                            i.Source = null;
                    }
                }
            }
            gw.HotbarGrid.Visibility = Visibility.Visible;
        }
    }
}
