using Core.Services;
using OpenTK;
using OpenTK.Mathematics;

namespace Core.Components
{
	internal class MovementBehavior : Component
	{
		public MovementBehavior(IGameObject gameObject, float velocityX, float velocityY) : this(gameObject, new Vector2(velocityX, velocityY))
		{
		}

		public MovementBehavior(IGameObject gameObject, Vector2 velocity) : base(gameObject)
		{
			gameTime = Helper.CheckServiceExists(gameObject.Scene.GetService<IGameTime>());
			Velocity = velocity;
		}

		public Vector2 Velocity { get; set; }

		public override IComponent CloneTo(IGameObject gameObject) => new MovementBehavior(gameObject, Velocity);

		public override void Update()
		{
			GameObject.Rectangle = GameObject.Rectangle.Translated(gameTime.FrameTime * Velocity);
		}

		private readonly IGameTime gameTime;
	}
}