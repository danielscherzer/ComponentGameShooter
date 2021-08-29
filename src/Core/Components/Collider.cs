using Core.Services;
using System;
using Zenseless.OpenTK;

namespace Core.Components
{
	internal class Collider : Component, ICollider
	{
		private readonly ICollisionDetection collisionDetection;

		public Collider(IGameObject gameObject, Action<IGameObject> handleCollision, string layer) : base(gameObject)
		{
			this.handleCollision = handleCollision ?? throw new ArgumentNullException(nameof(handleCollision));
			collisionDetection = Helper.CheckServiceExists(gameObject.Scene.GetService<ICollisionDetection>());
			LayerName = layer;
		}

		public override IComponent CloneTo(IGameObject gameObject) => new Collider(gameObject, handleCollision, LayerName);

		private readonly Action<IGameObject> handleCollision;

		private string LayerName { get; }

		public bool Intersects(ICollider other)
		{
			return GameObject.Bounds.Intersects(other.GameObject.Bounds);
		}

		public void HandleCollision(ICollider other)
		{
			handleCollision(other.GameObject);
		}

		public override void Update()
		{
			collisionDetection.Add(LayerName, this);
		}
	}
}
