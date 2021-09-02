using System;

namespace Core.Components
{
	public static class ISceneExtensions
	{
		public static TYPE RequireService<TYPE>(this IScene scene) where TYPE : class
		{
			return scene.GetService<TYPE>() ?? throw new ArgumentException($"Instance {typeof(TYPE)} not found.");
		}
	}
}
