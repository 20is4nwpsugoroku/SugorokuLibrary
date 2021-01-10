using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.ClientToServer.Converters
{
    public class PrevDiceMessageConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var diceMessage = (PrevDiceMessage) value!;

            writer.WriteStartObject();
            writer.WritePropertyName("methodType");
            writer.WriteValue(diceMessage.MethodType);
            writer.WritePropertyName("matchKey");
            writer.WriteValue(diceMessage.MatchKey);
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
            return new PrevDiceMessage((string) jObject["matchKey"]!, (int) jObject["playerId"]!,
                (int) jObject["nowPosition"]!);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PrevDiceMessage);
        }
    }
}