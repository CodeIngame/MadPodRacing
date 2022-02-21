namespace MadPodRacing.Domain.Entities
{
    using MadPodRacing.Domain.Helpers;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class Race
    {
        public Race()
        {
            CheckPoint = new List<CheckPoint>();
        }

        public List<CheckPoint> CheckPoint { get; set; }

        /// <summary>
        /// Nombre de tours
        /// </summary>
        public int Lap { get; set; } = 1;
    }

    public class RacePointComparer : IEqualityComparer<CheckPoint>
    {
        public bool Equals([AllowNull] CheckPoint p1, [AllowNull] CheckPoint p2)
        {
            return p1.IsEquals(p2);
        }

        public int GetHashCode([DisallowNull] CheckPoint obj)
        {
            return obj.X ^ obj.Y;

        }
    }
}
