using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.ServerToClient.Converters
{
	public class MatchMapImageMessageConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			var message = (MatchMapImageMessage) value!;
			
			writer.WriteStartObject();
			writer.WritePropertyName("methodType");
			writer.WriteValue(message.MethodType);
			writer.WritePropertyName("imageBase64");
			writer.WriteValue(message.ImageBase64);
			writer.WriteEndObject();
		}

		public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);

			return new MatchMapImageMessage((string) jObject["imageBase64"]!);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(MatchMapImageMessage);
		}
	}
}