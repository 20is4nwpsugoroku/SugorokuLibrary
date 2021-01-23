using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.ServerToClient.Converters
{
    public class AlreadyFinishedMessageConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var alreadyFinished = (AlreadyFinishedMessage) value!;
            
            writer.WriteStartObject();
            writer.WritePropertyName("methodType");
            writer.WriteValue(alreadyFinished.MethodType);
            writer.WritePropertyName("goaledPlayerId");
            writer.WriteValue(alreadyFinished.GoaledPlayerId);
            writer.WritePropertyName("ranking");
            writer.WriteValue(alreadyFinished.Ranking.ToList());
            writer.WriteEndObject();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            return new AlreadyFinishedMessage((int) jObject["goaledPlayerId"]!,
                jObject.SelectToken("ranking")?.ToObject<int[]>()!);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AlreadyFinishedMessage);
        }
    }
}