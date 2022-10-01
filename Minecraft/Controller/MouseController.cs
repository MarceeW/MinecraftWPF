﻿using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minecraft.Controller
{
    internal static class MouseController
    {
        internal static int DeltaX { get; private set; }
        internal static int DeltaY { get; private set; }

        internal static void ShowMouse()
        {
            Cursor.Show();
        }
        internal static void HideMouse()
        {
            Cursor.Hide();
        }
        internal static void MoveMouse(Vector2 pos)
        {
            DeltaX = Cursor.Position.X - (int)pos.X;
            DeltaY = Cursor.Position.Y - (int)pos.Y;

            Cursor.Position = new Point((int)pos.X, (int)pos.Y);
        }
    }
}
