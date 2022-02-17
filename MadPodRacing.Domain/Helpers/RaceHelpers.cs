

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
    }
}
