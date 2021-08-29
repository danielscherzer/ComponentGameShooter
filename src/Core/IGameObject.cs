using System.Collections.Generic;

namespace Core
{
	public interface IGameObject : IUpdate
	{
		bool Enabled { get; set; }
		string Name { get; }
		Rectangle Rectangle { get; set; }
		IScene Scene { get; }
		void AddComponent(IComponent component);
		IGameObject Clone();
		IEnumerable<T> GetComponents<T>() where T : class, IComponent;
	}
}