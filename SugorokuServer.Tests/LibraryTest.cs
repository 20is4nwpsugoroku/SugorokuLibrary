using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;
using SugorokuLibrary;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.SquareEvents;

namespace SugorokuServer.Tests
{
	public class LibraryTest
	{
		private Field _field;

		[OneTimeSetUp]
		public void SetUp()
		{
			_field = new Field();
		}

		private static readonly object[] JsonParseTestCases =
		{
			new object[] {"{\"methodType\":\"closeCreate\",\"matchKey\":\"abc\"}", new CloseCreateMessage("abc")},
			new object[]
			{
				"{\"methodType\":\"createPlayer\",\"playerName\":\"ばぬし\",\"matchKey\":\"abc\"}",
				new CreatePlayerMessage("ばぬし", "abc")
			},
			new object[]
			{
				"{\"methodType\":\"dice\",\"playerId\":0,\"nowPosition\":3,\"matchKey\":\"abc\"}",
				new DiceMessage("abc", 0)
			}
		};

		[TestCaseSource(nameof(JsonParseTestCases))]
		public void JsonテキストからCsharpクラスにコンバートできるかテスト(string jsonText, ClientMessage exp)
		{
			var parsed = JsonConvert.DeserializeObject<ClientMessage>(jsonText);
			Assert.AreEqual(exp, parsed);
		}

		[TestCaseSource(nameof(JsonParseTestCases))]
		public void CSharpクラスからJsonテキストにコンバートできるかテスト(string exp, ClientMessage abc)
		{
			var text = JsonConvert.SerializeObject(abc);
			Assert.AreEqual(JsonConvert.DeserializeObject<Dictionary<string, object>>(exp),
				JsonConvert.DeserializeObject<Dictionary<string, object>>(text));
		}
	}
}