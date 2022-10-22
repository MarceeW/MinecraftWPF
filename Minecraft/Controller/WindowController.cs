using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using System.Windows.Input;

namespace Minecraft.Controller
{
    public class WindowController
    {
        public RenderWindow Window { get;set; }
        public bool NeedsToResetMouse = true;

        public bool ShowGrids { get; set;}
        public WindowController(RenderWindow window)
        {
            Window = window;
        }
        public void ResetMousePosition()
        {
            if (NeedsToResetMouse)
            {
                MouseController.MoveMouse(Window.CenterPosition);
            }
        }
    }
}
        
