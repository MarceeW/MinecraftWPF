using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Minecraft.Logic
{
    internal class Texture
    {
        public int Handle { get; private set; }
        public TextureUnit TexUnit { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        private bool isCubemap;
        public Texture(string path, bool _isCubeMap, bool needToFlip = true, TextureUnit textureUnit = TextureUnit.Texture0)
        {
            Handle = GL.GenTexture();
            isCubemap = _isCubeMap;

            if (!isCubemap)
            {
                Image<Rgba32> image = Image.Load<Rgba32>(path);
                GL.BindTexture(TextureTarget.Texture2D, Handle);

                Width = image.Width;
                Height = image.Height;

                if (needToFlip)
                    image.Mutate(x => x.Flip(FlipMode.Vertical));

                byte[] pixels = new byte[4 * image.Width * image.Height];
                image.CopyPixelDataTo(pixels);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 4);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
            else
            {
                Width = -1;
                Height = -1;

                var fileNames = Directory.GetFiles(path);
                GL.BindTexture(TextureTarget.TextureCubeMap, Handle);

                int i = 0;
                foreach (var f in fileNames)
                {
                    Image<Rgba32> image = Image.Load<Rgba32>(f);

                    if (needToFlip)
                        image.Mutate(x => x.Flip(FlipMode.Vertical));

                    byte[] pixels = new byte[4 * image.Width * image.Height];
                    image.CopyPixelDataTo(pixels);

                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i,
                        0, PixelInternalFormat.Rgba,
                        image.Width, image.Height, 0,
                        PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

                    i++;
                }
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

                GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
            }

            TexUnit = textureUnit;
        }
        public int GetTexUnitId()
        {
            return TexUnit - TextureUnit.Texture0;
        }
        public void Use()
        {
            GL.ActiveTexture(TexUnit);
            if (!isCubemap)
                GL.BindTexture(TextureTarget.Texture2D, Handle);
            else
                GL.BindTexture(TextureTarget.TextureCubeMap, Handle);
        }
    }
}
