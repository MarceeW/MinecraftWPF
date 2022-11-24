using Microsoft.Toolkit.Mvvm.DependencyInjection;
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
            throw new NotImplementedException();
        }

        public void InventoryItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnKeyDown(KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnMouseDown(MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnMouseEnterBlockImage(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnMouseLeaveBlockImage(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnMouseWheel(MouseWheelEventArgs e)
        {
            throw new NotImplementedException();
        }

       
    }
}
