

namespace MadPodRacing.Domain.Helpers
{
    using MadPodRacing.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class PointHelpers
    {
        public static bool IsEquals(this Point p1, Point p2)
        {
            return (p1.X == p2.X && p1.Y == p2.Y);
        }

        public static double ToManhattanDistance(this Point p1, Point p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }

        public static double ToChebyshevDistance(this Point p1, Point p2)
        {
            return Math.Max(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
        }

        public static double ToEuclideDistance(this Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        public static double ToPythagore(this Point p1)
        {
            return Math.Sqrt(Math.Pow(p1.X, 2) + Math.Pow(p1.Y, 2));
        }

        public static double ToPythagore(this int[] p1)
        {
            if (p1.Length == 0 || p1.Length > 2) throw new ArgumentException($"{nameof(p1)} length is bad");
            return Math.Sqrt(Math.Pow(p1[0], 2) + Math.Pow(p1[1], 2));
        }
    }
}
