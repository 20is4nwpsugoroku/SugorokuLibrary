using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.ClientToServer.Converters
{
	public class DiceMessageConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			var diceMessage = (DiceMessage) value!;

			writer.WriteStartObject();
			writer.WritePropertyName("matchId");
			writer.WriteValue(diceMessage.MatchId);
			writer.WritePropertyName("playerId");
			writer.WriteValue(diceMessage.PlayerId);
			writer.WritePropertyName("nowPosition");
			writer.WriteValue(diceMessage.NowPosition);
			writer.WriteEndObject();
		}

		public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
			JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);
			return new DiceMessage((string) jObject["matchId"]!, (int) jObject["playerId"]!,
				(int) jObject["nowPosition"]!);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(DiceMessage);
		}
	}
}