using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Zenseless.OpenTK;
using Zenseless.Patterns;

namespace Core
{
	/// <summary>
	/// Base class for all game objects.
	/// </summary>
	[DebuggerDisplay("{Name}")]
	public class GameObject : Disposable, IGameObject
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

		public void AddComponent(IComponent component) => _components.Add(component);

		public IGameObject Clone()
		{
			var go = Scene.CreateGameObject(Name);
			go.Bounds = Bounds;
			foreach (var component in _components)
			{
				component.CloneTo(go);
			}
			return go;
		}

		public IEnumerable<T> GetComponents<T>() where T : class, IComponent
		{
			foreach (var component in _components.OfType<T>())
			{
				yield return component;
			}
		}

		public void Update()
		{
			if (!Enabled) return;
			foreach (var component in _components)
			{
				component.Update();
			}
		}

		protected override void DisposeResources()
		{
			Enabled = false;
			DisposeAllFields(this);
			//_components.Clear(); //iteration problem
		}

		private readonly List<IComponent> _components = new();
	}
}
