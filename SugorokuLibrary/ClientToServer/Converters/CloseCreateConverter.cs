using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.ClientToServer.Converters
{
	public class CloseCreateConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			var closeCreate = (CloseCreateMessage) value!;
			
			writer.WriteStartObject();
			writer.WritePropertyName("methodType");
			writer.WriteValue(closeCreate.MethodType);
			writer.WritePropertyName("matchKey");
			writer.WriteValue(closeCreate.MatchKey);
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);
			return new CloseCreateMessage((string) jObject["matchKey"]!);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(CloseCreateMessage);
		}
	}
}