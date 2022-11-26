using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.UI.Logic;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Minecraft.Controller
{
    public class MouseListener
    {
        public event Action? LeftMouseClick;
        public event Action? RightMouseClick;
        public MouseListener(GameWindow gameWindow)
        {
            gameWindow.MouseDown += OnMouseDown;
        }
        public void Reset()
        {
            LeftMouseClick = null;
            RightMouseClick = null;
        }
        public void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Ioc.Default.GetService<IUILogic>().IsGamePaused)
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
