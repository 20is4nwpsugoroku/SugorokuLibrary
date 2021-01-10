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

		private static readonly ISquareEvent[] FieldTestCases =
		{
			new NoneEvent(), // 0
			new NextEvent {NextCount = 2}, // 1
			new NoneEvent(), // 2
			new NoneEvent(), // 3
			new NoneEvent(), // 4
			new NextEvent {NextCount = 3}, // 5
			new GoStartEvent(), // 6
			new DiceAgainEvent(), // 7
			new NoneEvent(), // 8
			new NoneEvent(), // 9
			new WaitEvent(), // 10
			new NoneEvent(), // 11
			new PrevEvent {BackCount = 2}, // 12
			new NoneEvent(), // 13
			new NoneEvent(), // 14
			new NextEvent {NextCount = 3}, // 15
			new NoneEvent(), // 16
			new PrevDiceEvent(), // 17
			new NoneEvent(), // 18
			new WaitEvent(), // 19
			new NoneEvent(), // 20
			new DiceAgainEvent(), // 21
			new NoneEvent(), // 22
			new NoneEvent(), // 23
			new PrevEvent {BackCount = 4}, // 24
			new WaitEvent(), // 25
			new PrevDiceEvent(), // 26
			new NoneEvent(), // 27
			new DiceAgainEvent(), // 28
			new GoStartEvent(), // 29
			new NoneEvent() // 30
		};

		[Test]
		public void CheckSquares([Range(0, 30)] int count)
		{
			var expEvent = FieldTestCases[count];
			var target = Field.Squares[count];
			Assert.AreEqual(count, target.Index);
			Assert.IsInstanceOf(expEvent.GetType(), target.Event);
			switch (target.Event)
			{
				case PrevEvent p:
					Assert.AreEqual(((PrevEvent) expEvent).BackCount, p.BackCount);
					break;
				case NextEvent n:
					Assert.AreEqual(((NextEvent) expEvent).NextCount, n.NextCount);
					break;
			}
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
			},
			new object[]
			{
				"{\"methodType\":\"prevDice\",\"playerId\":0,\"nowPosition\":7,\"matchKey\":\"abc\"}",
				new PrevDiceMessage("abc", 0)
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