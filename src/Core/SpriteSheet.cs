using System;

namespace Core
{
	public class SpriteSheet
	{
		private class Animation
		{
			public uint StartFrame { get; set; }
			public uint EndFrame { get; set; }

			public uint CalcFrame(float normalizedAnimationTime)
			{
				var frameCount = EndFrame - StartFrame;
				return StartFrame + (uint)MathF.Round(normalizedAnimationTime * frameCount);
			}
		}

		public uint SpritesPerColumn { get; set; }
		public uint SpritesPerRow { get; set; }
		public uint FirstASCII { get; set; }

		public Rectangle CalcTexCoordsFromId(uint id)
		{
			var spriteId = id;
			uint row = spriteId / SpritesPerRow;
			uint col = spriteId % SpritesPerRow;

			float centerX = (col + 0.5f) / SpritesPerRow;
			float centerY = 1.0f - (row + 0.5f) / SpritesPerColumn;
			float width = 1f / SpritesPerRow;
			float height = 1f / SpritesPerColumn;

			return Rectangle.FromCenterSize(centerX, centerY, width, height);
		}

		public Rectangle CalcTexCoordsFromAnimationTime(float normalizedAnimationTime)
		{
			var frameCount = SpritesPerRow * SpritesPerColumn - 1;
			var frame = (uint)MathF.Round(normalizedAnimationTime * frameCount);
			return CalcTexCoordsFromId(frame);
		}
	}
}
