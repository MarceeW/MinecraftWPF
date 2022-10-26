using Minecraft.Graphics;
using OpenTK.Mathematics;
using Minecraft.Terrain;
using Minecraft.Graphics.Shapes;
using System;
using System.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System.ComponentModel;

namespace Minecraft.Render
{
    class Scene : IDisposable, IScene
    {
        private const float near = 0.1f;
        private const float far = 1000.0f;

        public event ShaderMat4Handler ProjectionMatrixChange;
        private Matrix4 Projection;

        private WorldRenderer worldRenderer;
        private ICamera camera;
        private Skybox skybox;
        private CharacterHand characterHand;
        public Scene(WorldRenderer worldRenderer, CharacterHand characterHand)
        {
            Shader skyboxShader = new Shader(@"..\..\..\Graphics\Shaders\Skybox\skyboxVert.glsl", @"..\..\..\Graphics\Shaders\Skybox\skyboxFrag.glsl");

            skybox = new Skybox(
                skyboxShader,
                new Texture(@"..\..\..\Assets\Textures\McSkybox\", true, false));

            camera = Ioc.Default.GetService<ICamera>();
            this.worldRenderer = worldRenderer;

            camera.ViewMatrixChange += worldRenderer.Shader.SetMat4;
            ProjectionMatrixChange += worldRenderer.Shader.SetMat4;

            camera.ViewMatrixChange += skyboxShader.SetMat4;
            ProjectionMatrixChange += skyboxShader.SetMat4;

            camera.ViewMatrixChange += Cube.GetShader().SetMat4;
            ProjectionMatrixChange += Cube.GetShader().SetMat4;

            LineRenderer.InitShader();
            camera.ViewMatrixChange += LineRenderer.Shader.SetMat4;
            ProjectionMatrixChange += LineRenderer.Shader.SetMat4;

            this.characterHand = characterHand;
            camera.FrontChange += characterHand.Shader.SetVec3;
            ProjectionMatrixChange += characterHand.Shader.SetMat4;

            camera.PropertyChanged += (object? sender, PropertyChangedEventArgs e) => OnProjectionMatrixChange();
        }
        public void OnProjectionMatrixChange()
        {
            Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(camera.Fov), 16f/9f, near, far);
            ProjectionMatrixChange.Invoke("projection", Projection);
        }
        public void Render(float delta)
        {
            camera.UpdateViewMatrix();
            camera.UpdateFront();
            skybox.Reposition(camera.Position);
            skybox.Render();
            worldRenderer.RenderWorld();
            characterHand.Render(delta);
        }

        public void Dispose()
        {

        }
    }
}