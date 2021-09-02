using Core.Services;

namespace Core.Components
{
	internal class TextDrawable : Component
	{
		public TextDrawable(IGameObject gameObject, string textureName) : base(gameObject)
		{
			graphic = gameObject.Scene.RequireService<IGraphic>();
			this.textureName = textureName;
		}

		public override IComponent CloneTo(IGameObject gameObject)
		{
			var td = new TextDrawable(gameObject, textureName)
			{
				Text = Text
			};
			return td;
		}

		private readonly string textureName;
		private readonly IGraphic graphic;
		public string Text { get; set; } = string.Empty;

		public override void Update()
		{
			graphic.DrawText(textureName, GameObject.Bounds, Text);
		}
	}
}
