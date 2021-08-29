namespace Core
{
	public interface IComponent : IUpdate
	{
		IComponent CloneTo(IGameObject gameObject);
		IGameObject GameObject { get; }
	}
}