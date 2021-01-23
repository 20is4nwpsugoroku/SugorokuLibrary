using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.SquareEvents.Converter
{
    public class PrevDiceEventConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            return new PrevDiceEvent((string) jObject["message"]!);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PrevDiceEvent);
        }
    }
}