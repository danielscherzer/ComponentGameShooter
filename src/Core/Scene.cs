using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core
{
	internal class Scene : IScene
	{
		//public IEnumerable<IGameObject> GameObjects => gameObjects;

		public void AddService<TYPE>(TYPE service) where TYPE : class
		{
			var type = typeof(TYPE);
			Trace.WriteLineIf(services.ContainsKey(type), $"Overwriting service {type}.");
			services[type] = service; // allow overwriting of services for decoration (like logging)
			if (service is IUpdate updatable) updateServices.Add(updatable);
		}

		public IGameObject CreateGameObject(string name = "")
		{
			Trace.WriteLine($"request spawn '{name}'");
			var go = new GameObject(this, name);
			// do not directly add to gameObjects because we could be in the process of iterating over it
			spawnedGameObjects.Add(go);
			return go;
		}

		public T GetService<T>() where T : class
		{
			if (services.TryGetValue(typeof(T), out var service))
			{
				return service as T;
			}
			return null;
		}

		//public IEnumerable<Type> GetServices() => services.Keys;

		public IEnumerable<IGameObject> GetGameObjects(string name = "")
		{
			foreach (var gameObject in gameObjects)
			{
				if (!string.IsNullOrEmpty(name) && name != gameObject.Name) continue;
				yield return gameObject;
			}
		}

		public void Remove(IGameObject gameObject)
		{
			Trace.WriteLine($"request remove '{gameObject.Name}'");
			if (gameObject is not GameObject go) return;
			// do not directly remove from gameObjects because we could be in the process of iterating over it
			removeGameObjects.Add(go);
			go.Enabled = false;
		}

		public void Update()
		{
			foreach (var updatable in updateServices)
			{
				updatable.Update();
			}
			ResolveRemovesAndSpawns();
			foreach (var obj in gameObjects)
			{
				obj.Update();
			}
			//Do not update spawned game objects because those may again spawn game objects!
			ResolveRemovesAndSpawns();
		}

		private void ResolveRemovesAndSpawns()
		{
			foreach (var obj in removeGameObjects)
			{
				gameObjects.Remove(obj);
			}
			removeGameObjects.Clear();
			gameObjects.UnionWith(spawnedGameObjects);
			spawnedGameObjects.Clear();
		}

		private readonly HashSet<GameObject> gameObjects = new HashSet<GameObject>();
		private readonly List<GameObject> removeGameObjects = new List<GameObject>();
		private readonly List<GameObject> spawnedGameObjects = new List<GameObject>();
		private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
		private readonly List<IUpdate> updateServices = new List<IUpdate>();
	}
}
