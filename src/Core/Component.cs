using System;

namespace Core
{
	public abstract class Component : IComponent
	{
		protected Component(IGameObject gameObject)
		{
			GameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
			gameObject.AddComponent(this);
		}

		public IGameObject GameObject { get; }

		public abstract IComponent CloneTo(IGameObject gameObject);

		public abstract void Update();
	}
}
