using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Minecraft.Logic;
using Minecraft.Terrain;
using Minecraft.UI;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Minecraft.Graphics
{
    internal class CharacterHand
    {
        public Shader Shader { get; } = new Shader(@"..\..\..\Graphics\Shaders\CharacterHand\handVert.glsl", @"..\..\..\Graphics\Shaders\CharacterHand\handFrag.glsl");

        private static string handTexturePath = @"..\..\..\Assets\Textures\hand.png";
        private Texture handTexture;
        private IHotbar hotbar;

        private int VBO, VAO;

        private Vector3 position;
        private Matrix4 modelMatrix;
        private Matrix4 viewMatrix;

        private enum AnimationType { None, BlockChange, Hit, Place, Walk }
        private double currentAnimationStep = 0;

        private double currentAnimationWalkingStep = 0;

        private AnimationType animation = AnimationType.None;
        private bool blockChangeHandled = true;

        private bool isHandEmpty;
        public CharacterHand()
        {
            position = new Vector3(0.5f, -1.15f, -1.05f);
            modelMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(40));
            modelMatrix *= Matrix4.CreateScale(0.6f);

            hotbar = Ioc.Default.GetService<IHotbar>();
            isHandEmpty = hotbar.Items[hotbar.SelectedItemIndex] == BlockType.Air;
            hotbar.BlockChangeOnSelect += OnSwitchBlock;

            Ioc.Default.GetService<IPlayerLogic>().Walking += OnWalking;

            handTexture = new Texture(handTexturePath, false);

            UpdateViewMatrix(Vector3.Zero);

            SetupVBO();
            ResetModel();
            UpdateModelMatrix();
        }

        public void Render(float delta)
        {
            UpdatePosition(delta);

            if (!isHandEmpty)
            {
                AtlasTexturesData.Atlas.Use();
                Shader.SetInt("tex", AtlasTexturesData.Atlas.GetTexUnitId());
                Shader.Use();
            }
            else
            {
                handTexture.Use();
                Shader.SetInt("tex", handTexture.GetTexUnitId());
                Shader.Use();
            }

            GL.DepthFunc(DepthFunction.Always);

            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 24);

            GL.DepthFunc(DepthFunction.Less);
        }
        private void UpdateModelMatrix()
        {
            Shader.Use();
            Shader.SetMat4("model", modelMatrix);
        }
        private void UpdateViewMatrix(Vector3 deltaPos)
        {
            viewMatrix = Matrix4.CreateTranslation(position + deltaPos);

            Shader.Use();
            Shader.SetMat4("view", viewMatrix);
        }
        private void ResetViewMatrix()
        {
            viewMatrix = Matrix4.CreateTranslation(position);
            Shader.Use();
            Shader.SetMat4("view", viewMatrix);
        }
        private void ResetAnimation()
        {
            currentAnimationStep = 0;
            currentAnimationWalkingStep = 0;
            animation = AnimationType.None;
        }
        private void ResetModel()
        {
            if (!isHandEmpty)
            {
                modelMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(40));
                modelMatrix *= Matrix4.CreateScale(0.6f);
            }
            else
            {
                modelMatrix = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(-35));
                modelMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(40));
                modelMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-20));
                modelMatrix *= Matrix4.CreateScale(0.8f);
            }
        }
        private void UpdatePosition(float delta)
        {
            if (animation == AnimationType.BlockChange)
            {
                if (currentAnimationStep >= Math.PI)
                {
                    ResetAnimation();
                    ResetViewMatrix();

                    animation = AnimationType.None;
                }
                else
                {
                    if (!blockChangeHandled && Math.PI / 2 <= currentAnimationStep)
                    {
                        UpdateVBOData();
                        blockChangeHandled = true;
                    }

                    UpdateViewMatrix(new Vector3(0, -(float)Math.Sin(currentAnimationStep), 0));

                    int animationSpeed = 3;
                    currentAnimationStep += Math.PI * delta * animationSpeed;
                }
            }
            else if (animation == AnimationType.Place || animation == AnimationType.Hit)
            {
                if (animation == AnimationType.Hit && isHandEmpty)
                {
                    if (currentAnimationStep >= Math.PI)
                    {
                        ResetAnimation();
                        ResetModel();
                        UpdateModelMatrix();
                        ResetViewMatrix();

                        animation = AnimationType.None;
                    }
                    else
                    {
                        ResetModel();
                        UpdateViewMatrix(new Vector3(-(float)Math.Sin(currentAnimationStep) * 0.7f, -(float)Math.Sin(currentAnimationStep) * 0.3f, (float)Math.Sin(currentAnimationStep) * 0.3f));
                        modelMatrix *= Matrix4.CreateRotationX(-(float)Math.Sin(currentAnimationStep));
                        modelMatrix *= Matrix4.CreateRotationY((float)Math.Sin(currentAnimationStep) * 0.5f);
                        UpdateModelMatrix();

                        float animationSpeed = 3.5f;
                        currentAnimationStep += Math.PI * delta * animationSpeed;
                    }
                }
                else
                {
                    if (currentAnimationStep >= Math.PI)
                    {
                        ResetAnimation();
                        ResetModel();
                        UpdateModelMatrix();

                        animation = AnimationType.None;
                    }
                    else
                    {
                        ResetModel();
                        modelMatrix *= Matrix4.CreateRotationX((float)-Math.Sin(currentAnimationStep));
                        modelMatrix *= Matrix4.CreateRotationZ((float)Math.Sin(currentAnimationStep) * 0.1f);
                        UpdateModelMatrix();

                        int animationSpeed = 5;
                        currentAnimationStep += Math.PI * delta * animationSpeed;
                    }
                }
            }
            else if (animation == AnimationType.Walk)
            {
                UpdateViewMatrix(new Vector3((float)Math.Cos(currentAnimationWalkingStep / 2) / 10, -(float)Math.Sin(currentAnimationWalkingStep) / 15, 0));
                animation = AnimationType.None;
            }
        }
        public void OnBlockPlace()
        {
            ResetViewMatrix();
            ResetAnimation();
            animation = AnimationType.Place;
        }
        public void OnHit()
        {
            ResetViewMatrix();
            ResetAnimation();
            animation = AnimationType.Hit;
        }
        public void OnSwitchBlock()
        {
            ResetViewMatrix();

            if (animation == AnimationType.Hit || animation == AnimationType.Place)
            {
                ResetAnimation();
                ResetModel();
                UpdateModelMatrix();
            }
            if (!isHandEmpty || hotbar.GetSelectedBlock() != BlockType.Air)
            {
                animation = AnimationType.BlockChange;
                blockChangeHandled = false;
            }
        }
        public void OnWalking(float delta)
        {
            if (animation == AnimationType.None)
            {
                animation = AnimationType.Walk;

                int animationSpeed = 6;
                currentAnimationWalkingStep += Math.PI * delta * animationSpeed;
            }
        }
        private void SetupVBO()
        {
            VBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BindVertexArray(VAO);

            UpdateVBOData();

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 9 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, 9 * sizeof(float), 8 * sizeof(float));
            GL.EnableVertexAttribArray(3);
        }
        private void UpdateVBOData(float? fov = null)
        {
            var type = hotbar.GetSelectedBlock();
            List<float> data = new List<float>();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BindVertexArray(VAO);

            if (type != BlockType.Air)
            {
                if (isHandEmpty)
                {
                    isHandEmpty = false;
                    ResetModel();
                    UpdateModelMatrix();
                }
                isHandEmpty = false;

                if (BlockData.IsVegetationBlock(type))
                    data.AddRange(Face.GetBlockFaceVertices(type, FaceDirection.Left, new Vector3(0, .5f, 0), false, 0));
                else
                {
                    data.AddRange(Face.GetBlockFaceVertices(type, FaceDirection.Bot, Vector3.Zero, false, 0));
                    data.AddRange(Face.GetBlockFaceVertices(type, FaceDirection.Front, Vector3.Zero, false, 0));
                    data.AddRange(Face.GetBlockFaceVertices(type, FaceDirection.Left, Vector3.Zero, false, 0));
                    data.AddRange(Face.GetBlockFaceVertices(type, FaceDirection.Top, Vector3.Zero, false, 0));
                }

            }
            else
            {
                if (!isHandEmpty)
                {
                    isHandEmpty = true;
                    ResetModel();
                    UpdateModelMatrix();
                }

                Vector3 pos = new Vector3(0.75f, 0.5f, -0.5f);

                float thickness = 0.5f;
                data.AddRange(Face.GetHandFaceVertices(FaceDirection.Front, pos, thickness));
                data.AddRange(Face.GetHandFaceVertices(FaceDirection.Left, pos, thickness));
                data.AddRange(Face.GetHandFaceVertices(FaceDirection.Top, pos, thickness));
            }
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Count, data.ToArray(), BufferUsageHint.StaticDraw);
        }
    }
}
