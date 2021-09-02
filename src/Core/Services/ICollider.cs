namespace Core.Services
{
	internal interface ICollider : IComponent
	{
		void CollisionResponse(ICollider other);
		bool Overlaps(ICollider other);
	}
}