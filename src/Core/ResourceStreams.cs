using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Core
{
	/// <summary>
	/// Abstracts all resources into streams; Manages file handling internally;
	/// </summary>
	internal class ResourceStreams
	{
		/// <summary>
		/// Searches all files inside a given directory <paramref name="directory"/>, 
		/// opens each and calls the <paramref name="fileHandler"/>.
		/// </summary>
		/// <param name="directory">The directory name.</param>
		/// <param name="fileHandler">The given method is called for each found file.</param>
		/// <exception cref="ArgumentNullException">namedStreamHandler</exception>
		/// <exception cref="ArgumentException"></exception>
		public static void IterateOverFiles(string directory, Action<string, Stream> fileHandler)
		{
			if (fileHandler == null) throw new ArgumentNullException(nameof(fileHandler));

			var resourceDir = Path.Combine(GetSourceDirectory(), directory);
			if (!Directory.Exists(resourceDir)) throw new ArgumentException($"{resourceDir} was not found!");
			foreach (var file in Directory.EnumerateFiles(resourceDir))
			{
				using (var stream = File.OpenRead(file))
				{
					fileHandler(Path.GetFileName(file), stream);
				}
			}
		}

		public static Stream OpenStream(string directory, string name)
		{
			var resourceDir = Path.Combine(GetSourceDirectory(), directory);
			if (!Directory.Exists(resourceDir)) throw new ArgumentException($"{resourceDir} was not found!");
			var matchingFile = Directory.EnumerateFiles(resourceDir, $"{name}.*").FirstOrDefault();
			return matchingFile is null ? null : File.OpenRead(matchingFile);
		}

		private static string GetSourceDirectory([CallerFilePath] string sourcePath = "")
		{
			return Path.Combine(Path.GetDirectoryName(sourcePath), "..");
		}
	}
}
