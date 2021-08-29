namespace Core.Services
{
	internal interface ICollider : IComponent
	{
		void CollisionResponse(ICollider other);
		bool Intersects(ICollider other);
	}
}