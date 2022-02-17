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

        public override string ToString()
        {
            return $"[{X}:{Y}]";
        }
    }

    public class PlayerPoint
        : Point
    {

        /// <summary>
        /// distance to the next checkpoint
        /// </summary>
        public int Distance { get; set; }
        /// <summary>
        /// angle between your pod orientation and the direction of the next checkpoint
        /// </summary>
        public int Angle { get; set; }
    }

    public class RacePoint
        : Point
    {
        public bool IsCurrent { get; set; }
        public int Id { get; set; }
        public int DistanceWithNextPoint { get; set; }

    }
}
