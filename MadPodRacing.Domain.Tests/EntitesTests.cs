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
            IGameManager gameService = new GameManager();

            gameService.ReadTurn();
            var result = gameService.Play();
            Assert.IsNotNull(result);

            ConsoleHelpers.PushInReader(msg[1]);
            gameService.ReadTurn();
            result = gameService.Play();
            Assert.IsNotNull(result);

        }

        [Test]
        public void RaceEntitie()
        {
            var race = new Race
            {
                Points = new HashSet<RacePoint>
                {
                    new RacePoint { Id = 1, X = 5, Y = 0, IsCurrent = true },
                    new RacePoint { Id = 2, X = 6, Y = 0, IsCurrent = false },
                    new RacePoint { Id = 3, X = 7, Y = 0, IsCurrent = false },
                    new RacePoint { Id = 4, X = 8, Y = 0, IsCurrent = false },
                }
            };

            var n = race.NextPoint();
            Assert.AreEqual(1, n.Id);

            var p = race.PreviousPoint();
            Assert.AreEqual(4, p.Id);

            race.Points.ToList().ForEach(p => p.IsCurrent = false);
            race.Points.ToList()[3].IsCurrent = true;

            n = race.NextPoint();
            Assert.AreEqual(4, n.Id);

            p = race.PreviousPoint();
            Assert.AreEqual(3, p.Id);



        }
    }
}
