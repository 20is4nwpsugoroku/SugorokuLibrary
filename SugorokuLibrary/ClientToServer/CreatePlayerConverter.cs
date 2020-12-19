using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.ClientToServer
{
	public class CreatePlayerConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			var createPlayer = (CreatePlayerMessage) value!;
			
			writer.WriteStartObject();
			writer.WritePropertyName("methodType");
			writer.WriteValue(createPlayer.MethodType);
			writer.WritePropertyName("playerName");
			writer.WriteValue(createPlayer.PlayerName);
			writer.WritePropertyName("matchKey");
			writer.WriteValue(createPlayer.MatchKey);
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);
			return new CreatePlayerMessage((string) jObject["playerName"]!, (string) jObject["matchKey"]!);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(CreatePlayerMessage);
		}
	}
}