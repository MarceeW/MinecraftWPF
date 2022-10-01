using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Minecraft.Controller
{
    internal class MouseListener
    {
        //Stopwatch clickStopWatch;
        //bool isLeftButtonDown = false;
        //bool isRightButtonDown = false;

        public event Action LeftMouseClick;
        public event Action RightMouseClick;
        public void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                LeftMouseClick?.Invoke();
            }
            else if(e.RightButton == MouseButtonState.Pressed)
            {
                RightMouseClick?.Invoke();
            }
        }
        public void OnMouseUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
