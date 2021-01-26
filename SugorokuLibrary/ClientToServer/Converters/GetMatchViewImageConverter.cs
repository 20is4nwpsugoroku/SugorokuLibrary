using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.ClientToServer.Converters
{
	public class GetMatchViewImageMessageConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			var message = (GetMatchViewImageMessage) value!;
			
			writer.WriteStartObject();
			writer.WritePropertyName("methodType");
			writer.WriteValue(message.MethodType);
			writer.WritePropertyName("matchKey");
			writer.WriteValue(message.MatchKey);
			writer.WriteEndObject();
		}

		public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);

			return new GetMatchViewImageMessage((string) jObject["matchKey"]!);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(GetMatchViewImageMessage);
		}
	}
}