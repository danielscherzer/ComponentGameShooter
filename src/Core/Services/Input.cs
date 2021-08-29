using OpenTK;
using OpenTK.Input;
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
		public Input(INativeWindow window)
		{
			window.KeyDown += Window_KeyDown;
			window.KeyUp += Window_KeyUp;
			axes[Vertical] = 0f;
			axes[Horizontal] = 0f;
			// One could read key mappings from a file, registry, ...
			// here we just hard code them
			keyMappings.Add(Key.Space, Fire);
			keyMappings.Add(Key.ControlLeft, Fire);
			axisKeyMappings.Add(Key.Left, new Tuple<string, float>(Horizontal, -1f));
			axisKeyMappings.Add(Key.Right, new Tuple<string, float>(Horizontal, 1f));
			axisKeyMappings.Add(Key.Down, new Tuple<string, float>(Vertical, -1f));
			axisKeyMappings.Add(Key.Up, new Tuple<string, float>(Vertical, 1f));

			axisKeyMappings.Add(Key.A, new Tuple<string, float>(Horizontal, -1f));
			axisKeyMappings.Add(Key.D, new Tuple<string, float>(Horizontal, 1f));
			axisKeyMappings.Add(Key.S, new Tuple<string, float>(Vertical, -1f));
			axisKeyMappings.Add(Key.W, new Tuple<string, float>(Vertical, 1f));
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

		private readonly HashSet<string> pressedButtons = new HashSet<string>();
		private readonly Dictionary<string, float> axes = new Dictionary<string, float>();
		private readonly Dictionary<Key, Tuple<string, float>> axisKeyMappings = new Dictionary<Key, Tuple<string, float>>();
		private readonly Dictionary<Key, string> keyMappings = new Dictionary<Key, string>();

		private string ConvertToName(Key key)
		{
			if (keyMappings.TryGetValue(key, out var name))
			{
				return name;
			}
			return key.ToString();
		}

		private void UpdateAxes(Key key, float sign)
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

		private void Window_KeyDown(object sender, KeyboardKeyEventArgs e)
		{
			if (e.IsRepeat) return;
			pressedButtons.Add(ConvertToName(e.Key));
			UpdateAxes(e.Key, 1f);
		}

		private void Window_KeyUp(object sender, KeyboardKeyEventArgs e)
		{
			if (e.IsRepeat) return;
			pressedButtons.Remove(ConvertToName(e.Key));
			UpdateAxes(e.Key, -1f);
		}
	}
}
