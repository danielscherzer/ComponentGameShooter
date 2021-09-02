namespace Core.Components
{
	public abstract class Component : IComponent
	{
		protected Component(IGameObject gameObject)
		{
			GameObject = gameObject;
			gameObject.AddComponent(this);
		}

		public IGameObject GameObject { get; }

		public abstract IComponent CloneTo(IGameObject gameObject);

		public abstract void Update();
	}
}
