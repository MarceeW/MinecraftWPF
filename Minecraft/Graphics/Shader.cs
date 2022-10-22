using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Minecraft.Graphics
{
    delegate void ShaderMat4Handler(string property, Matrix4 matrix);
    delegate void ShaderVec3Handler(string property, Vector3 vector);
    internal class Shader : IDisposable
    {
        private int handle;
        private bool disposeValue;
        private List<int> shaders;
        public Shader(string vertexPath,string fragmentPath)
        {
            shaders = new List<int>();
            handle = GL.CreateProgram();

            AttachShader(vertexPath, ShaderType.VertexShader);
            AttachShader(fragmentPath, ShaderType.FragmentShader);
        }
        ~Shader()
        {
            GL.DeleteProgram(handle);
        }
        public void AttachShader(string shaderPath, ShaderType shaderType)
        {
            string source;
            int shader;

            using (StreamReader reader = new StreamReader(shaderPath, Encoding.UTF8))
            {
                source = reader.ReadToEnd();
            }

            shader = GL.CreateShader(shaderType);
            GL.ShaderSource(shader, source);

            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int compileSuccess);

            if (compileSuccess == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                Console.WriteLine(infoLog);
            }
            else
            {
                shaders.Add(shader);
            }

            GL.AttachShader(handle, shader);
            GL.LinkProgram(handle);

            GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int linkSuccess);
            if (linkSuccess == 0)
            {
                string infoLog = GL.GetProgramInfoLog(handle);
                Console.WriteLine(infoLog);
            }

        }
        public void Use()
        {
            GL.UseProgram(handle);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposeValue)
            {
                foreach (int s in shaders)
                {
                    GL.DetachShader(handle, s);
                    GL.DeleteShader(s);
                }

                GL.DeleteProgram(handle);

                disposeValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(handle, attribName);
        }
        public int GetUniformLocation(string uniformName)
        {
            return GL.GetUniformLocation(handle, uniformName);
        }
        public void SetInt(string name, int value)
        {
            GL.UseProgram(handle);
            GL.Uniform1(GL.GetUniformLocation(handle, name), value);
        }
        public void SetMat4(string name, Matrix4 matrix)
        {
            GL.UseProgram(handle);
            GL.UniformMatrix4(GL.GetUniformLocation(handle, name), true, ref matrix);
        }
        public void SetVec3(string name, Vector3 vec)
        {
            GL.UseProgram(handle);
            GL.Uniform3(GL.GetUniformLocation(handle, name), vec);
        }
        public void SetDouble(string name, double value)
        {
            GL.UseProgram(handle);
            GL.Uniform1(GL.GetUniformLocation(handle, name), value);
        }
    }
}
