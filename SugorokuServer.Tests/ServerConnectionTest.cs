using System;
using System.Net.Sockets;
using Newtonsoft.Json;
using NUnit.Framework;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.Protocol;

namespace SugorokuServer.Tests
{
    public class ServerConnectionTest
    {
        private Socket _socket;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Program.Main(new string[] { });
        }

        [SetUp]
        public void SetUp()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect("127.0.0.1", 9500);
        }

        [TearDown]
        public void TearDown()
        {
            _socket.Close();
        }

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
        public void ああああ(string input, bool exp)
        {
            var withHeader = HeaderProtocol.MakeHeader(input, true);
            var (s, r, m) = Connection.SendAndRecvMessage(withHeader, _socket);
            Console.WriteLine($"{s} {r} {m}");
        }
    }
}