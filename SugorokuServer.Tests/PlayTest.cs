using System;
using Newtonsoft.Json;
using NUnit.Framework;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.Protocol;

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
                JsonConvert.SerializeObject(new CreatePlayerMessage("ばぬし", "aaa")),
                true
            },
            new object[]
            {
                JsonConvert.SerializeObject(new CreatePlayerMessage("ねこ", "aaa")),
                true
            },
            new object[]
            {
                JsonConvert.SerializeObject(new CloseCreateMessage("aaa")),
                true
            },
            new object[]
            {
                JsonConvert.SerializeObject(new CreatePlayerMessage("いぬ", "aaa")),
                false
            },
            new object[]
            {
                JsonConvert.SerializeObject(new DiceMessage("aaa", 1)),
                true
            },
            new object[]
            {
                JsonConvert.SerializeObject(new DiceMessage("aaa", 1)),
                false
            },
            new object[]
            {
                JsonConvert.SerializeObject(new DiceMessage("aaa", 2)),
                true
            },
            new object[]
            {
                JsonConvert.SerializeObject(new DiceMessage("aaa", 1)),
                true
            },
            new object[]
            {
                JsonConvert.SerializeObject(new DiceMessage("aaa", 2)),
                true
            },
            
            new object[]
            {
                JsonConvert.SerializeObject(new DiceMessage("aaa", 1)),
                true
            },
            new object[]
            {
                JsonConvert.SerializeObject(new DiceMessage("aaa", 2)),
                true
            }
        };

        [TestCaseSource(nameof(_sugorokuPlayTestCase))]
        public void GamePlayingTest(string receivedMsg, bool exp)
        {
            var sendingMsg = _handleClient.MakeSendMessage(receivedMsg);
            Console.WriteLine(sendingMsg);
            var (bufSize, isTrueMessage, msg) = HeaderProtocol.ParseHeader(sendingMsg);
            Assert.AreEqual(bufSize, msg.Length);
            Assert.AreEqual(exp, isTrueMessage);
        }
    }
}