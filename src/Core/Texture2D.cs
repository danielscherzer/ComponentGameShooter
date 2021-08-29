using ImageMagick;
using OpenTK.Graphics.OpenGL;
using System;
using System.IO;

namespace Core
{
	internal struct Texture2d
	{
		public Texture2d(Stream stream)
		{
			using (var image = new MagickImage(stream))
			{
				var format = PixelFormat.Rgb;
				switch (image.ChannelCount)
				{
					case 3: break;
					case 4: format = PixelFormat.Rgba; break;
					default: throw new ArgumentException("Unexpected image format");
				}
				image.Flip();
				var bytes = image.GetPixelsUnsafe().ToArray();
				TexturId = GL.GenTexture();
				Bind();
				GL.TexImage2D(Target, 0, (PixelInternalFormat)format, image.Width, image.Height, 0, format, PixelType.UnsignedByte, bytes);
				GL.TexParameter(Target, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
				GL.TexParameter(Target, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
				GL.TexParameter(Target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
				GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
				GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
				Unbind();
			}
		}

		public void Bind()
		{
			GL.BindTexture(Target, TexturId);
		}

		public static void Unbind()
		{
			GL.BindTexture(Target, 0);
		}

		public int TexturId { get; private set; }
		public const TextureTarget Target = TextureTarget.Texture2D;
	}
}
