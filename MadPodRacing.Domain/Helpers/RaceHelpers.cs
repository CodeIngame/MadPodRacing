

namespace MadPodRacing.Domain.Helpers
{
    using MadPodRacing.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class RaceHelpers
    {
        public static RacePoint NextPoint(this Race r)
        {
            return r.Points.First(p => p.IsCurrent);
        }

        public static RacePoint PreviousPoint(this Race r)
        {
            var currentIndex = r.Points.ToList().IndexOf(r.NextPoint());
            var index = currentIndex == 0 ? r.Points.Count - 1 : currentIndex - 1;
            return r.Points.ToList()[index];
        }
    }
}
