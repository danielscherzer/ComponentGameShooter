using Core.Services;

namespace Core.Components
{
	internal class SpriteDrawable : Component
	{
		public SpriteDrawable(IGameObject gameObject, string textureName) : base(gameObject)
		{
			graphic = Helper.CheckServiceExists(gameObject.Scene.GetService<IGraphic>());
			this.textureName = textureName;
		}

		public override IComponent CloneTo(IGameObject gameObject) => new SpriteDrawable(gameObject, textureName);

		private readonly IGraphic graphic;
		private readonly string textureName;

		public override void Update()
		{
			var texCoords =  Rectangle.FromMinSize(0f, 0f, 1f, 1f);
			graphic.DrawSprite(textureName, GameObject.Rectangle, texCoords);
		}
	}
}
