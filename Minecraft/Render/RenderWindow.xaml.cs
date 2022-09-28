using Minecraft.Controller;
using Minecraft.Render;
using OpenTK.Mathematics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Wpf;
using System;
using OpenTK.Windowing.Common;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;

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
            Width = 1600;
            Height = 900;
            WindowState = System.Windows.WindowState.Maximized;
            WindowStyle = WindowStyle.None;

            var settings = new GLWpfControlSettings
            {
                MajorVersion = 4,
                MinorVersion = 2,
            };
            OpenTkControl.Start(settings);

            renderer = new Renderer();
            new GameController(renderer, this);

            var resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

            Left = resolution.Width / 2 - Width / 2;
            Top = resolution.Height / 2 - Height / 2;

            CenterPosition = new Vector2(resolution.Width / 2, resolution.Height / 2);
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

            renderer.RenderFrame();
        }
    }
}
