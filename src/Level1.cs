using Core;
using Core.Components;
using Core.Services;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Zenseless.OpenTK;
using Zenseless.Resources;

namespace Example
{
	internal class Level1
	{
		internal static void Load(IScene scene)
		{
			var sceneDir = new EmbeddedResourceDirectory(nameof(Example) + ".Scene");
			var collisionDetection = scene.GetService<ICollisionDetection>();
			using (var stream = sceneDir.Open("collisionLayers.json"))
			{
				using var reader = new StreamReader(stream);
				var text = reader.ReadToEnd();
				var layerLayerCollisions = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(text);
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

			var typeInfo = Assembly.GetExecutingAssembly().DefinedTypes.
				Where(t => t.ImplementedInterfaces.Contains(typeof(IComponent)) && !t.IsAbstract);
			var types = typeInfo.ToDictionary((t) => t.Name, (t) => t.AsType());

			var prototypes = new Dictionary<string, IGameObject>();
			//var converter = new JsonConverter[] { new ConvertRectangle() };
			using (var stream = sceneDir.Open("prototypes.json"))
			{
				using var reader = new StreamReader(stream);
				var text = reader.ReadToEnd();
				var prototypesParams = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<string>>>>(text);
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
			}

			var score = prototypes["Score"].Clone();
			score.Bounds = Box2Extensions.CreateFromMinSize(-0.9f, -0.9f, 0.05f, 0.05f);
			var textDrawable = score.GetComponents<TextDrawable>().First();
			var points = 0;
			AddPoints(0);
			void AddPoints(int newPoints)
			{
				points += newPoints;
				textDrawable.Text = $"points={points}";
			}

			void CreateExplosion(Box2 rectangle)
			{
				var go = prototypes["Explosion"].Clone();
				go.Bounds = rectangle;
			}

			void CreatePlayer(float centerX, float centerY, float width)
			{
				var gameObject = prototypes["Player"].Clone();
				gameObject.Bounds = Box2Extensions.CreateFromCenterSize(centerX, centerY, width, 1.2f * width);
				var playerInput = gameObject.GetComponents<PlayerInputBehavior>().First();

				IGameObject CreatePlayerLaser()
				{
					var laser = prototypes["PlayerLaser"].Clone();
					new Collider(laser, (collider) => scene.Remove(laser), GameLayer.PlayerLaser);
					return laser;
				}

				bool AllowSpawnLaser(IGameObject go)
				{
					void PositionLaser(float x)
					{
						var laserHeight = 0.08f;
						var laser = CreatePlayerLaser();
						laser.Bounds = Box2Extensions.CreateFromCenterSize(x, go.Bounds.Max.Y + 0.25f * laserHeight
							, 0.6f * laserHeight, laserHeight);
					}

					var result = playerInput.Fire;
					if (result)
					{
						PositionLaser(go.Bounds.Center.X - 0.3f * go.Bounds.Size.X);
						PositionLaser(go.Bounds.Center.X + 0.3f * go.Bounds.Size.X);
						return true;
					}
					return false;
				}

				new PeriodicBehavior(gameObject, 0.3f, AllowSpawnLaser);
				void HandleCollision(IGameObject other)
				{
					gameObject.Scene.Remove(gameObject);
					CreateExplosion(gameObject.Bounds);
				}
				new Collider(gameObject, HandleCollision, GameLayer.Player);
			}

			void CreateEnemySpawner(Action<int> addPoints)
			{
				IGameObject CreateEnemy(float centerX, float centerY, float width, Vector2 velocity)
				{
					var gameObject = prototypes["Enemy"].Clone();
					gameObject.Bounds = Box2Extensions.CreateFromCenterSize(centerX, centerY, width, 1.2f * width);

					bool SpawnLaser(IGameObject enemy)
					{
						var laserHeight = 0.04f;
						var laser = prototypes["EnemyLaser"].Clone();
						new Collider(laser, (collider) => scene.Remove(laser), GameLayer.EnemyLaser);
						laser.Bounds = Box2Extensions.CreateFromCenterSize(enemy.Bounds.Center.X, enemy.Bounds.Min.Y - 0.5f * laserHeight, 0.6f * laserHeight, laserHeight);
						return true;
					}
					new PeriodicBehavior(gameObject, 1.5f, SpawnLaser, 3f);
					new MovementBehavior(gameObject, velocity);

					void HandleCollision(IGameObject other)
					{
						if (!gameObject.Enabled) return;
						addPoints(1);
						gameObject.Scene.Remove(gameObject);
						CreateExplosion(gameObject.Bounds);
					}

					new Collider(gameObject, HandleCollision, GameLayer.Enemies);
					return gameObject;
				}

				bool left = false;
				bool SpawnEnemy(IGameObject spawner)
				{
					var x = left ? -0.9f : 0.9f;
					left = !left;
					var velocity = new Vector2(-Math.Sign(x) * 0.1f, -10f / 60f);
					var enemy = CreateEnemy(x, 0.9f, 0.1f, velocity);
					return true;
				}

				var go = scene.CreateGameObject("EnemySpawner");
				new PeriodicBehavior(go, 1f, SpawnEnemy);
				new ExpiringBehavior(go, 15f);
			}

			CreateEnemySpawner(AddPoints);
			CreatePlayer(0f, 0f, 0.1f);

			prototypes["Goblin"].Clone();
			prototypes["Background"].Clone();
		}

		private class ConvertRectangle : JsonConverter<Box2>
		{
			public override Box2 ReadJson(JsonReader reader, Type objectType, Box2 existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				var minX = (float)reader.ReadAsDouble();
				var minY = (float)reader.ReadAsDouble();
				var sizeX = (float)reader.ReadAsDouble();
				var sizeY = (float)reader.ReadAsDouble();
				reader.Read();
				return Box2Extensions.CreateFromMinSize(minX, minY, sizeX, sizeY);
			}

			public override void WriteJson(JsonWriter writer, Box2 value, JsonSerializer serializer)
			{
				writer.WriteStartArray();
				writer.WriteValue(value.Min.X);
				writer.WriteValue(value.Min.Y);
				writer.WriteValue(value.Size.X);
				writer.WriteValue(value.Size.Y);
				writer.WriteEndArray();
			}
		}

	}
}
