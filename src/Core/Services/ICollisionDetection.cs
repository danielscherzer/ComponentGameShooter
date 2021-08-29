namespace Core.Services
{
	internal interface ICollisionDetection
	{
		bool Add(string layer, ICollider collider);
		bool AddLayer(string name);
		bool AddLayerToLayerCollision(string layer1, string layer2);
		bool Remove(string layer, ICollider collider);
	}
}