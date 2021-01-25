using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.ServerToClient.Converters
{
    public class RankingMessageConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var ranking = (RankingMessage) value!;
            
            writer.WriteStartObject();
            writer.WritePropertyName("methodType");
            writer.WriteValue(ranking.MethodType);
            writer.WritePropertyName("ranking");
            writer.WriteStartArray();
            foreach (var rank in ranking.Ranking)
            {
                writer.WriteValue(rank);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            return new RankingMessage(jObject["ranking"]!.ToObject<int[]>()!);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(RankingMessage);
        }
    }
}