namespace MadPodRacing.Domain.Entities
{
    using MadPodRacing.Domain.Helpers;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class Race
    {
        public Race()
        {
            Points = new HashSet<RacePoint>(new RacePointComparer());
        }

        public HashSet<RacePoint> Points { get; set; }
        /// <summary>
        /// Nombre de tours
        /// </summary>
        public int Lap { get; set; } = 1;
    }

    public class RacePointComparer : IEqualityComparer<RacePoint>
    {
        public bool Equals([AllowNull] RacePoint p1, [AllowNull] RacePoint p2)
        {
            return p1.IsEquals(p2);
        }

        public int GetHashCode([DisallowNull] RacePoint obj)
        {
            return obj.X ^ obj.Y;

        }
    }
}
