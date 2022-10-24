using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Minecraft.Controller
{
    internal class MouseListener
    {
        public event Action? LeftMouseClick;
        public event Action? RightMouseClick;
        private RenderWindow renderWindow;
        public MouseListener(RenderWindow renderWindow)
        {
            this.renderWindow = renderWindow;
        }
        public void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!renderWindow.PauseMenuOpened)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    LeftMouseClick?.Invoke();
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    RightMouseClick?.Invoke();
                }
            }
        }
        public void OnMouseUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
