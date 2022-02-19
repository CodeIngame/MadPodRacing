using MadPodRacing.Domain.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPodRacing.Domain.Tests
{
    public class Trigo
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void MathTrigo()
        {
            Assert.AreEqual(6.123233995736766E-17d, Math.Cos(90.ToRadian()));
            Assert.AreEqual(1, Math.Sin(90.ToRadian()));
            Assert.AreEqual(-1, Math.Sin(-90.ToRadian()));
            Assert.AreEqual(1, Math.Cos(0.ToRadian()));

        }
    }
}
