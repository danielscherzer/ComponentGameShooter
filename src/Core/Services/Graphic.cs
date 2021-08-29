using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Zenseless.OpenTK;
using Zenseless.Resources;

namespace Core.Services
{
	/// <summary>
	/// Class that handles all the actual drawing using OpenGL.
	/// </summary>
	internal class Graphic : IGraphic
	{
		internal Graphic()
		{
			var textureDir = new EmbeddedResourceDirectory(nameof(Example) + ".Content.Textures");

			// load textures
			foreach (var resourceName in textureDir.EnumerateResources())
			{
				using var stream = textureDir.Open(resourceName);
				var shortName = Path.GetFileNameWithoutExtension(resourceName);
				_textures.Add(shortName, Texture2DLoader.Load(stream));
				_spriteBatches.Add(shortName, new List<(Box2, Box2)>());
			}

			var spriteSheetDir = new EmbeddedResourceDirectory(nameof(Example) + ".Content.SpriteSheets");
			var serializer = new XmlSerializer(typeof(SpriteSheet));
			// load sprite sheets
			foreach (var resourceName in spriteSheetDir.EnumerateResources())
			{
				using var stream = spriteSheetDir.Open(resourceName);
				if (serializer.Deserialize(stream) is not SpriteSheet spriteSheet)
				{
					Trace.WriteLine($"Could not deserialize sprite sheet {resourceName}");
					continue;
				}
				var shortName = Path.GetFileNameWithoutExtension(resourceName);
				_spriteSheets.Add(shortName, spriteSheet);
			}
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		}

		internal void Draw()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit); // clear the screen

			GL.Enable(EnableCap.Blend);
			GL.Color4(Color4.White);
			DrawSprites();
			GL.Disable(EnableCap.Blend);
			foreach (var spriteBatch in _spriteBatches)
			{
				spriteBatch.Value.Clear();
			}

			GL.Color4(Color4.DarkSeaGreen);
			DrawRectangles();
			rectangles.Clear();
		}

		internal static void Resize(int width, int height)
		{
			GL.Viewport(0, 0, width, height); // tell OpenGL to use the whole window for drawing

			var invWindowAspectRatio = height / (float)width;
			var windowAspectMatrix = Matrix4.CreateScale(invWindowAspectRatio, 1f, 1f);
			GL.LoadMatrix(ref windowAspectMatrix);
		}

		public void DrawRectangle(Box2 rectangle)
		{
			rectangles.Add(rectangle);
		}

		public void DrawSprite(string textureName, Box2 rectangle, Box2 texCoords)
		{
			_spriteBatches[textureName].Add((rectangle, texCoords));
		}

		public void DrawText(string textureName, Box2 firstCharacterRectangle, string text)
		{
			var spriteSheet = _spriteSheets[textureName];
			var batch = _spriteBatches[textureName];
			var rect = firstCharacterRectangle;
			byte[] asciiBytes = Encoding.ASCII.GetBytes(text);
			foreach (var character in asciiBytes)
			{
				uint id = character - spriteSheet.FirstASCII;
				var texCoords = spriteSheet.CalcTexCoordsFromId(id);
				batch.Add((rect, texCoords));
				rect = rect.Translated(new Vector2(rect.Size.X, 0f));
			}
		}

		public Box2 TexCoordsForAnimation(string textureName, float normalizedAnimationTime)
		{
			var spriteSheet = _spriteSheets[textureName];
			return spriteSheet.CalcTexCoordsFromAnimationTime(normalizedAnimationTime);
		}

		private readonly Dictionary<string, Texture2D> _textures = new();
		private readonly Dictionary<string, List<(Box2 bounds, Box2 texCoord)>> _spriteBatches = new();
		private readonly Dictionary<string, SpriteSheet> _spriteSheets = new();
		private readonly List<Box2> rectangles = new();

		private void DrawRectangles()
		{
			foreach (var rectangle in rectangles)
			{
				GL.Begin(PrimitiveType.LineLoop);
				GL.Vertex2(rectangle.Min);
				GL.Vertex2(rectangle.Max.X, rectangle.Min.Y);
				GL.Vertex2(rectangle.Max);
				GL.Vertex2(rectangle.Min.X, rectangle.Max.Y);
				GL.End();
			}
		}

		private void DrawSprites()
		{
			GL.Enable(EnableCap.Texture2D); //activate texturing
			foreach (var spriteBatch in _spriteBatches)
			{
				var texture = _textures[spriteBatch.Key];
				GL.BindTexture(TextureTarget.Texture2D, texture.Handle);
				foreach ((Box2 bounds, Box2 texCoord) in spriteBatch.Value)
				{
					DrawTextured(bounds, texCoord);
				}
			}
			GL.Disable(EnableCap.Texture2D);
		}

		private static void DrawTextured(Box2 bounds, Box2 texCoords)
		{
			GL.Begin(PrimitiveType.Quads);
			GL.TexCoord2(texCoords.Min); GL.Vertex2(bounds.Min);
			GL.TexCoord2(texCoords.Max.X, texCoords.Min.Y); GL.Vertex2(bounds.Max.X, bounds.Min.Y);
			GL.TexCoord2(texCoords.Max); GL.Vertex2(bounds.Max);
			GL.TexCoord2(texCoords.Min.X, texCoords.Max.Y); GL.Vertex2(bounds.Min.X, bounds.Max.Y);
			GL.End();
		}
	}
}
