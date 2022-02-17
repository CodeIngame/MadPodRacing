using MadPodRacing.Domain.Interface;
using MadPodRacing.Domain.Manager;
using MadPodRacing.Domain.Tests.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPodRacing.Domain.Tests
{
    class EntitesTests
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
    }
}
