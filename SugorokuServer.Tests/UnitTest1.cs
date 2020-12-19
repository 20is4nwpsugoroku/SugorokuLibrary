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
				JsonConvert.SerializeObject(new Player
				{
					IsHost = true, MatchKey = "12345", PlayerID = 1, PlayerName = "ばぬし", Position = 0
				}, Settings)
			},
			new object[]
			{
				JsonConvert.SerializeObject(new CreatePlayerMessage("ねこ", "12345"), Settings),
				JsonConvert.SerializeObject(new Player
				{
					IsHost = false, MatchKey = "12345", PlayerID = 2, PlayerName = "ねこ", Position = 0
				}, Settings)
			}
		};

		[TestCaseSource(nameof(_testCases))]
		public void CreatePlayerTest(string inputMsg, string exp)
		{
			var respondMessage = _handleClient.MakeSendMessage(inputMsg);
			var (_, _, body) = HeaderProtocol.ParseHeader(respondMessage);

			Assert.AreEqual(body, exp);
		}
	}
}