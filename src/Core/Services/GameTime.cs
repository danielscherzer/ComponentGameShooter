using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Core.Services
{
	public class GameTime : IGameTime
	{
		public GameTime(GameWindow gameWindow)
		{
			gameWindow.UpdateFrame += GameWindow_UpdateFrame;
		}

		public float FrameTime { get; private set; } = 1 / 60f;
		public float Scale { get; set; } = 1f;
		public float Time { get; private set; } = 0f;

		private void GameWindow_UpdateFrame(FrameEventArgs e)
		{
			FrameTime = (float)e.Time * Scale;
			Time += FrameTime;
		}
	}
}
