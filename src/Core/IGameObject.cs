using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Core
{
	public interface IGameObject : IUpdate, IDisposable
	{
		bool Enabled { get; set; }
		string Name { get; }
		Box2 Bounds { get; set; }
		IScene Scene { get; }
		void AddComponent(IComponent component);
		IGameObject Clone();
		IEnumerable<T> GetComponents<T>() where T : class, IComponent;
	}
}