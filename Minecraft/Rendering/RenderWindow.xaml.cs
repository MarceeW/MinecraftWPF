using Minecraft.Controller;
using Minecraft.Render;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using System;
using OpenTK.Windowing.Common;
using System.Windows;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;

namespace Minecraft
{
    public delegate void MouseInputHandler(MouseMoveEventArgs mouseMoveEventArgs);
    public delegate void GameUpdateHandler();
    partial class RenderWindow : Window
    {
        public Vector2 CenterPosition;
        public event Action<float> RenderSizeChange;

        private Renderer renderer;
        public bool ShouldClose { get; set; }
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
            if (ShouldClose)
                Close();

            fpsCounter.Content = "FPS:\t" + Math.Round(1.0 / delta.TotalSeconds, 0);
            renderer.RenderFrame();
        }
    }
}
