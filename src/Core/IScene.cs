using System.Collections.Generic;

namespace Core
{
	public interface IScene
	{
		IGameObject CreateGameObject(string name = "");
		void Remove(IGameObject gameObject);
		IEnumerable<IGameObject> GetGameObjects(string name = "");
		T GetService<T>() where T : class;
	}
}