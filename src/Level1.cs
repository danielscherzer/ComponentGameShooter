using Core;
using Core.Components;
using Core.Services;
using OpenTK.Mathematics;
using System;
using System.Linq;
using Zenseless.OpenTK;
using Zenseless.Resources;

namespace Example
{
	internal class Level1
	{
		internal static void Load(IScene scene)
		{
			EmbeddedResourceDirectory sceneDir = new(nameof(Example) + ".Content.Scene");
			scene.RequireService<ICollisionDetection>().LoadCollisionLayers(sceneDir);

			var prototypes = scene.LoadPrototypes(sceneDir);
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
	}
}
