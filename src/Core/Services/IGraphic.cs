using OpenTK.Mathematics;

namespace Core.Services
{
	public interface IGraphic
	{
		void DrawRectangle(Box2 rectangle);
		void DrawSprite(string textureName, Box2 rectangle, Box2 texCoords);
		void DrawText(string textureName, Box2 firstCharacterRectangle, string text);
		Box2 TexCoordsForAnimation(string textureName, float normalizedAnimationTime);
	}
}