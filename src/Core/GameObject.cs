using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Diagnostics;
using Zenseless.OpenTK;

namespace Core
{
	/// <summary>
	/// Base class for all game objects.
	/// </summary>
	[DebuggerDisplay("{Name}")]
	public class GameObject : IGameObject
	{
		public GameObject(IScene scene, string name = "")
		{
			Scene = scene;
			Name = name;
		}

		public bool Enabled { get; set; } = true;
		public string Name { get; }
		public Box2 Bounds { get; set; } = Box2Extensions.CreateFromCenterSize(0.0f, 0.0f, 1f, 1f);
		public IScene Scene { get; }

		public void AddComponent(IComponent component)
		{
			components.Add(component);
		}

		public IGameObject Clone()
		{
			var go = Scene.CreateGameObject(Name);
			go.Bounds = Bounds;
			foreach (var component in components)
			{
				component.CloneTo(go);
			}
			return go;
		}

		public IEnumerable<T> GetComponents<T>() where T : class, IComponent
		{
			foreach (var component in components)
			{
				if (!(component is T typed)) continue;
				yield return typed;
			}
		}

		public void Update()
		{
			if (!Enabled) return;
			foreach (var component in components)
			{
				component.Update();
			}
		}

		private readonly List<IComponent> components = new List<IComponent>();
	}
}
