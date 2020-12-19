using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using SugorokuLibrary;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.Protocol;

namespace SugorokuServer.Tests
{
	public class Tests
	{
		private HandleClient _handleClient;

		private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver()
		};

		[SetUp]
		public void Setup()
		{
			_handleClient = new HandleClient();
		}

		private static object[] _testCases =
		{
			new object[]
			{
				JsonConvert.SerializeObject(new CreatePlayerMessage("ばぬし", "12345"), Settings),
				new Player
				{
					IsHost = true, MatchKey = "12345", PlayerID = 1, PlayerName = "ばぬし", Position = 0
				}
			},
			new object[]
			{
				JsonConvert.SerializeObject(new CreatePlayerMessage("ねこ", "12345"), Settings),
				new Player
				{
					IsHost = false, MatchKey = "12345", PlayerID = 2, PlayerName = "ねこ", Position = 0
				}
			}
		};

		private static object[] _dateTimeToTimeStamp =
		{
			new object[] {new DateTime(2020, 12, 19, 21, 15, 0), 1608380100},
			new object[] {new DateTime(2021, 1, 1, 0, 0, 0), 1609426800}
		};

		private static object[] _timeStampToDateTime =
		{
			new object[] {1608380100, new DateTime(2020, 12, 19, 21, 15, 0)},
			new object[] {1609426800, new DateTime(2021, 1, 1, 0, 0, 0)}
		};

		[TestCaseSource(nameof(_dateTimeToTimeStamp))]
		public void DateTimeからタイムスタンプの変換をチェックするの(DateTime dt, long exp)
		{
			Assert.AreEqual(exp, dt.ToTimeStamp());
		}

		[TestCaseSource(nameof(_timeStampToDateTime))]
		public void タイムスタンプからDateTimeへの変換をチェックするの(long timeStamp, DateTime exp)
		{
			Assert.AreEqual(exp, timeStamp.ToDateTime());
		}

		[TestCaseSource(nameof(_testCases))]
		public void CreatePlayerTest(string inputMsg, Player exp)
		{
			var respondMessage = _handleClient.MakeSendMessage(inputMsg);
			var (_, _, body) = HeaderProtocol.ParseHeader(respondMessage);

			var deserialized = JsonConvert.DeserializeObject<Player>(body, Settings);

			Assert.AreEqual(exp, deserialized);
		}
	}
}