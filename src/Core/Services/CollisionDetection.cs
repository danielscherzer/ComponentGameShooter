using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core.Services
{
	/// <summary>
	/// Class that detects collisions
	/// </summary>
	internal class CollisionDetection : IUpdate, ICollisionDetection
	{
		public bool Add(string layer, ICollider collider)
		{
			return _layerColliders[layer].Add(collider);
		}

		public bool AddLayer(string name)
		{
			if (!_layerLayerCollisions.ContainsKey(name))
			{
				var collidingLayers = new HashSet<string>();
				_layerLayerCollisions.Add(name, collidingLayers);
				_layerColliders[name] = new HashSet<ICollider>();
				Trace.WriteLine($"Add Collision layer: {name}");
				return true;
			}
			return false;
		}

		public bool AddLayerToLayerCollision(string layer1, string layer2)
		{
			if (!_layerLayerCollisions.TryGetValue(layer1, out var collidingLayers))
			{
				throw new ArgumentException($"Layer {layer1} not found");
			}
			if (!_layerLayerCollisions.ContainsKey(layer2))
			{
				throw new ArgumentException($"Layer {layer2} not found");
			}
			return collidingLayers.Add(layer2);
		}

		public void Update()
		{
			foreach (var layer1 in _layerLayerCollisions)
			{
				foreach (var layer2 in layer1.Value)
				{
					HandleCollisions(_layerColliders[layer1.Key], _layerColliders[layer2]);
				}
			}
			//Trace.WriteLine($"Collider count:{_layerColliders.Values.Sum(colls => colls.Count)}");
		}

		public bool Remove(string layer, ICollider collider)
		{
			return _layerColliders[layer].Remove(collider);
		}

		private static void HandleCollisions(IEnumerable<ICollider> layer1Colliders, IEnumerable<ICollider> layer2Colliders)
		{
			foreach (var collider1 in layer1Colliders)
			{
				foreach (var collider2 in layer2Colliders)
				{
					if (collider1 == collider2) continue;
					if (collider1.Intersects(collider2))
					{
						Trace.WriteLine($"collision between {collider1.GameObject.Name} and {collider2.GameObject.Name}");
						collider1.CollisionResponse(collider2);
						collider2.CollisionResponse(collider1);
					}
				}
			}
		}

		private readonly Dictionary<string, HashSet<ICollider>> _layerColliders = new();
		private readonly Dictionary<string, HashSet<string>> _layerLayerCollisions = new();
	}
}
