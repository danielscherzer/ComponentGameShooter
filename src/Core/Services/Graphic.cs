using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Core.Services
{
	using Rectangle = Core.Rectangle;
	/// <summary>
	/// Class that handles all the actual drawing using OpenGL.
	/// </summary>
	internal class Graphic : IGraphic
	{
		internal Graphic()
		{
			// load textures
			void LoadSprite(string name, Stream stream)
			{
				var shortName = Path.GetFileNameWithoutExtension(name);
				textures.Add(shortName, new Texture2d(stream));
				spriteBatches.Add(shortName, new List<Tuple<Rectangle, Rectangle>>());
			}
			ResourceStreams.IterateOverFiles("Images", LoadSprite);
			var serializer = new XmlSerializer(typeof(SpriteSheet));
			void LoadSpriteSheet(string name, Stream stream)
			{
				var shortName = Path.GetFileNameWithoutExtension(name);
				var spriteSheet = serializer.Deserialize(stream) as SpriteSheet;
				spriteSheets.Add(shortName, spriteSheet);
			}
			ResourceStreams.IterateOverFiles("SpriteSheets", LoadSpriteSheet);

			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		}

		internal void Draw()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit); // clear the screen

			GL.Enable(EnableCap.Blend);
			GL.Color4(Color4.White);
			DrawSprites();
			GL.Disable(EnableCap.Blend);
			foreach (var spriteBatch in spriteBatches)
			{
				spriteBatch.Value.Clear();
			}

			GL.Color4(Color4.DarkSeaGreen);
			DrawRectangles();
			rectangles.Clear();
		}

		internal void Resize(int width, int height)
		{
			GL.Viewport(0, 0, width, height); // tell OpenGL to use the whole window for drawing

			var invWindowAspectRatio = height / (float)width;
			var windowAspectMatrix = Matrix4.CreateScale(invWindowAspectRatio, 1f, 1f);
			GL.LoadMatrix(ref windowAspectMatrix);
		}

		public void DrawRectangle(Rectangle rectangle)
		{
			rectangles.Add(rectangle);
		}

		public void DrawSprite(string textureName, Rectangle rectangle, Rectangle texCoords)
		{
			spriteBatches[textureName].Add(new Tuple<Rectangle, Rectangle>(rectangle, texCoords));
		}

		public void DrawText(string textureName, Rectangle firstCharacterRectangle, string text)
		{
			var spriteSheet = spriteSheets[textureName];
			var batch = spriteBatches[textureName];
			var rect = firstCharacterRectangle;
			byte[] asciiBytes = Encoding.ASCII.GetBytes(text);
			foreach (var character in asciiBytes)
			{
				uint id = character - spriteSheet.FirstASCII;
				var texCoords = spriteSheet.CalcTexCoordsFromId(id);
				batch.Add(new Tuple<Rectangle, Rectangle>(rect, texCoords));
				rect = rect.Translate(new Vector2(rect.Size.X, 0f));
			}
		}

		public Rectangle TexCoordsForAnimation(string textureName, float normalizedAnimationTime)
		{
			var spriteSheet = spriteSheets[textureName];
			return spriteSheet.CalcTexCoordsFromAnimationTime(normalizedAnimationTime);
		}

		private readonly Dictionary<string, Texture2d> textures = new();
		private readonly Dictionary<string, List<Tuple<Rectangle, Rectangle>>> spriteBatches = new();
		private readonly Dictionary<string, SpriteSheet> spriteSheets = new();
		private readonly List<Rectangle> rectangles = new();

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
			foreach (var spriteBatch in spriteBatches)
			{
				var texture = textures[spriteBatch.Key];
				texture.Bind();
				foreach (var sprite in spriteBatch.Value)
				{
					DrawTextured(sprite.Item1, sprite.Item2);
				}
			}
			GL.Disable(EnableCap.Texture2D);
		}

		private static void DrawTextured(Rectangle rectangle, Rectangle texCoords)
		{
			GL.Begin(PrimitiveType.Quads);
			GL.TexCoord2(texCoords.Min); GL.Vertex2(rectangle.Min);
			GL.TexCoord2(texCoords.Max.X, texCoords.Min.Y); GL.Vertex2(rectangle.Max.X, rectangle.Min.Y);
			GL.TexCoord2(texCoords.Max); GL.Vertex2(rectangle.Max);
			GL.TexCoord2(texCoords.Min.X, texCoords.Max.Y); GL.Vertex2(rectangle.Min.X, rectangle.Max.Y);
			GL.End();
		}
	}
}
