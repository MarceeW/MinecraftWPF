using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Minecraft.Controller
{
    public class MouseListener
    {
        public event Action? LeftMouseClick;
        public event Action? RightMouseClick;
        private GameWindow gameWindow;
        public MouseListener(GameWindow gameWindow)
        {
            this.gameWindow = gameWindow;

            gameWindow.MouseDown += OnMouseDown;
        }
        public void Reset()
        {
            LeftMouseClick = null;
            RightMouseClick = null;
        }
        public void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!gameWindow.IsGamePaused)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    LeftMouseClick?.Invoke();
                else if (e.RightButton == MouseButtonState.Pressed)
                    RightMouseClick?.Invoke();
            }
        }
        public void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
