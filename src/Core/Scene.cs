using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core
{
	internal class Scene : IScene
	{
		public void AddService<TYPE>(TYPE service) where TYPE : class
		{
			var type = typeof(TYPE);
			Trace.WriteLineIf(_services.ContainsKey(type), $"Overwriting service {type}.");
			_services[type] = service; // allow overwriting of services for decoration (like logging)
			if (service is IUpdate updatable) _updateServices.Add(updatable);
		}

		public IGameObject CreateGameObject(string name = "")
		{
			Trace.WriteLine($"request spawn '{name}'");
			var go = new GameObject(this, name);
			// do not directly add to gameObjects because we could be in the process of iterating over it
			_spawnedGameObjects.Add(go);
			return go;
		}

		public T GetService<T>() where T : class
		{
			if (_services.TryGetValue(typeof(T), out var service))
			{
				return service as T;
			}
			return null;
		}

		public IEnumerable<IGameObject> GetGameObjects(string name = "")
		{
			foreach (var gameObject in _gameObjects)
			{
				if (!string.IsNullOrEmpty(name) && name != gameObject.Name) continue;
				yield return gameObject;
			}
		}

		public void Remove(IGameObject gameObject)
		{
			Trace.WriteLine($"request remove '{gameObject.Name}'");
			// do not directly remove from gameObjects because we could be in the process of iterating over it
			gameObject.Dispose();
			_removeGameObjects.Add(gameObject);
		}

		public void Update()
		{
			foreach (var updatable in _updateServices)
			{
				updatable.Update();
			}
			ResolveRemovesAndSpawns();
			foreach (var obj in _gameObjects)
			{
				obj.Update();
			}
			//Do not update spawned game objects because those may again spawn game objects!
			ResolveRemovesAndSpawns();
		}

		private void ResolveRemovesAndSpawns()
		{
			foreach (var obj in _removeGameObjects)
			{
				_gameObjects.Remove(obj);
			}
			_removeGameObjects.Clear();
			_gameObjects.UnionWith(_spawnedGameObjects);
			_spawnedGameObjects.Clear();
		}

		private readonly HashSet<IGameObject> _gameObjects = new();
		private readonly List<IGameObject> _removeGameObjects = new();
		private readonly List<IGameObject> _spawnedGameObjects = new();
		private readonly Dictionary<Type, object> _services = new();
		private readonly List<IUpdate> _updateServices = new();
	}
}
