using Minecraft.Controller;
using Minecraft.Terrain;
using Minecraft.UI;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Minecraft.Graphics
{
    internal class CharacterHand
    {
        public Shader Shader { get; } = new Shader(@"..\..\..\Graphics\Shaders\CharacterHand\handVert.glsl", @"..\..\..\Graphics\Shaders\CharacterHand\handFrag.glsl");

        private Hotbar hotbar;

        private int VBO, VAO;

        private Vector3 position;
        private Matrix4 modelMatrix;
        private Matrix4 viewMatrix;

        private enum AnimationType { None, BlockChange, Hit, Place }
        private double currentAnimationStep = 0;
        private AnimationType animation = AnimationType.None;
        private bool blockChangeHandled = true;

        private bool isHandEmpty = false;
        public CharacterHand(Hotbar hotbar)
        {
            position = new Vector3(0.5f,-1.15f,-1.05f);
            modelMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(40));
            modelMatrix *= Matrix4.CreateScale(0.6f);

            this.hotbar = hotbar;
            hotbar.BlockChangeOnSelect += OnSwitchBlock;

            UpdateModelMatrix();
            UpdateViewMatrix(Vector3.Zero);

            SetupVBO();
        }

        public void Render(float delta)
        {
            UpdatePosition(delta);

            if (!isHandEmpty)
            {
                Shader.Use();
                GL.DepthFunc(DepthFunction.Always);

                GL.BindVertexArray(VAO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 24);

                GL.DepthFunc(DepthFunction.Less);
            }
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
        private void ResetAnimation()
        {
            currentAnimationStep = 0;
            animation = AnimationType.None;
        }
        private void ResetModel()
        {
            modelMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(40));
            modelMatrix *= Matrix4.CreateScale(0.6f);
        }
        private void UpdatePosition(float delta)
        {
            if(animation == AnimationType.BlockChange)
            {
                if(currentAnimationStep >= Math.PI)
                {
                    ResetAnimation();
                    UpdateViewMatrix(Vector3.Zero);
                }
                else
                {
                    if(!blockChangeHandled && Math.PI / 2 <= currentAnimationStep)
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
                if (currentAnimationStep >= Math.PI)
                {
                    ResetAnimation();
                    ResetModel();
                    UpdateModelMatrix();
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
        public void OnBlockPlace()
        {
            currentAnimationStep = 0;
            animation = AnimationType.Place;
        }
        public void OnHit()
        {
            currentAnimationStep = 0;
            animation = AnimationType.Hit;
        }
        public void OnSwitchBlock()
        {
            if(animation == AnimationType.Hit || animation == AnimationType.Place)
            {
                currentAnimationStep = 0;
                ResetModel();
                UpdateModelMatrix();
            }
                
            animation = AnimationType.BlockChange;
            blockChangeHandled = false;
        }
        private void SetupVBO()
        {
            VBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();

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
        private void UpdateVBOData()
        {
            var type = hotbar.GetSelectedBlock();

            if (type != BlockType.Air)
            {
                isHandEmpty = false;

                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BindVertexArray(VAO);

                List<float> data = new List<float>();

                if (BlockData.IsVegetationBlock(type))
                {
                    data.AddRange(Face.GetBlockFaceVertices(type, FaceDirection.Left, new Vector3(0, .5f, 0),false,0));
                }
                else
                {
                    data.AddRange(Face.GetBlockFaceVertices(type, FaceDirection.Bot, Vector3.Zero,false,0));
                    data.AddRange(Face.GetBlockFaceVertices(type, FaceDirection.Front, Vector3.Zero,false, 0));
                    data.AddRange(Face.GetBlockFaceVertices(type, FaceDirection.Left, Vector3.Zero,false, 0));
                    data.AddRange(Face.GetBlockFaceVertices(type, FaceDirection.Top, Vector3.Zero,false, 0));
                }

                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Count, data.ToArray(), BufferUsageHint.StaticDraw);
            }
            else
                isHandEmpty = true;
        }
    }
}
