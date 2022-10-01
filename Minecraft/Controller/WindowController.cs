using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using System.Windows.Input;

namespace Minecraft.Controller
{
    internal static class WindowController
    {
        public static RenderWindow? Window { get;set; }
        public static bool ShowGrids {get; private set;}
        private static bool needsToResetMouse = true;
        public static void CheckForKeyPress()
        {
            if(Window != null)
            {
                if (Keyboard.IsKeyDown(Key.Escape))
                    Window.ShouldClose = true;

                if (Keyboard.IsKeyDown(Key.G))
                {
                    ShowGrids = !ShowGrids;
                }
                if (Keyboard.IsKeyDown(Key.P))
                {
                    needsToResetMouse = !needsToResetMouse;
                }
            }
        }
        
        public static void ResetMousePosition()
        {
            if (needsToResetMouse)
            {
                if (Window != null)
                    MouseController.MoveMouse(Window.CenterPosition);
            }
        }
    }
}
        
