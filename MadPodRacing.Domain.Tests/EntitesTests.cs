using MadPodRacing.Domain.Entities;
using MadPodRacing.Domain.Helpers;
using MadPodRacing.Domain.Interface;
using MadPodRacing.Domain.Manager;
using MadPodRacing.Domain.Tests.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MadPodRacing.Domain.Tests
{
    public class EntitesTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Play()
        {
            //Arrange
            var msg = new List<List<string>>
            {
                 // 1st round
                new List<string>
                {
                    "5073 7793 4525 6001 1874 0",
                    "4785 6396"
                },
                // 2nd round
                new List<string>
                {
                    "5066 7698 4525 6001 1775 0",
                    "4785 6396"
                },
            };
            ConsoleHelpers.PushInReader(msg[0]);
            GameManager gameService = new GameManager();

            gameService.Initialize();
            gameService.ReadTurn();
            var result = gameService.Play();
            Assert.IsNotNull(result);

            var me = gameService.Me;

            Assert.AreEqual(me.Position.Distance, Math.Ceiling(me.ComputeDistanceNextCheckPoint()));
            Assert.IsTrue(me.ComputeSpeed() == 0);


            ConsoleHelpers.PushInReader(msg[1]);
            gameService.ReadTurn();
            result = gameService.Play();
            Assert.IsNotNull(result);
            Assert.IsTrue(me.ComputeSpeed() > 0);


        }

        [Test]
        public void RaceEntitie()
        {
            var race = new Race
            {
                CheckPoint = new List<CheckPoint>
                {
                    new CheckPoint { Id = 1, X = 5, Y = 0, IsCurrent = true },
                    new CheckPoint { Id = 2, X = 6, Y = 0, IsCurrent = false },
                    new CheckPoint { Id = 3, X = 7, Y = 0, IsCurrent = false },
                    new CheckPoint { Id = 4, X = 8, Y = 0, IsCurrent = false },
                }
            };

            var n = race.NextPoint();
            Assert.AreEqual(1, n.Id);

            var p = n.Previous(race);
            Assert.AreEqual(4, p.Id);

            var pp = n.Previous(race)
                .Previous(race);
            Assert.AreEqual(3, pp.Id);

            var nn = n.Next(race);
            Assert.AreEqual(2, nn.Id);

            race.CheckPoint.ToList().ForEach(p => p.IsCurrent = false);
            race.CheckPoint.ToList()[3].IsCurrent = true;

            n = race.NextPoint();
            Assert.AreEqual(4, n.Id);

            p = n.Previous(race);
            Assert.AreEqual(3, p.Id);

            nn = n.Next(race);
            Assert.AreEqual(1, nn.Id);

            var added = race.CheckPoint.TryAdd(new CheckPoint { Id = 1, X = 5, Y = 0, IsCurrent = false });
            Assert.IsFalse(added);

            added = race.CheckPoint.TryAdd(new CheckPoint { Id = 1, X = 1, Y = 0, IsCurrent = false });
            Assert.IsTrue(added);


        }
    }
}
