using Core;
using Core.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Zenseless.Resources;

namespace Example
{
	public static class Helper
	{
		internal static void LoadCollisionLayers(this ICollisionDetection collisionDetection, IResourceDirectory sceneDir)
		{
			var layerLayerCollisions = sceneDir.LoadJson<Dictionary<string, List<string>>>("collisionLayers.json");
			foreach (var layerCollisions in layerLayerCollisions)
			{
				collisionDetection.AddLayer(layerCollisions.Key);
				foreach (var layer in layerCollisions.Value)
				{
					collisionDetection.AddLayer(layer);
					collisionDetection.AddLayerToLayerCollision(layerCollisions.Key, layer);
				}
			}
		}

		internal static Dictionary<string, IGameObject> LoadPrototypes(this IScene scene, IResourceDirectory sceneDir)
		{
			var typeInfo = Assembly.GetExecutingAssembly().DefinedTypes.
				Where(t => t.ImplementedInterfaces.Contains(typeof(IComponent)) && !t.IsAbstract);
			var types = typeInfo.ToDictionary((t) => t.Name, (t) => t.AsType());

			var prototypes = new Dictionary<string, IGameObject>();
			var prototypesParams = sceneDir.LoadJson<Dictionary<string, Dictionary<string, List<string>>>>("prototypes.json");
			foreach (var prototype in prototypesParams)
			{
				var go = scene.CreateGameObject(prototype.Key);
				go.Enabled = false;
				prototypes[prototype.Key] = go;
				foreach (var component in prototype.Value)
				{
					var name = component.Key;
					var type = types[name];
					var ctor = type.GetConstructors().FirstOrDefault();
					if (ctor is null) continue;
					var parameterTypes = ctor.GetParameters();
					if (typeof(IGameObject) != parameterTypes[0].ParameterType)
					{
						throw new Exception($"First constructor for class {name} must have {nameof(IGameObject)} as first parameter to be deserialized.");
					}
					var parameterValues = new List<object> { go };
					for (int i = 1; i < parameterTypes.Length; ++i)
					{
						var parameterType = parameterTypes[i].ParameterType;
						var paramText = component.Value[i - 1];
						var param = Convert.ChangeType(paramText, parameterType, CultureInfo.InvariantCulture);
						parameterValues.Add(param);
					}
					Activator.CreateInstance(type, parameterValues.ToArray());
				}
			}
			return prototypes;
		}
	}
}
