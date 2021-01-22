using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.ClientToServer.Converters
{
	public class GetStartedMatchConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			var getStartedMatch = (GetStartedMatchMessage) value!;
			
			writer.WriteStartObject();
			writer.WritePropertyName("methodType");
			writer.WriteValue(GetStartedMatchMessage.MethodType);
			writer.WritePropertyName("matchKey");
			writer.WriteValue(getStartedMatch.MatchKey);
			writer.WriteEndObject();
		}

		public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);
			return new GetMatchInfoMessage((string) jObject["matchKey"]!);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(GetStartedMatchMessage);
		}
	}
}