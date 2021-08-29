using Core.Services;
using System;
using Zenseless.OpenTK;

namespace Core.Components
{
	internal class Collider : Component, ICollider, IDisposable
	{
		private readonly ICollisionDetection collisionDetection;

		public Collider(IGameObject gameObject, Action<IGameObject> collisionResponse, string layer) : base(gameObject)
		{
			this._collisionResponse = collisionResponse ?? throw new ArgumentNullException(nameof(collisionResponse));
			collisionDetection = Helper.CheckServiceExists(gameObject.Scene.GetService<ICollisionDetection>());
			LayerName = layer;
			collisionDetection.Add(LayerName, this);
		}

		public override IComponent CloneTo(IGameObject gameObject) => new Collider(gameObject, _collisionResponse, LayerName);

		private readonly Action<IGameObject> _collisionResponse;

		private string LayerName { get; }

		public bool Intersects(ICollider other)
		{
			return GameObject.Bounds.Intersects(other.GameObject.Bounds);
		}

		public void CollisionResponse(ICollider other)
		{
			_collisionResponse(other.GameObject);
		}

		public override void Update()
		{
		}

		public void Dispose() => collisionDetection.Remove(LayerName, this);
	}
}
