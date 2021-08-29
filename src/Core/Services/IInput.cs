using System.Collections.Generic;

namespace Core.Services
{
	/// <summary>
	/// Interface for input handling classes.
	/// </summary>
	public interface IInput
	{
		/// <summary>
		/// Gets all input axes names and current state.
		/// </summary>
		/// <value>
		/// Returns a list of pairs of axis name and current state [-1, 1].
		/// </value>
		IEnumerable<KeyValuePair<string, float>> Axes { get; }

		/// <summary>
		/// Returns a list of the names of all pressed buttons.
		/// </summary>
		/// <value>
		/// A list of pressed button names.
		/// </value>
		IEnumerable<string> PressedButtons { get; }

		/// <summary>
		/// Gets the state [-1, 1] of the axis <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The name of the axis.</param>
		/// <returns>The axis state in the range [-1, 1].</returns>
		float GetAxis(string name);

		/// <summary>
		/// Returns <code>true</code> if the button <paramref name="name"/> is pressed.
		/// </summary>
		/// <param name="name">The name of the button.</param>
		/// <returns><code>true</code> if the button is pressed.</returns>
		bool IsButtonDown(string name);
	}
}