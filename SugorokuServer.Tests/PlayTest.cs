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

        // サーバーに送る通信要求のbody文字列はClientToServerにあるクラスのインスタンスを作成し
        // それをSerializeObject()すると作成できます。
        // 作成できたbodyをLibrary/Protocol/HeaderProtocol.MakeHeader(body, true)で投げて
        // 返った文字列を送信してください。
        private static object[] _sugorokuPlayTestCase =
        {
            // CreatePlayerMessage は2引数に取る文字列(MatchKey)で開かれている部屋があればそこに参加、
            // なければその部屋名で新規の参加を待ちます。
            // ばぬしで生成した Player.cs インスタンスをシリアライズした文字列がbodyで返るので
            // デシリアライズしたものをClientで持っておいてください
            new object[]
            {
                JsonConvert.SerializeObject(new CreatePlayerMessage("ばぬし", "aaa")),
                true
            },
            // aaaという部屋は↑で作成されているので「ねこ」はその部屋に参加します
            new object[]
            {
                JsonConvert.SerializeObject(new CreatePlayerMessage("ねこ", "aaa")),
                true
            },
            // aaaという部屋の新規のプレイヤーの参加を締め切ります。
            // (TODO このメッセージを送ったのが「ばぬし」かどうかの判定はまだ書いていません)
            new object[]
            {
                JsonConvert.SerializeObject(new CloseCreateMessage("aaa")),
                true
            },
            // aaaという部屋の新規の参加は↑で締め切られているので「いぬ」は参加できません
            new object[]
            {
                JsonConvert.SerializeObject(new CreatePlayerMessage("いぬ", "aaa")),
                false
            },
            // サイコロを振る要求を飛ばします。(playerId: 1 は「ばぬし」)
            new object[]
            {
                JsonConvert.SerializeObject(new DiceMessage("aaa", 1)),
                true
            },
            // ↑で「ばぬし」がサイコロを振ったはずなのでplayer:1のターンではない→false
            new object[]
            {
                JsonConvert.SerializeObject(new DiceMessage("aaa", 1)),
                false
            },
            // 「ねこ」がサイコロを振る要求
            new object[]
            {
                JsonConvert.SerializeObject(new DiceMessage("aaa", 2)),
                true
            },
            // 以下「一回休み」が出た場合はtrueでない値になるためテスト失敗の可能性あり
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