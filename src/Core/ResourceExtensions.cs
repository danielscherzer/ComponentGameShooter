using Newtonsoft.Json;
using OpenTK.Mathematics;
using System;
using Zenseless.OpenTK;
using Zenseless.Resources;

namespace Core
{
	internal static class ResourceExtensions
	{
		internal static Type LoadJson<Type>(this IResourceDirectory resourceDirectory, string name)
		{
			var text = resourceDirectory.Resource(name).AsString();
			return JsonConvert.DeserializeObject<Type>(text) ?? throw new ArgumentException($"Could not deserialize '{name}'");
		}
		//var converter = new JsonConverter[] { new ConvertRectangle() };

		private class ConvertRectangle : JsonConverter<Box2>
		{
			public override Box2 ReadJson(JsonReader reader, Type objectType, Box2 existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				float ReadFloat() => (float)(reader.ReadAsDouble() ?? throw new JsonSerializationException("Could not convert Box2"));
				var minX = ReadFloat();
				var minY = ReadFloat();
				var sizeX = ReadFloat();
				var sizeY = ReadFloat();
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
