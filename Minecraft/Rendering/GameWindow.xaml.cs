using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Controller;
using Minecraft.Misc;
using Minecraft.Render;
using Minecraft.Rendering.ViewModel;
using Minecraft.UI.Logic;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Wpf;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Minecraft
{
    public delegate void MouseInputHandler(MouseMoveEventArgs mouseMoveEventArgs);
    public delegate void GameUpdateHandler();
    partial class GameWindow : Window
    {

        public Vector2 CenterPosition;
        public event Action? RenderSizeChange;
        public bool ShowWireFrames = false;
        public double WinWidth { get { return this.ActualWidth; } }
        public double WinHeight { get { return this.ActualHeight; } }
        public MouseListener MouseListener { get; private set; }

        internal Renderer renderer;
        private double mainMenuTitleAnim = 0;
        
        public GameWindow()
        {
            InitializeComponent();

            WindowState = System.Windows.WindowState.Maximized;

            var settings = new GLWpfControlSettings
            {
                MajorVersion = 3,
                MinorVersion = 1,
            };
            OpenTkControl.Start(settings);

            var resolution = Screen.PrimaryScreen.Bounds;

            Left = resolution.Width / 2 - Width / 2;
            Top = resolution.Height / 2 - Height / 2;

            CenterPosition = new Vector2(resolution.Width / 2, resolution.Height / 2);
            float hudScale = 0.6f;

            MouseListener = new MouseListener(this);

            HotbarGrid.Width *= hudScale;
            HotbarGrid.Height *= hudScale;

            var vm = new GameWindowViewModel();
            vm.GameWindow = this;

            DataContext = vm;


            Ioc.Default.GetService<IUILogic>().GameWindow = this;
            Ioc.Default.GetService<IInventoryLogic>().GameWindow = this;

            
            
        }
        protected override void OnLocationChanged(EventArgs e)
        {
            CenterPosition = new Vector2((float)(Left + Width / 2), (float)(Top + Height / 2));
            base.OnLocationChanged(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            UserSettings.Save((float)FovSlider.Value, (int)RenderDistanceSlider.Value, (float)SensitivitySlider.Value);
            Environment.Exit(0);
            base.OnClosed(e);
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            CenterPosition = new Vector2((float)(Left + Width / 2), (float)(Top + Height / 2));

            RenderSizeChange?.Invoke();
        }


        private void OpenTkControl_OnRender(TimeSpan delta)
        {
            if (!UILogic.IsInMainMenu)
            {
                if (ShowWireFrames)
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                else
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

                renderer.RenderFrame(delta.Milliseconds / 1000.0f);

                fpsCounter.Text = Math.Round(1.0 / delta.TotalSeconds, 0) + " Fps";
            }
            else
            {
                SplashText.FontSize += Math.Sin(mainMenuTitleAnim += 0.05) * 0.03;
            }
        }
        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var img = e.Source as Image;
            img.Source = (BitmapSource)Resources["MenuButtonSelectedFrame"];
        }
        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var img = e.Source as Image;
            img.Source = (BitmapSource)Resources["MenuButtonFrame"];
        }
        private void WorldSelector_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left && e.ClickCount == 2)
                Ioc.Default.GetService<IUILogic>().EnterWorld();
        }

        private void BorderlessOption_Click(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Normal;
            WindowStyle = BorderlessOption.IsChecked == true ? WindowStyle.None : WindowStyle.SingleBorderWindow;
            WindowState = System.Windows.WindowState.Maximized;
        }

        
        private void MainMenu_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var c in this.MainMenu.Children)
            {
                if (c is System.Windows.Controls.Button b)
                {
                    b.Width = this.ActualWidth * 0.2083333333;
                    b.Height = this.ActualHeight * 0.05555555556;
                }
            }
        }

        
    }
}