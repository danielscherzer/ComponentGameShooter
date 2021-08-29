namespace Core.Services
{
	internal interface ICollider : IComponent
	{
		void HandleCollision(ICollider other);
		bool Intersects(ICollider other);
	}
}