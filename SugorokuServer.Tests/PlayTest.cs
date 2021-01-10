using System;
using Newtonsoft.Json;
using NUnit.Framework;
using SugorokuLibrary.ClientToServer;

namespace SugorokuServer.Tests
{
    public class PlayTest
    {
        private HandleClient _handleClient;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _handleClient = new HandleClient();
        }

        private static object[] _sugorokuPlayTestCase =
        {
            new object[]
            {
                JsonConvert.SerializeObject(new CreatePlayerMessage("ばぬし", "1")),
                true
            },
            new object[]
            {
                JsonConvert.SerializeObject(new CreatePlayerMessage("ねこ", "1")),
                true
            },
            new object[]
            {
                JsonConvert.SerializeObject(new CloseCreateMessage("1")),
                true
            },
            new object[]
            {
                JsonConvert.SerializeObject(new CreatePlayerMessage("いぬ", "1")),
                false
            },
            new object[]
            {
                JsonConvert.SerializeObject(new DiceMessage("1", 1, 0)),
                true
            },
            new object[]
            {
                JsonConvert.SerializeObject(new DiceMessage("1", 1, 0)),
                false
            },
            new object[]
            {
                JsonConvert.SerializeObject(new DiceMessage("1", 2, 0)),
                true
            }
        };

        [TestCaseSource(nameof(_sugorokuPlayTestCase))]
        public void GamePlayingTest(string receivedMsg, bool exp)
        {
            var sendingMsg = _handleClient.MakeSendMessage(receivedMsg);
            Console.WriteLine(sendingMsg);
            var (bufSize, isTrueMessage, msg) = SugorokuLibrary.Protocol.HeaderProtocol.ParseHeader(sendingMsg);
            Assert.AreEqual(bufSize, msg.Length);
            Assert.AreEqual(exp, isTrueMessage);
        }
    }
}