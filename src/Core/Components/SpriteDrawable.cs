using Core.Services;
using Zenseless.OpenTK;

namespace Core.Components
{
	internal class SpriteDrawable : Component
	{
		public SpriteDrawable(IGameObject gameObject, string textureName) : base(gameObject)
		{
			graphic = gameObject.Scene.RequireService<IGraphic>();
			this.textureName = textureName;
		}

		public override IComponent CloneTo(IGameObject gameObject) => new SpriteDrawable(gameObject, textureName);

		private readonly IGraphic graphic;
		private readonly string textureName;

		public override void Update()
		{
			var texCoords = Box2Extensions.CreateFromMinSize(0f, 0f, 1f, 1f);
			graphic.DrawSprite(textureName, GameObject.Bounds, texCoords);
		}
	}
}
