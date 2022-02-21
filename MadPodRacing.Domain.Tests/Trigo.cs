using MadPodRacing.Domain.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using MadPodRacing.Domain.Entities;
using System.Numerics;

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


            var angle = AngleHelpers.ComputeAngle(
                new Point { X = 1, Y = 0},
                new Point {  X = 0, Y = 1 });

            Assert.AreEqual(90, angle);

            angle = AngleHelpers.ComputeAngle(
                new Point { X = 0, Y = 1 },
                new Point { X = 1, Y = 0 });

            Assert.AreEqual(90, angle);


           

        }

        [Test]
        public void MathVector()
        {
            var toCheckpoint = new Vector2[] { new Vector2((float)-1600.76065655, (float)-2448.35285522) };
            var R = new Vector2[] { new Vector2 ((float)0.99978068, (float)-0.02094242), new Vector2 ((float)0.02094242, (float)0.99978068) };

            var t = Vector2.Dot(R[0], toCheckpoint[0]);
            var t2 = Vector2.Dot(R[1], toCheckpoint[0]);

            Assert.AreEqual(-1549.13513f, t);
            Assert.AreEqual(-2481.33936f, t2);


        }
    }
}
