using Minecraft.Controller;
using Minecraft.Render;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using System;
using OpenTK.Windowing.Common;
using System.Windows;
using System.Windows.Forms;
using Minecraft.UI;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Minecraft.Terrain;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;

namespace Minecraft
{
    public delegate void MouseInputHandler(MouseMoveEventArgs mouseMoveEventArgs);
    public delegate void GameUpdateHandler();
    partial class RenderWindow : Window
    {
        public Vector2 CenterPosition;
        public event Action<float>? RenderSizeChange;
        public bool ShouldClose { get; set; }
        public Hotbar? Hotbar { get; set; }
        public WindowController Controller;

        private Renderer renderer;

        public RenderWindow()
        {
            InitializeComponent();

            Title = "Minecraft";
            WindowState = System.Windows.WindowState.Maximized;
            WindowStyle = WindowStyle.None;

            var settings = new GLWpfControlSettings
            {
                MajorVersion = 3,
                MinorVersion = 1,
            };
            OpenTkControl.Start(settings);

            renderer = new Renderer();
            new GameController(renderer, this);

            var resolution = Screen.PrimaryScreen.Bounds;

            Left = resolution.Width / 2 - Width / 2;
            Top = resolution.Height / 2 - Height / 2;

            CenterPosition = new Vector2(resolution.Width / 2, resolution.Height / 2);

            float hudScale = 0.7f;

            Toolbar.Width = 900 * hudScale;
            Toolbar.Height = 100 * hudScale;

            Loaded += (object sender, RoutedEventArgs e) => SetupToolbarIcons();

            Controller = new WindowController(this);
        }
        private void SetupToolbarIcons()
        {
            if(Hotbar != null)
            {
                Item0.Source = new CroppedBitmap((BitmapSource)Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect(Hotbar.Content[0]));
                Item1.Source = new CroppedBitmap((BitmapSource)Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect(Hotbar.Content[1]));
                Item2.Source = new CroppedBitmap((BitmapSource)Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect(Hotbar.Content[2]));
                Item3.Source = new CroppedBitmap((BitmapSource)Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect(Hotbar.Content[3]));
                Item4.Source = new CroppedBitmap((BitmapSource)Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect(Hotbar.Content[4]));
                Item5.Source = new CroppedBitmap((BitmapSource)Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect(Hotbar.Content[5]));
                Item6.Source = new CroppedBitmap((BitmapSource)Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect(Hotbar.Content[6]));
                Item7.Source = new CroppedBitmap((BitmapSource)Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect(Hotbar.Content[7]));
                Item8.Source = new CroppedBitmap((BitmapSource)Resources["BlockAtlas"], AtlasTexturesData.GetTextureRect(Hotbar.Content[8]));
            }
        }
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if(e.Source == OpenTkControl)
            {
                switch (e.Key)
                {
                    case Key.T:
                        {
                            CommandLine.Focusable = true;

                            CommandLine.Visibility = Visibility.Visible;
                            CommandLine.Focus();

                            PlayerController.CanMove = false;
                        }
                        break;
                    case Key.G:
                        {
                            Controller.ShowGrids = !Controller.ShowGrids;
                            e.Handled = true;
                        }
                        break;
                    case Key.P:
                        {
                            Controller.NeedsToResetMouse = !Controller.NeedsToResetMouse;

                            if(Controller.NeedsToResetMouse)
                                MouseController.HideMouse();
                            else
                                MouseController.ShowMouse();
                        }
                        break;
                    case Key.Escape:
                        {
                            if (CommandLine.Visibility == Visibility.Visible)
                            {
                                CommandLine.Focusable = false;
                                CommandLine.Visibility = Visibility.Hidden;
                                PlayerController.CanMove = true;
                            }
                            else
                            {
                                ShouldClose = true;
                            }
                        }
                        break;
                }
            }

            base.OnKeyDown(e);
        }
        protected override void OnMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        {
            if(Hotbar != null)
            {
                Hotbar.UpdateSelectedIndex(e.Delta);
                Grid.SetColumn(SelectedItemFrame, Hotbar.SelectedItemIndex);
            }
            
            base.OnMouseWheel(e);
        }
        protected override void OnLocationChanged(EventArgs e)
        {
            CenterPosition = new Vector2((float)(Left + Width / 2), (float)(Top + Height / 2));
            base.OnLocationChanged(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            Environment.Exit(0);
            base.OnClosed(e);
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            CenterPosition = new Vector2((float)(Left + Width / 2), (float)(Top + Height / 2));


            RenderSizeChange?.Invoke((float)OpenTkControl.FrameBufferWidth / OpenTkControl.FrameBufferHeight);
        }
        private void OpenTkControl_OnRender(TimeSpan delta)
        {
            if (Controller.ShowGrids)
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            else
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            if (ShouldClose)
                Close();

            fpsCounter.Content = "FPS:\t" + Math.Round(1.0 / delta.TotalSeconds, 0);
            renderer.RenderFrame();
        }
    }
}
