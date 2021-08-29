namespace Core.Services
{
	public interface IGraphic
	{
		void DrawRectangle(Rectangle rectangle);
		void DrawSprite(string textureName, Rectangle rectangle, Rectangle texCoords);
		void DrawText(string textureName, Rectangle firstCharacterRectangle, string text);
		Rectangle TexCoordsForAnimation(string textureName, float normalizedAnimationTime);
	}
}