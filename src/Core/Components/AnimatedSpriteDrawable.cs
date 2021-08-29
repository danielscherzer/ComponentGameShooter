using Core.Services;
using System;

namespace Core.Components
{
	internal class AnimatedSpriteDrawable : Component
	{
		public AnimatedSpriteDrawable(IGameObject gameObject, string textureName, float animationLength, bool looped) : base(gameObject)
		{
			graphic = Helper.CheckServiceExists(gameObject.Scene.GetService<IGraphic>());
			time = Helper.CheckServiceExists(gameObject.Scene.GetService<IGameTime>());
			this.textureName = textureName;
			this.animationLength = animationLength;
			this.looped = looped;
		}

		public override IComponent CloneTo(IGameObject gameObject) => new AnimatedSpriteDrawable(gameObject, textureName, animationLength, looped);

		private readonly IGameTime time;
		private readonly string textureName;
		private readonly float animationLength;
		private readonly bool looped;
		private float normalizedAnimationTime = 0f;
		private readonly IGraphic graphic;

		public override void Update()
		{
			normalizedAnimationTime += time.FrameTime / animationLength;
			normalizedAnimationTime = looped ? normalizedAnimationTime % 1f : MathF.Min(normalizedAnimationTime, 1f);
			var texCoords = graphic.TexCoordsForAnimation(textureName, normalizedAnimationTime);
			graphic.DrawSprite(textureName, GameObject.Rectangle, texCoords);
		}
	}
}
