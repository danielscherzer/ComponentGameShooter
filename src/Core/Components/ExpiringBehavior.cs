namespace Core.Components
{
	public class ExpiringBehavior : PeriodicBehavior
	{
		public ExpiringBehavior(IGameObject gameObject, float lifeSpan) : base(gameObject, lifeSpan, Remove, lifeSpan)
		{
		}

		private static bool Remove(IGameObject go)
		{
			go.Scene.Remove(go);
			return false;
		}
	}
}
