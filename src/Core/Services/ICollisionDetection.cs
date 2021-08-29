namespace Core.Services
{
	internal interface ICollisionDetection
	{
		void Add(string layer, ICollider collider);
		bool AddLayer(string name);
		bool AddLayerToLayerCollision(string layer1, string layer2);
	}
}