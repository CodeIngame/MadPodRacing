namespace MadPodRacing.Domain.Entities
{
    using MadPodRacing.Domain.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point()
        {
        }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return $"[{X}:{Y}]";
        }
    }

    public class PlayerPoint
        : Point
    {
        public PlayerPoint()
        {
        }

        public PlayerPoint(int x, int y) : base(x, y)
        {

        }

        /// <summary>
        /// distance to the next checkpoint
        /// </summary>
        public int Distance { get; set; }
        /// <summary>
        /// angle between your pod orientation and the direction of the next checkpoint
        /// </summary>
        public int Angle { get; set; }
    }

    public class CheckPoint
        : Point
    {

        public CheckPoint()
        {

        }

        public CheckPoint(int x, int y) : base(x, y)
        {

        }

        public bool IsCurrent { get; set; }
        public int Id { get; set; }

        /// <summary>
        /// Computed X
        /// </summary>
        public int NewX { get; set; }
        /// <summary>
        /// Computed Y
        /// </summary>
        public int NewY { get; set; }

    }
}
