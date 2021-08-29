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
		public void Add(string layer, ICollider collider)
		{
			layerColliders[layer].Add(collider);
		}

		public bool AddLayer(string name)
		{
			if (!layerLayerCollisions.ContainsKey(name))
			{
				var collidingLayers = new HashSet<string>();
				layerLayerCollisions.Add(name, collidingLayers);
				layerColliders[name] = new HashSet<ICollider>();
				return true;
			}
			return false;
		}

		public bool AddLayerToLayerCollision(string layer1, string layer2)
		{
			if (!layerLayerCollisions.TryGetValue(layer1, out var collidingLayers))
			{
				throw new ArgumentException($"Layer {layer1} not found");
			}
			if (!layerLayerCollisions.ContainsKey(layer2))
			{
				throw new ArgumentException($"Layer {layer2} not found");
			}
			return collidingLayers.Add(layer2);
		}

		public void Update()
		{
			foreach (var layer1 in layerLayerCollisions)
			{
				foreach (var layer2 in layer1.Value)
				{
					HandleCollisions(layerColliders[layer1.Key], layerColliders[layer2]);
				}
			}

			foreach (var layer in layerColliders.Values)
			{
				layer.Clear();
			}
		}

		private void HandleCollisions(IEnumerable<ICollider> layer1Colliders, IEnumerable<ICollider> layer2Colliders)
		{
			foreach (var collider1 in layer1Colliders)
			{
				foreach (var collider2 in layer2Colliders)
				{
					if (collider1 == collider2) continue;
					if (collider1.Intersects(collider2))
					{
						Trace.WriteLine($"collision between {collider1.GameObject.Name} and {collider2.GameObject.Name}");
						collider1.HandleCollision(collider2);
						collider2.HandleCollision(collider1);
					}
				}
			}
		}

		private readonly Dictionary<string, HashSet<ICollider>> layerColliders = new Dictionary<string, HashSet<ICollider>>();
		private readonly Dictionary<string, HashSet<string>> layerLayerCollisions = new Dictionary<string, HashSet<string>>();
	}
}
