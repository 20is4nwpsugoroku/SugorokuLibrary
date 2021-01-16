using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.ServerToClient.Converters
{
	public class DiceResultMessageConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			var diceResult = (DiceResultMessage) value!;

			writer.WriteStartObject();
			writer.WritePropertyName("methodType");
			writer.WriteValue(diceResult.MethodType);
			writer.WritePropertyName("dice");
			writer.WriteValue(diceResult.Dice);
			writer.WritePropertyName("squareEvent");
			writer.WriteValue(diceResult.SquareEvent);
			writer.WritePropertyName("finalPosition");
			writer.WriteValue(diceResult.FinalPosition);

			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object? existingValue,
			JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);
			return new DiceResultMessage((int) jObject["dice"]!, (string) jObject["squareEvent"]!,
				(int) jObject["finalObject"]!);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(DiceResultMessage);
		}
	}
}