using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.ClientToServer
{
	public class ClientMessageConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);
			JsonConverter converter = (string) jObject["methodType"]! switch
			{
				"closeCreate" => new CloseCreateConverter(),
				"createPlayer" => new CreatePlayerConverter(),
				"getMatchInfo" => new GetMatchInfoConverter(),
				"getAllMatches" => new GetAllMatchesConverter(),
				_ => throw new ArgumentException()
			};
			var newReader = jObject.CreateReader();
			return converter.ReadJson(newReader, objectType, existingValue, serializer);
		}

		public override bool CanConvert(Type typeToConvert)
		{
			return typeToConvert == typeof(ClientMessage);
		}

		public override bool CanWrite => false;
	}
}