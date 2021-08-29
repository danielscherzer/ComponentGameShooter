using System;
using System.Collections.Generic;
using System.Diagnostics;
using Zenseless.Patterns;

namespace Core
{
	internal class Scene : IScene
	{
		public void AddService<TYPE>(TYPE service) where TYPE : class
		{
			Trace.WriteLineIf(_services.Contains<TYPE>(), $"Overwriting service {typeof(TYPE)}.");
			_services.RegisterTypeInstance(service);
			if (service is IUpdate updatable) _updateServices.Add(updatable);
		}

		public IGameObject CreateGameObject(string name = "")
		{
			Trace.WriteLine($"request spawn '{name}'");
			var go = new GameObject(this, name);
			// do not directly add to gameObjects because we could be in the process of iterating over it
			_gameObjects.Add(go);
			return go;
		}

		public T? GetService<T>() where T : class => _services.GetInstance<T>();

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
			_gameObjects.Remove(gameObject);
		}

		public void Update()
		{
			foreach (var updatable in _updateServices)
			{
				updatable.Update();
			}
			foreach (var obj in _gameObjects)
			{
				obj.Update();
			}
		}

		private readonly LazyMutableHashSet<IGameObject> _gameObjects = new();
		private readonly TypeRegistry _services = new();
		private readonly List<IUpdate> _updateServices = new();
	}
}
