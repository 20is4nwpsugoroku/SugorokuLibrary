using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.ClientToServer.Converters
{
	public class GetMatchInfoConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			var createPlayer = (GetMatchInfoMessage) value!;
			
			writer.WriteStartObject();
			writer.WritePropertyName("methodType");
			writer.WriteValue(createPlayer.MethodType);
			writer.WritePropertyName("matchKey");
			writer.WriteValue(createPlayer.MatchKey);
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);
			return new GetMatchInfoMessage((string) jObject["matchKey"]!);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(GetMatchInfoMessage);
		}
	}
}