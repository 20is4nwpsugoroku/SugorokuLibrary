using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.SquareEvents.Converter
{
    public class SquareEventConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            JsonConverter converter = (string) jObject["eventType"]! switch
            {
                "none" => new NoneEventConverter(),
                "goStart" => new GoStartEventConverter(),
                "diceAgain" => new DiceAgainEventConverter(),
                "next" => new NextEventConverter(),
                "prevDice" => new PrevDiceEventConverter(),
                "prev" => new PrevEventConverter(),
                "wait" => new WaitEventConverter(),
                _ => throw new ArgumentException()
            };
            var newReader = jObject.CreateReader();
            return converter.ReadJson(newReader, objectType, existingValue, serializer);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SquareEvent);
        }

        public override bool CanWrite => false;
    }
}