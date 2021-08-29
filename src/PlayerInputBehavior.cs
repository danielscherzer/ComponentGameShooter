using Core;
using Core.Components;
using Core.Services;
using OpenTK.Mathematics;

namespace Example
{
	internal class PlayerInputBehavior : Component
	{
		private readonly IInput input;
		private readonly IGameTime gameTime;

		public PlayerInputBehavior(IGameObject gameObject) : base(gameObject)
		{
			input = Helper.CheckServiceExists(gameObject.Scene.GetService<IInput>());
			gameTime = Helper.CheckServiceExists(gameObject.Scene.GetService<IGameTime>());
		}

		public bool Fire => input.IsButtonDown("Fire");

		public override IComponent CloneTo(IGameObject gameObject) => new PlayerInputBehavior(gameObject);

		public override void Update()
		{
			var axisX = input.GetAxis("Horizontal");
			var axisY = input.GetAxis("Vertical");
			var velocity = 0.6f * new Vector2(axisX, axisY);
			GameObject.Bounds = GameObject.Bounds.Translated(gameTime.FrameTime * velocity);
		}
	}
}