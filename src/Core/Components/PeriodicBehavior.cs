using Core.Services;
using System;

namespace Core.Components
{
	public class PeriodicBehavior : Component
	{
		public float PeriodSpan { get; }

		private readonly Func<IGameObject, bool> periodicActionReset;
		private readonly IGameTime gameTime;
		private float coolDown;

		public PeriodicBehavior(IGameObject gameObject, float periodSpan, Func<IGameObject, bool> periodicActionReset, float initialPeriodSpan = -1) : base(gameObject)
		{
			this.periodicActionReset = periodicActionReset ?? throw new ArgumentNullException(nameof(periodicActionReset));
			gameTime = gameObject.Scene.RequireService<IGameTime>();
			PeriodSpan = periodSpan;
			coolDown = initialPeriodSpan;
		}

		public override IComponent CloneTo(IGameObject gameObject) => new PeriodicBehavior(gameObject, PeriodSpan, periodicActionReset, coolDown);

		public override void Update()
		{
			if (coolDown < 0)
			{
				if (periodicActionReset(GameObject)) coolDown = PeriodSpan;
			}
			coolDown -= gameTime.FrameTime;
		}
	}
}