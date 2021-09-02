using Core.Services;

namespace Core.Components
{
	internal class ColliderDrawable : Component
	{
		public ColliderDrawable(IGameObject gameObject) : base(gameObject)
		{
			graphic = gameObject.Scene.RequireService<IGraphic>();
		}
		public override IComponent CloneTo(IGameObject gameObject) => new ColliderDrawable(gameObject);

		private readonly IGraphic graphic;

		public override void Update()
		{
			graphic.DrawRectangle(GameObject.Bounds);
		}
	}
}
