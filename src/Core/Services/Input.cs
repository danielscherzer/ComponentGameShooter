using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;

namespace Core.Services
{
	/// <summary>
	/// Handles Input from Keyboard, 
	/// </summary>
	/// <seealso cref="IInput" />
	public class Input : IInput
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Input"/> class.
		/// </summary>
		/// <param name="window">The window.</param>
		public Input(NativeWindow window)
		{
			window.KeyDown += Window_KeyDown;
			window.KeyUp += Window_KeyUp;
			axes[Vertical] = 0f;
			axes[Horizontal] = 0f;
			// One could read key mappings from a file, registry, ...
			// here we just hard code them
			keyMappings.Add(Keys.Space, Fire);
			keyMappings.Add(Keys.LeftControl, Fire);
			axisKeyMappings.Add(Keys.Left, new Tuple<string, float>(Horizontal, -1f));
			axisKeyMappings.Add(Keys.Right, new Tuple<string, float>(Horizontal, 1f));
			axisKeyMappings.Add(Keys.Down, new Tuple<string, float>(Vertical, -1f));
			axisKeyMappings.Add(Keys.Up, new Tuple<string, float>(Vertical, 1f));

			axisKeyMappings.Add(Keys.A, new Tuple<string, float>(Horizontal, -1f));
			axisKeyMappings.Add(Keys.D, new Tuple<string, float>(Horizontal, 1f));
			axisKeyMappings.Add(Keys.S, new Tuple<string, float>(Vertical, -1f));
			axisKeyMappings.Add(Keys.W, new Tuple<string, float>(Vertical, 1f));
			// of course one would augment this class with pointing device input (mouse) etc.
			// buttons could also be accessed via IsButtonDown
			// an additional function that returns the current position of the pointing device would still be needed
		}

		/// <summary>
		/// Gets the state [-1, 1] of the axis <paramref name="name" />.
		/// </summary>
		/// <param name="name">The name of the axis.</param>
		/// <returns>
		/// The axis state in the range [-1, 1].
		/// </returns>
		public float GetAxis(string name)
		{
			return axes[name];
		}

		/// <summary>
		/// Returns <code>true</code> if the button <paramref name="name" /> is pressed.
		/// </summary>
		/// <param name="name">The name of the button.</param>
		/// <returns>
		///   <code>true</code> if the button is pressed.
		/// </returns>
		public bool IsButtonDown(string name)
		{
			return pressedButtons.Contains(name);
		}

		/// <summary>
		/// Returns a list of the names of all pressed buttons.
		/// </summary>
		/// <value>
		/// A list of pressed button names.
		/// </value>
		public IEnumerable<string> PressedButtons => pressedButtons;

		/// <summary>
		/// Gets all input axes names and current state.
		/// </summary>
		/// <value>
		/// Returns a list of pairs of axis name and current state [-1, 1].
		/// </value>
		public IEnumerable<KeyValuePair<string, float>> Axes => axes;

		private const string Vertical = nameof(Vertical);
		private const string Horizontal = nameof(Horizontal);
		private const string Fire = nameof(Fire);

		private readonly HashSet<string> pressedButtons = new();
		private readonly Dictionary<string, float> axes = new();
		private readonly Dictionary<Keys, Tuple<string, float>> axisKeyMappings = new();
		private readonly Dictionary<Keys, string> keyMappings = new();

		private string ConvertToName(Keys key)
		{
			if (keyMappings.TryGetValue(key, out var name))
			{
				return name;
			}
			return key.ToString();
		}

		private void UpdateAxes(Keys key, float sign)
		{
			void UpdateAxes(string axis, float value)
			{
				axes[axis] = MathHelper.Clamp(axes[axis] + value, -1f, 1f);
			}
			if (axisKeyMappings.TryGetValue(key, out var data))
			{
				UpdateAxes(data.Item1, sign * data.Item2);
			}
		}

		private void Window_KeyDown(KeyboardKeyEventArgs e)
		{
			if (e.IsRepeat) return;
			pressedButtons.Add(ConvertToName(e.Key));
			UpdateAxes(e.Key, 1f);
		}

		private void Window_KeyUp(KeyboardKeyEventArgs e)
		{
			if (e.IsRepeat) return;
			pressedButtons.Remove(ConvertToName(e.Key));
			UpdateAxes(e.Key, -1f);
		}
	}
}
