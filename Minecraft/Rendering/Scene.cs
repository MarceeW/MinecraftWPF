using Minecraft.Graphics;
using OpenTK.Mathematics;
using Minecraft.Terrain;
using Minecraft.Graphics.Shapes;
using System;
using System.Diagnostics;

namespace Minecraft.Render
{
    class Scene : IDisposable
    {
        private const float near = 0.1f;
        private const float far = 1000.0f;

        public event ShaderMat4Handler ProjectionMatrixChange;
        private Matrix4 Projection;

        private WorldRenderer worldRenderer;
        private ICamera camera;
        private Skybox skybox;
        public Scene(Camera camera,World world,WorldRenderer worldRenderer)
        {
            Shader skyboxShader = new Shader(@"..\..\..\Graphics\Shaders\Skybox\skyboxVert.glsl", @"..\..\..\Graphics\Shaders\Skybox\skyboxFrag.glsl");

            skybox = new Skybox(
                skyboxShader,
                new Texture(@"..\..\..\Assets\Textures\McSkybox\",true,false));

            this.camera = camera;
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
        }
        public void OnProjectionMatrixChange(float aspectRatio)
        {
            Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(camera.Fov), aspectRatio , near, far);
            ProjectionMatrixChange.Invoke("projection",Projection);
        }
        public void Render()
        {
            camera.UpdateViewMatrix();
            skybox.Reposition(camera.Position);
            skybox.Render();
            worldRenderer.RenderWorld();
        }

        public void Dispose()
        {
            
        }
    }
}