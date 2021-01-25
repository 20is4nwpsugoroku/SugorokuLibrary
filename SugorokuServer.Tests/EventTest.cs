using System;
using System.Collections.Generic;
using NUnit.Framework;
using SugorokuLibrary;
using SugorokuLibrary.Match;

namespace SugorokuServer.Tests
{
    public class EventTest
    {
        private MatchCore _core;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var matchInfo = new MatchInfo
            {
                Players = new List<Player>
                {
                    new Player
                    {
                        IsHost = true,
                        MatchKey = "test",
                        PlayerID = 1,
                        PlayerName = "ばぬし",
                        Position = 0,
                        Wait = false
                    },
                    new Player
                    {
                        IsHost = false,
                        MatchKey = "test",
                        PlayerID = 2,
                        PlayerName = "ねこ",
                        Position = 0,
                        Wait = false
                    }
                },
                CreatePlayerClosed = true,
                HostPlayerID = 1,
                MatchKey = "test",
                NextPlayerID = 1,
                Turn = 0
            };
            _core = new MatchCore(matchInfo);
            _core.Start();
        }

        [Test]
        public void ばぬしが1回休みを踏んだときのテスト()
        {
            _core.Players[1].Position = 18;

            var step = new PlayerAction
            {
                Length = 1, PlayerID = 1
            };

            _core.ReflectAction(step);

            var exp = new List<int> {2, 2, 1, 2, 1};
            for (var i = 0; i < Math.Min(exp.Count, _core.ActionSchedule.Count); i++)
            {
                Assert.AreEqual(exp[i], _core.ActionSchedule[i]);
            }

            Assert.AreEqual(2, _core.MatchInfo.NextPlayerID);
        }

        [Test]
        public void ばぬしが一回休みを踏んだあとねこも一回休みを踏んで結局順番通りだよねっていうテスト()
        {
            _core.Players[1].Position = 18;
            _core.Players[2].Position = 18;

            var step1 = new PlayerAction
            {
                Length = 1, PlayerID = 1
            };
            _core.ReflectAction(step1);

            var exp = new List<int> {2, 2, 1, 2, 1};
            for (var i = 0; i < Math.Min(exp.Count, _core.ActionSchedule.Count); i++)
            {
                Assert.AreEqual(exp[i], _core.ActionSchedule[i]);
            }

            Assert.AreEqual(2, _core.MatchInfo.NextPlayerID);

            var step2 = new PlayerAction
            {
                Length = 1, PlayerID = 2
            };
            _core.ReflectAction(step2);

            var exp2 = new List<int> {1, 2, 1, 2, 1, 2};
            for (var i = 0; i < Math.Min(exp2.Count, _core.ActionSchedule.Count); i++)
            {
                Assert.AreEqual(exp2[i], _core.ActionSchedule[i]);
            }

            Assert.AreEqual(1, _core.MatchInfo.NextPlayerID);
        }

        [Test]
        public void ばぬしが1回休みの間にねこがもう一回回すを出したらやばいことにならないか不安でしょうがないテスト()
        {
            _core.Players[1].Position = 18;
            _core.Players[2].Position = 20;

            var step1 = new PlayerAction
            {
                Length = 1, PlayerID = 1
            };
            _core.ReflectAction(step1);

            var exp = new List<int> {2, 2, 1, 2, 1};
            for (var i = 0; i < Math.Min(exp.Count, _core.ActionSchedule.Count); i++)
            {
                Assert.AreEqual(exp[i], _core.ActionSchedule[i]);
            }

            Assert.AreEqual(2, _core.MatchInfo.NextPlayerID);

            var step2 = new PlayerAction
            {
                Length = 1, PlayerID = 2
            };
            _core.ReflectAction(step2);

            var exp2 = new List<int> {1, 2, 1, 2, 1, 2};
            for (var i = 0; i < Math.Min(exp2.Count, _core.ActionSchedule.Count); i++)
            {
                Assert.AreEqual(exp2[i], _core.ActionSchedule[i]);
            }

            Assert.AreEqual(1, _core.MatchInfo.NextPlayerID);
        }

        [Test]
        public void ばぬしがDiceAgainを引いたらもっかいばぬしのターンをやったあとねこのターンになるよねって信じたいテスト()
        {
            _core.Players[1].Position = 20;

            var step1 = new PlayerAction
            {
                Length = 1, PlayerID = 1
            };
            _core.ReflectAction(step1);
            
            Assert.AreEqual(1, _core.MatchInfo.NextPlayerID);

            _core.ReflectAction(step1);
            
            Assert.AreEqual(2, _core.MatchInfo.NextPlayerID);
        }

        [Test]
        public void まじでサイコロの数だけ戻るが機能してほしいテスト()
        {
            _core.Players[1].Position = 25;
            var step = new PlayerAction
            {
                Length = 1, PlayerID = 1
            };
            _core.ReflectAction(step);
            
            Assert.AreEqual(1, _core.MatchInfo.NextPlayerID);
            Assert.AreEqual(26, _core.Players[1].Position);

            _core.ReflectAction(step);
            Assert.AreEqual(25, _core.Players[1].Position);
            Assert.AreEqual(2, _core.MatchInfo.NextPlayerID);
        }
    }
}